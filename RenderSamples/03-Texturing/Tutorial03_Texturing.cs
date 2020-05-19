using Diligent.Graphics;
using System;
using System.Numerics;
using System.Reflection;
using System.Runtime.InteropServices;
using Vrmac;
using Vrmac.Animation;

namespace RenderSamples
{
	class Tutorial03_Texturing: SampleBase, iDeltaTimeUpdate
	{
		IPipelineState pipelineState;
		IShaderResourceBinding resourceBinding;
		ITextureView textureView;
		IBuffer cubeVertexBuffer, cubeIndexBuffer, vsConstants;
		Matrix4x4 worldViewProjMatrix;

		void createPipelineState( IRenderDevice device, iStorageFolder assets )
		{
			PipelineStateDesc PSODesc = new PipelineStateDesc( false );
			PSODesc.setBufferFormats( context );

			// Primitive topology defines what kind of primitives will be rendered by this pipeline state
			PSODesc.GraphicsPipeline.PrimitiveTopology = PrimitiveTopology.TriangleList;
			// Cull back faces
			PSODesc.GraphicsPipeline.RasterizerDesc.CullMode = CullMode.Back;
			// Enable depth testing
			PSODesc.GraphicsPipeline.DepthStencilDesc.DepthEnable = true;

			iShaderFactory shaderFactory = device.GetShaderFactory();
			// We won't be using the factory object after this, `using` to release the COM interface once finished
			using( iPipelineStateFactory stateFactory = device.CreatePipelineStateFactory() )
			{
				stateFactory.setName( "Cube PSO" );

				// Compile the two shaders
				ShaderSourceInfo sourceInfo = new ShaderSourceInfo( ShaderType.Vertex, ShaderSourceLanguage.Hlsl );
				sourceInfo.combinedTextureSamplers = true;  // This appears to be the requirement of OpenGL backend.

				// In this tutorial, we will load shaders from resources embedded into this .NET DLL.
				var vs = shaderFactory.compileFromFile( assets, "cube.vsh", sourceInfo, "Cube VS" );
				stateFactory.graphicsVertexShader( vs );

				// Create dynamic uniform buffer that will store our transformation matrix. Dynamic buffers can be frequently updated by the CPU.
				BufferDesc CBDesc = new BufferDesc( false );
				CBDesc.uiSizeInBytes = Marshal.SizeOf<Matrix4x4>();
				CBDesc.Usage = Usage.Dynamic;
				CBDesc.BindFlags = BindFlags.UniformBuffer;
				CBDesc.CPUAccessFlags = CpuAccessFlags.Write;
				vsConstants = device.CreateBuffer( CBDesc, "VS constants CB" );

				// Create a pixel shader
				sourceInfo.shaderType = ShaderType.Pixel;

				var ps = shaderFactory.compileFromFile( assets, "cube.psh", sourceInfo, "Cube PS" );
				stateFactory.graphicsPixelShader( ps );

				// Define vertex shader input layout

				// Attribute 0 - vertex position
				LayoutElement elt = new LayoutElement( false )
				{
					InputIndex = 0,
					BufferSlot = 0,
					NumComponents = 3,
					ValueType = GpuValueType.Float32,
					IsNormalized = false
				};
				stateFactory.graphicsLayoutElement( elt );
				// Attribute 1 - texture coordinates
				elt.InputIndex = 1;
				elt.NumComponents = 2;
				stateFactory.graphicsLayoutElement( elt );

				// Define variable type that will be used by default
				PSODesc.ResourceLayout.DefaultVariableType = ShaderResourceVariableType.Static;
				// Shader variables should typically be mutable, which means they are expected to change on a per-instance basis
				stateFactory.layoutVariable( ShaderType.Pixel, ShaderResourceVariableType.Mutable, "g_Texture" );

				// Define static sampler for g_Texture. Static samplers should be used whenever possible.
				// The default constructor is good enough, it sets FilterType.Linear and TextureAddressMode.Clamp for all 3 coordinates.
				SamplerDesc samplerDesc = new SamplerDesc( false );
				stateFactory.layoutStaticSampler( ShaderType.Pixel, ref samplerDesc, "g_Texture" );

				stateFactory.apply( ref PSODesc );
				pipelineState = device.CreatePipelineState( ref PSODesc );
			}

			// Since we did not explicitly specify the type for 'Constants' variable, default
			// type (SHADER_RESOURCE_VARIABLE_TYPE_STATIC) will be used. Static variables never
			// change and are bound directly through the pipeline state object.
			pipelineState.GetStaticVariableByName( ShaderType.Vertex, "Constants" ).Set( vsConstants );

			// Since we are using mutable variable, we must create a shader resource binding object
			// http://diligentgraphics.com/2016/03/23/resource-binding-model-in-diligent-engine-2-0/
			resourceBinding = pipelineState.CreateShaderResourceBinding( true );
		}

