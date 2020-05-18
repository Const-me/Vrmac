using Diligent.Graphics;
using System;
using System.IO;

namespace Vrmac.Utils
{
	static class ScreenGrabber
	{
		public delegate void ConsumeTextureDelegate( TextureFormat format, CSize size, MappedTextureSubresource mapped );

		sealed class ScreenGrabberResources
		{
			TextureDesc stagingDesc;
			IFence fence;
			uint nextFenceValue = 0;
			ITexture stagingTexture;

			void waitForGpu( IDeviceContext context )
			{
				if( !RuntimeEnvironment.runningWindows )
					return;

				nextFenceValue++;
				context.SignalFence( fence, nextFenceValue );
				context.WaitForFence( fence, nextFenceValue, true );
			}

			public void grab( IRenderDevice device, IDeviceContext context, ITexture texture, ConsumeTextureDelegate consume )
			{
				TextureDesc desc = texture.GetDesc();
				// Create staging texture if needed
				if( null == stagingTexture || stagingDesc.Size != desc.Size || stagingDesc.Format != desc.Format )
				{
					ComUtils.clear( ref stagingTexture );
					desc.Type = ResourceDimension.Tex2d;
					desc.BindFlags = BindFlags.None;
					desc.CPUAccessFlags = CpuAccessFlags.Read;
					desc.Usage = Usage.Staging;
					stagingTexture = device.CreateTexture( ref desc, "ScreenGrabber staging" );
					stagingDesc = desc;
				}

				// In D3D12, Diligent engine fails instead of waiting for GPU. Need to wait manually, need a fence for that.
				if( RuntimeEnvironment.runningWindows && null == fence )
				{
					var fd = new FenceDesc( false );
					fence = device.CreateFence( ref fd );
				}

				// Finish the rendering, if any
				waitForGpu( context );

				// Unset the targets
				context.SetRenderTargets( 0, null, null );
				waitForGpu( context );

				// Copy source texture to staging
				context.copyTexture( stagingTexture, texture );
				waitForGpu( context );

				// Map the texture
				Box box = new Box( false )
				{
					MaxX = desc.Size.cx,
					MaxY = desc.Size.cy
				};
				MappedTextureSubresource mapped = context.MapTextureSubresource( stagingTexture, 0, 0, MapType.Read, MapFlags.DoNotWait, ref box );
				try
				{
					consume( desc.Format, desc.Size, mapped );
				}
				finally
				{
					context.UnmapTextureSubresource( stagingTexture, 0, 0 );
				}
			}
		}

		static readonly ScreenGrabberResources grabber = new ScreenGrabberResources();

		/// <summary>Grab texture from VRAM into system memory</summary>
		public static void readTexture( IRenderDevice device, IDeviceContext context, ITexture texture, ConsumeTextureDelegate consume )
		{
			grabber.grab( device, context, texture, consume );
		}

		static void encodeScreenshot( TextureFormat format, CSize size, MappedTextureSubresource mapped, string dest )
		{
			// Pick the right set of PNG options
			ePngOptions options;
			switch( format )
			{
				case TextureFormat.Rgba8Sint:
				case TextureFormat.Rgba8Snorm:
				case TextureFormat.Rgba8Typeless:
				case TextureFormat.Rgba8Unorm:
					options = ePngOptions.None;
					break;
				case TextureFormat.Rgba8UnormSrgb:
					options = ePngOptions.SrgbMetadata;
					break;
				case TextureFormat.Bgra8Typeless:
				case TextureFormat.Bgrx8Typeless:
				case TextureFormat.Bgra8Unorm:
				case TextureFormat.Bgrx8Unorm:
					options = ePngOptions.FlipChannels;
					break;
				case TextureFormat.Bgra8UnormSrgb:
				case TextureFormat.Bgrx8UnormSrgb:
					options = ePngOptions.FlipChannels | ePngOptions.SrgbMetadata;
					break;
				default:
					throw new ArgumentException( $"{ format } textures can’t be PNG encoded. Do something else, e.g. DDS." );
			}

			// No idea why, but identical C# and C++ code produces good images on Windows and vertically flipped ones on Linux.
			// Need to pass a bit to encoder to distinguish.
			// Fortunately, LibPNG takes an array of row pointers and thus doesn't care about rows RAM layout, the implementation doesn't need to make copies.
			if( RuntimeEnvironment.runningLinux )
				options |= ePngOptions.FlipRows;

			// Finally, write the PNG
			using( var fs = File.Create( dest ) )
				GraphicsUtils.encodeRgbaPng( size, mapped, options, fs );
		}

		/// <summary>Grab texture from VRAM, encode into 32-bit PNG</summary>
		public static void saveTexture( IRenderDevice device, IDeviceContext context, ITexture texture, string destinationPath )
		{
			string dir = Path.GetDirectoryName( destinationPath );
			if( !Directory.Exists( dir ) )
				Directory.CreateDirectory( dir );

			readTexture( device, context, texture, ( format, size, mapped ) => encodeScreenshot( format, size, mapped, destinationPath ) );
		}
	}
}