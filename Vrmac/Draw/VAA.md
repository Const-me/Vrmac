# Vrmac Anti-Aliasing

This document describes VAA algorithm used for 2D vector graphics in Vrmac Draw.

There’re multiple AA implementations there.<br/>
You can enable Direct2D backend instead of Vrmac Draw, and there’s high quality LCD-aware font AA in FreeType.

This document is only related to rendering 2D vector graphics, and only when using the built-in Vrmac Draw backend.

## High Level Architecture

The upstream part of pipeline is quite similar to MS Direct2D.

Specifically, as a first step both libraries are converting splines into polylines, using precision derived from the shape’s current transformation matrix, and physical pixels density.

Then, they both are building triangular meshes from these polylines.

The last step however is different. Direct2D uses two-pass rendering, they first rendering each shape into `R16_UINT` texture with forced 16x MSAA, and a pixel shader which outputs `SV_Coverage` system-generated value into that texture. On the main pass, they read from that texture to get per-pixel AA values.
Therefore, they need at least 2 draw calls for every shape rendered.

Vrmac Draw renders the complete 2D scene in a single pass with just 2 draw calls.<br/>
Initially I also used hardware MSAA BTW, but I quickly discovered even with low MSAA levels like 4, Raspberry Pi4 GPU is not remotely fast enough to render simple 2D at 60Hz/FullHD.

## Data Flow