		[StructLayout( LayoutKind.Sequential, Pack = 4 )]
		struct Vertex
		{
			public Vector3 pos;
			public Vector2 texCoords;
		}

		static Vector3 float3( float x, float y, float z )
		{
			return new Vector3( x, y, z );
		}
		static Vector2 float2( float x, float y)
		{
			return new Vector2( x, y );
		}
		static Vertex vert( Vector3 xyz, Vector2 uv )
		{
			return new Vertex()
			{
				pos = xyz,
				texCoords = uv
			};
		}

		void createVertexBuffer( IRenderDevice device )
		{
			// Cube vertices

			//      (-1,+1,+1)________________(+1,+1,+1)
			//               /|              /|
			//              / |             / |
			//             /  |            /  |
			//            /   |           /   |
			//(-1,-1,+1) /____|__________/(+1,-1,+1)
			//           |    |__________|____|
			//           |   /(-1,+1,-1) |    /(+1,+1,-1)
			//           |  /            |   /
			//           | /             |  /
			//           |/              | /
			//           /_______________|/
			//        (-1,-1,-1)       (+1,-1,-1)
			//

			Vertex[] CubeVerts = new Vertex[]
			{
				vert( float3(-1,-1,-1), float2(0,1) ),
				vert( float3(-1,+1,-1), float2(0,0) ),
				vert( float3(+1,+1,-1), float2(1,0) ),
				vert( float3(+1,-1,-1), float2(1,1) ),

				vert( float3(-1,-1,-1), float2(0,1) ),
				vert( float3(-1,-1,+1), float2(0,0) ),
				vert( float3(+1,-1,+1), float2(1,0) ),
				vert( float3(+1,-1,-1), float2(1,1) ),

				vert( float3(+1,-1,-1), float2(0,1) ),
				vert( float3(+1,-1,+1), float2(1,1) ),
				vert( float3(+1,+1,+1), float2(1,0) ),
				vert( float3(+1,+1,-1), float2(0,0) ),

				vert( float3(+1,+1,-1), float2(0,1) ),
				vert( float3(+1,+1,+1), float2(0,0) ),
				vert( float3(-1,+1,+1), float2(1,0) ),
				vert( float3(-1,+1,-1), float2(1,1) ),

				vert( float3(-1,+1,-1), float2(1,0) ),
				vert( float3(-1,+1,+1), float2(0,0) ),
				vert( float3(-1,-1,+1), float2(0,1) ),
				vert( float3(-1,-1,-1), float2(1,1) ),

				vert( float3(-1,-1,+1), float2(1,1) ),
				vert( float3(+1,-1,+1), float2(0,1) ),
				vert( float3(+1,+1,+1), float2(0,0) ),
				vert( float3(-1,+1,+1), float2(1,0) ),
			};
			BufferDesc VertBuffDesc = new BufferDesc( false )
			{
				Usage = Usage.Static,
				BindFlags = BindFlags.VertexBuffer,
			};
			cubeVertexBuffer = device.CreateBuffer( VertBuffDesc, CubeVerts, "Cube vertex buffer" );
		}

