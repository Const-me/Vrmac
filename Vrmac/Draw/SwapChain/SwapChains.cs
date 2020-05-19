using Diligent.Graphics;
using System;
using System.Numerics;

namespace Vrmac.Draw.SwapChain
{
	static class SwapChains
	{
		static byte testSamplesCounts( Context context, TextureFormat colorCormat )
		{
			// On Raspberry Pi4 4x MSAA appears to work quite well, yet the support is not detected by Diligent Engine.
			// Testing manually.
			byte[] levels = new byte[] { 8, 4, 2 };
			CSize size = new CSize( 32, 32 );
			foreach( byte lvl in levels )
			{
				try
				{
					using( var rt = new RenderTarget( context, size, colorCormat, lvl, $"MSAA { lvl }x test" ) )
						return lvl;
				}
				catch( Exception ) { }
			}
			return 1;
		}

		static byte pickSampleCount( int SampleCounts )
		{
			if( 0 != ( SampleCounts & 0x10 ) )
				return 16;
			if( 0 != ( SampleCounts & 8 ) )
				return 8;
			if( 0 != ( SampleCounts & 4 ) )
				return 4;
			if( 0 != ( SampleCounts & 2 ) )
				return 2;
			return 0;
		}

		public static iSwapChain create( Context context )
		{
			// Not using MSAA chains anymore, performance on Pi4 is too slow.
			return new ImmediateSwapChain( context );
			/* 
			byte samples = context.swapChainFormats.sampleCount;
			if( samples > 1 )
			{
				ConsoleLogger.logDebug( "The 3D swap chain already uses MSAA, drawing directly to 3D swap chain" );
				return new ImmediateSwapChain( context );
			}

			TextureFormat colorFormat = context.swapChainFormats.color;
			byte level;
			if( RuntimeEnvironment.operatingSystem == eOperatingSystem.Linux )
				level = testSamplesCounts( context, colorFormat );
			else
			{
				TextureFormatInfoExt info = context.renderContext.device.GetTextureFormatInfoExt( colorFormat );
				level = pickSampleCount( info.SampleCounts );
			}

			if( level < 2 )
			{
				// MSAA ain't supported
				ConsoleLogger.logDebug( "MSAA is not supported for {0}, drawing directly to 3D swap chain", colorFormat );
				return new ImmediateSwapChain( context );
			}

			int buffersCount = context.swapChainBuffersCount;
			iSwapChain result;
			if( buffersCount > 1 )
			{
				result = new MsaaSwapChain( context, level, buffersCount );
				ConsoleLogger.logDebug( "Created MSAA swap chain for 2D rendering with {0} samples and {1} buffers in the chain", level, buffersCount );
			}
			else
			{
				result = new SingleTarget( context, level );
				ConsoleLogger.logDebug( "Created MSAA target for 2D rendering with {0} samples", level );
			}
			return result; */
		}

		public static bool isNotTransparent( this ref Vector4 color )
		{
			return color.W >= ( 1.0f / 255.0f );
		}

		public static bool isOpaque( this ref Vector4 color )
		{
			return color.W >= ( 254.0f / 255.0f );
		}
	}
}