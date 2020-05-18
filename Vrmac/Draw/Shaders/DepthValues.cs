using Diligent.Graphics;
using System;

namespace Vrmac.Draw.Shaders
{
	/// <summary>Computes depth values from index of the draw command</summary>
	/// <remarks>It’s counter-intuitive, but using Z for 2D rendering improved performance tremendously.
	/// For complex 2D scenes, early Z rejection drops vast majority of pixels, before pixel shader is invoked.</remarks>
	interface iDepthValues
	{
		float value( int index );
	}

	static class DepthValues
	{
		// In GL, the depth output by shaders must be in [ -1 .. +1 ] interval.
		sealed class GL16: iDepthValues
		{
			const float mul = 2.0f / 0xFFFF;
			float iDepthValues.value( int index )
			{
				return 1.0f - ( index + 2 ) * mul;
			}
		}

		sealed class GL24: iDepthValues
		{
			const double mul = 2.0 / 0xFFFFFF;
			float iDepthValues.value( int index )
			{
				// Too bad .NET doesn't have FMA CPU instructions exposed, using doubles instead for the precision
				// Have no idea why "+2", but without the +2 the very first 2D layer is depth-clipped out on Linux.
				return (float)( 1.0 - ( index + 2 ) * mul );
			}
		}

		sealed class DX32: iDepthValues
		{
			// The magic number is 1.0f: https://www.h-schmidt.net/FloatConverter/IEEE754.html
			// Abusing the fact the sort order of floats is the same as order of integers.
			// And also we assume GPU vendors don't cheat about the resolution of that buffer.
			const int first = 0x3f800000 - 1;
			float iDepthValues.value( int index )
			{
				return BitConverter.Int32BitsToSingle( first - index );
			}
		}

		public static iDepthValues create( TextureFormat depthFormat )
		{
			if( RuntimeEnvironment.runningLinux )
			{
				switch( depthFormat )
				{
					case TextureFormat.D16Unorm:
						return new GL16();
					case TextureFormat.D24UnormS8Uint:
						return new GL24();
					// At the time of writing, Linux on Pi4 doesn't support any better.
				}
			}
			else
			{
				switch( depthFormat )
				{
					case TextureFormat.D32Float:
					case TextureFormat.D32FloatS8x24Uint:
						return new DX32();
				}
				// On Windows, D32_FLOAT support is required since D3D 10.0 was introduced in Vista, i.e. since 2006:
				// https://docs.microsoft.com/en-us/windows/win32/direct3ddxgi/format-support-for-direct3d-feature-level-10-0-hardware
				// Look for "Depth/Stencil Target" column.
				// No point in using anything else.
			}
			throw new NotImplementedException();
		}
	}
}