		void loadTexture( IRenderDevice device, iStorageFolder assets )
		{
			// Decode the image from embedded resource of this DLL.
			assets.openRead( "net-core-logo.png", out var png );
			if( null == png )
				throw new ApplicationException( "The texture wasn't found" );
			TextureLoadInfo loadInfo = new TextureLoadInfo( false )
			{
				IsSRGB = true
			};
			var texture = device.LoadTexture( png, eImageFileFormat.PNG, ref loadInfo, null );
			// Get shader resource view from the texture
			textureView = texture.GetDefaultView( TextureViewType.ShaderResource );
			// Update the ShaderResourceBinding setting the shader resource view of the newly loaded texture
			resourceBinding.GetVariableByName( ShaderType.Pixel, "g_Texture" ).Set( textureView );
		}

		void createIndexBuffer( IRenderDevice device )
		{
			ushort[] Indices = new ushort[]
			{
				2,0,1,    2,3,0,
				4,6,5,    4,7,6,
				8,10,9,   8,11,10,
				12,14,13, 12,15,14,
				16,18,17, 16,19,18,
				20,21,22, 20,22,23
			};

			BufferDesc IndBuffDesc = new BufferDesc( false )
			{
				Usage = Usage.Static,
				BindFlags = BindFlags.IndexBuffer
			};
			cubeIndexBuffer = device.CreateBuffer( IndBuffDesc, Indices, "Cube index buffer" );
		}

		protected override void createResources( IRenderDevice device )
		{
			iStorageFolder assets = StorageFolder.embeddedResources( Assembly.GetExecutingAssembly(), "RenderSamples/03-Texturing" );
			createPipelineState( device, assets );
			createVertexBuffer( device );
			createIndexBuffer( device );
			loadTexture( device, assets );
			animation.startDelta( this );
		}

		static readonly Vector4 clearColor = Color.parse( "black" );

		void renderScene( IDeviceContext ic, ITextureView swapChainRgb, ITextureView swapChainDepthStencil )
		{
			ic.SetRenderTarget( swapChainRgb, swapChainDepthStencil, ResourceStateTransitionMode.Transition );

			// Clear the back buffer
			ic.ClearRenderTarget( swapChainRgb, clearColor );
			ic.ClearDepthStencil( swapChainDepthStencil, ClearDepthStencilFlags.DepthFlag, 1.0f, 0 );

			// Map the buffer and write current world-view-projection matrix
			Matrix4x4 transposed = worldViewProjMatrix.transposed();
			ic.writeBuffer( vsConstants, ref transposed );

			// Bind vertex and index buffers
			ic.SetVertexBuffer( 0, cubeVertexBuffer, 0 );
			ic.SetIndexBuffer( cubeIndexBuffer, 0 );

			// Set the pipeline state
			ic.SetPipelineState( pipelineState );
			// Commit shader resources. RESOURCE_STATE_TRANSITION_MODE_TRANSITION mode makes sure that resources are transitioned to required states.
			ic.CommitShaderResources( resourceBinding );

			DrawIndexedAttribs draw = new DrawIndexedAttribs( false )
			{
				IndexType = GpuValueType.Uint16,
				NumIndices = 36,
				Flags = DrawFlags.VerifyAll
			};
			ic.DrawIndexed( ref draw );
		}

		static readonly TimeSpan cycleDuration = TimeSpan.FromSeconds( 3 );
		static readonly float velocity = (float)( MathF.PI * 2.0f / cycleDuration.TotalSeconds );

		Angle angle;
		void iDeltaTimeUpdate.tick( float elapsedSeconds )
		{
			angle.rotate( velocity, elapsedSeconds );

			// Set cube world view matrix
			Matrix4x4 CubeWorldView = Matrix4x4.CreateRotationY( angle )
				* Matrix4x4.CreateRotationX( MathF.PI * -0.1f )
				* Matrix4x4.CreateTranslation( 0, 0, 5 );

			float NearPlane = 0.1f;
			float FarPlane = 100;
			// Projection matrix differs between DX and OpenGL
			Matrix4x4 Proj = MathUtils.createPerspectiveFieldOfView( 0.25f * MathF.PI, context.aspectRatio, NearPlane, FarPlane, isOpenGlDevice );
			worldViewProjMatrix = CubeWorldView * Proj;
		}

		protected override void render( ITextureView swapChainRgb, ITextureView swapChainDepthStencil )
		{
			printFps();
			renderScene( context.context, swapChainRgb, swapChainDepthStencil );
		}
	}
}