1. User creates a [2D draw device](https://github.com/Const-me/Vrmac/blob/1.2/Vrmac/Draw/Main/DrawDevice.cs).

2. User creates various resources on that device, such as path geometries.
That data is copied from managed to unmanaged memory.
iVrmacDraw [COM interface](https://github.com/Const-me/Vrmac/blob/1.2/VrmacInterop/Draw/Render/iVrmacDraw.cs) serves as a factory.<br/>
That object, nor any other C++ classes related to Vrmac Draw vector graphics, doesn’t touch GPU at all.<br/>
The main reason why that code is written in C++ is missing support for NEON SIMD in .NET framework.
Even the current .NET 5 still doesn’t support NEON hardware intrinsics when running on ARMv7.
There’s some limited support when running on ARM64, but not the 32-bit one which was my target platform for this project.

3. User then sends these paths and brushes to render something by calling methods of [iDrawContext](https://github.com/Const-me/Vrmac/blob/1.2/Vrmac/Draw/iDrawContext.cs) interface.
At this point, the code still doesn’t touch the GPU, but the CPU starts processing these shapes right away.
The processing happens on background threads implemented by the .NET runtime, and vast majority of CPU time is spent running CPU-bound C++ code in the native DLL.
See [iTesselator interface](https://github.com/Const-me/Vrmac/blob/1.2/Vrmac/Draw/Tessellate/iTesselator.cs) for the API surface, and the rest of the ` *.cs` files in that subfolder for the implementation.

4. Finally, when user wants to present a frame they call Dispose() on the drawing context.
The [implementation is there](https://github.com/Const-me/Vrmac/blob/1.2/Vrmac/Draw/Main/ImmediateContext.api.cs#L228).
The code waits for all pending tessellation tasks, combines triangle meshes into a single vertex + index buffer in VRAM,
uploads per-draw call buffer, and finally submits up to 2 `DrawIndexed` calls to GPU, using same vertex/index buffers but different pipeline states and index ranges.

## Shaders

Shaders are in this repository, under MIT license:
[vertex shader](https://github.com/Const-me/Vrmac/blob/1.2/Vrmac/Draw/Shaders/drawVS.hlsl) and [pixel shader](https://github.com/Const-me/Vrmac/blob/1.2/Vrmac/Draw/Shaders/drawPS.hlsl).

When running on Linux, the GPU abstraction layer I’m using re-compiles these shaders from HLSL into GLSL, to render things with GLES instead of D3D12.

These two shaders are compiled multiple times each, with different set of preprocessor definitions.
Specifically, there’s [a cache](https://github.com/Const-me/Vrmac/blob/1.2/Vrmac/Draw/PipelineStates/StatesCache.cs) with pipeline state objects used by 2D rendering.<br/>
If you only interested in VAA but not the rest of the 2D engine, the most important preprocessor switch is `OPAQUE_PASS`.

## Draw Calls

The complete 2D scene is rendered with 2 draw calls.

The first draw calls defines `OPAQUE_PASS=1` when compiling both shaders, and renders opaque-only triangles.<br/>
The triangles in that part of the index buffer are in front to back order.
This saves millions of pixel shader invocation per frame due to early Z rejection inside GPUs.
When rendering opaque triangles, no VAA is happening, the code in the pixel shader is disabled by the macro.

The second draw call defines `OPAQUE_PASS=0` and renders translucent triangles in back to front order, to achieve correct alpha blending.
However, that render state still reads and tests Z buffer values. This way translucent triangles occluded behind opaque triangles should be rejected early, before running that relatively expensive pixel shader.

## Meshes

All 2D meshes are using [sVertexWithId structure](https://github.com/Const-me/Vrmac/blob/1.2/VrmacInterop/Draw/Render/geometryStructures.cs#L48-L61) for per-vertex data.

The structure takes 12 bytes of VRAM per vertex, and consists of float 2D coordinates, and one extra 32-bit integer.

The upper 24 bits of the extra integer contain 0-based index of the draw command within the draw call, this way I can merge very different draw commands (2D shapes, text glyphs, bitmaps, etc) in a single vertex/index buffer.

The draw call data is in [sDrawCallData](https://github.com/Const-me/Vrmac/blob/1.2/Vrmac/Draw/PipelineStates/sDrawCallData.cs) structure.
Each one takes 48 bytes of VRAM, but that’s OK because these structures are reused across many vertices belonging to the same input shapes.

## VAA Rendering Modes

The shaders support 4 distinct modes of operation for VAA. These values are defined in [eVaaKind enumeration](https://github.com/Const-me/Vrmac/blob/1.2/Vrmac/Draw/Shaders/eDrawCall.cs#L27-L33).

That value is set for every shape and uploaded to VRAM in `sDrawCallData` structure.<br/>
When shapes are merged into single vertex/index buffer, each shape uses it’s own value for rendering.

1. `None` is obvious, it disables VAA.

2. `Filled` is used for filled shapes.<br/>
The low byte of the per-vertex integer contains either 0 or 1. It’s set to 1 for vertices inside the mesh, and 0 for vertices on the outer edge of the mesh.<br/>
The C++ code which makes these meshes subdivides triangles similar to the following illustration:<br/>
![vaa-mesh-filled.png](vaa-mesh-filled.png).<br/>
The blue stuff is the original mesh as produced by [Libtess2 library](https://github.com/memononen/libtess2). The green stuff is subdivision made by extra C++ code which, when VAA is enabled, runs immediately after Libtess2 tesselator.<br/>
Only a small fraction of the screen-space pixels is occupied by the translucent triangles.
The majority of the area is inside triangles, and unless the brush has transparency, these triangles gonna go to the opaque part of the output mesh.
The shaders break when all 3 vertices of the triangle are outside ones, but C++ code is aware that’s why it split the thin triangle near the top left of the image into 2 pieces, inserting an inner vertex.<br/>
For an isolated triangle, I think it gonna insert just 1 inner vertex at the triangle’s center of mass, and connect it to all 3 vertices of the triangle.

3. `StrokedFat` VAA mode is used for stroked lines which are at least 1 physical pixel thick after the transforms.<br/>
The complete shape mesh goes into the translucent half of the output. The low byte of the per-vertex integer contains either 0, 1 or 2.<br/>
0 means the vertex is on the left edge of the curve.<br/>
1 means the vertex is inside the shape.<br/>
2 means the vertex is on the right edge of the curve.<br/>
The mesh looks more or less like the following illustration:<br/>
![vaa-mesh-stroked.png](vaa-mesh-stroked.png).<br/>
The light gray line is the input polyline, the blue triangular mesh is what I building from that data.<br/>
If you wonder what happens at line endings where left edge becomes right one — I simply duplicate 1 vertex at every end, flipping VAA byte value of the copy.<br/>
The exact shape and topology depends on many things: input data, desired resolution and DPI scaling, applied transform,
values in [sStrokeStyle structure](https://github.com/Const-me/Vrmac/blob/1.2/VrmacInterop/Draw/Path/sStrokeStyle.cs) and a few others like viewport clipping.<br/>
I wasn’t able to find a good enough library which generates stroked meshes like that, that part is 100% custom C++ code.

4. `StrokedThin` VAA mode is used for stroked lines which are less than 1 pixel after the transforms.<br/>
The mesh is exactly the same as for `StrokedFat` VAA mode, there’s minor differences in HLSL and C# code for that VAA mode.

## Final Words

Given all the information above, it should now be clear what precisely these two shaders are doing.

The vertex shader [is converting](https://github.com/Const-me/Vrmac/blob/1.2/Vrmac/Draw/Shaders/drawVS.hlsl#L154-L174) these per-vertex magic bytes into a `float` number.<br/>
It then [outputs](https://github.com/Const-me/Vrmac/blob/1.2/Vrmac/Draw/Shaders/drawVS.hlsl#L181-L185) that number in vertex attributes.
It also [copies](https://github.com/Const-me/Vrmac/blob/1.2/Vrmac/Draw/Shaders/drawVS.hlsl#L114-L116) VAA type `uint` constant from the draw call into another vertex attribute.

GPU then interpolates the float numbers across 3 vertices of the triangle.<br/>
That interpolation is implemented in hardware. Really fast.
GPUs have that hardware because they’re interpolating quite a few values within triangles: positions, texture coordinates, and colors and interpolated over the triangle as well.

Pixel shader receives the interpolated value, and does different things based on VAA mode of the draw command.

[For filled shapes](https://github.com/Const-me/Vrmac/blob/1.2/Vrmac/Draw/Shaders/drawPS.hlsl#L120-L123),
it uses screen-space derivatives of that value to detect when the pixel is near the outer edge of the shape, and reduces the alpha accordingly.

[For fat strokes](https://github.com/Const-me/Vrmac/blob/1.2/Vrmac/Draw/Shaders/drawPS.hlsl#L132-L136),
it uses screen-space derivatives of that value to detect when the current pixel is in close proximity of uv=±1.0 iso-lines, and again reduces the alpha accordingly.

[For thin strokes](https://github.com/Const-me/Vrmac/blob/1.2/Vrmac/Draw/Shaders/drawPS.hlsl#L141-L142),
it computes distance to uv=0 line (which is the geometric center of the stroke), and uses that value to reduce the alpha.

If you’re unfamiliar with screen-space derivatives, [here’s a good answer](https://gamedev.stackexchange.com/a/130933/) on stackoverflow.<br/>
That question was about OpenGL, but that feature is also available in Direct3D, Vulkan, Metal, etc.

### Performance Considerations

Due to these screen-space derivatives, filled meshes aren’t required to have the outer translucent edges to be exactly 1 pixel wide.<br/>
Similarly, stroked meshes aren’t required to be inflated by exactly 0.5 pixels compared to the input stroke width.

The outer translucent edge needs to be at least sqrt(2)=1.414 pixels wide (the length of diagonal for the worst case of 45° oriented edge).<br/>
Can be more than that too, an edge of 2-4 pixels is fine as well and should look equally good, but note you generally want to minimize count of pixels rendered with translucent draw call.
Opaque stuff is way more efficient to render.

My implementation caches these meshes rather aggressively. I’m only re-generating them when zoom changes by at least a factor of 2.

Despite the meshes can be reused after arbitrary changes to orientation and translation, I still re-generate them when the shapes are translated or rotated substantially.<br/>
The reason for that, the C++ code which generates these meshes can optionally do viewport culling.<br/>
It appears that on Pi4 in my test cases, the profit from fewer vertices and triangles due to viewport culling outweighs costs of more frequent re-generation of these meshes.