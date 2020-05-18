using Diligent.Graphics;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Vrmac.Utils.Cursor.Load
{
	static class LoadCursor
	{
		public static CursorTexture load( IRenderDevice renderDevice, byte[] payload, CursorFile.ImageInfo info )
		{
			BITMAPINFOHEADER header = new BITMAPINFOHEADER();
			int bihSize = Marshal.SizeOf<BITMAPINFOHEADER>();
			payload.AsSpan().Slice( 0, bihSize ).CopyTo( MiscUtils.asSpan( ref header ) );

			if( bihSize == header.biSize )
			{
				bool monochrome;
				ITextureView texture = loadBmp( renderDevice, payload, ref header, info.size, out monochrome );
				if( monochrome )
					return new MonochromeCursorTexture( texture, info.size, info.hotspot );
				else
					return new StaticCursorTexture( texture, info.size, info.hotspot );
			}

			if( header.biSize == 0x474E5089 )  // "‰PNG" magic number
				return new StaticCursorTexture( loadPng( renderDevice, payload ), info.size, info.hotspot );

			throw new ArgumentException( "LoadCursor.load only supports BMP or PNG images" );
		}

		static ITextureView loadPng( IRenderDevice device, byte[] payload )
		{
			TextureLoadInfo loadInfo = new TextureLoadInfo( false );
			var png = new MemoryStream( payload, false );
			var texture = device.LoadTexture( png, eImageFileFormat.PNG, ref loadInfo, "Mouse cursor" );
			return texture.GetDefaultView( TextureViewType.ShaderResource );
		}

		static ITextureView loadBmp( IRenderDevice device, byte[] payload, ref BITMAPINFOHEADER header, CSize size, out bool monochrome )
		{
			if( header.biWidth != size.cx )
				throw new ArgumentException( "Size in BMP doesn't match" );
			uint[] rgba;
			if( header.biCompression == BitmapCompressionMode.BI_RGB && header.biHeight == size.cy * 2 )
			{
				if( header.biBitCount == 32 )
				{
					monochrome = false;
					rgba = decodeRgbIcon( payload, size );
				}
				else if( header.biBitCount == 1 )
				{
					monochrome = true;
					return Monochrome.load( device, payload, size );
				}
				else
					throw new NotImplementedException();
			}
			else
				throw new NotImplementedException();

			TextureDesc desc = new TextureDesc( false );
			desc.Type = ResourceDimension.Tex2d;
			desc.Size = size;
			if( RuntimeEnvironment.operatingSystem == eOperatingSystem.Windows )
				desc.Format = TextureFormat.Bgra8Unorm;
			else
			{
				desc.Format = TextureFormat.Rgba8Unorm;
				GraphicsUtils.swapRedBlueChannels( rgba );
			}
			desc.Usage = Usage.Static;
			desc.BindFlags = BindFlags.ShaderResource;

			ITexture texture = device.CreateTexture( ref desc, rgba, size.cx * 4, "Mouse cursor" );
			return texture.GetDefaultView( TextureViewType.ShaderResource );
		}

		// Very inefficient BTW, any SIMD would improve performance by an order of magnitude.
		// Mouse cursors are really small though, we don't really care about the performance of this code.
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		static void applyMask( Span<uint> pixels, int destOffset, int value )
		{
			// Technically, the formula is `pixel = (screen AND mask) XOR image`
			// Practically, we interpret the mask as inverted 1 bit alpha. The proper AND/XOR requires logical operations on render target, these are not available in GLES.
			// Monochrome cursors do support the feature. They render the quad twice, with weird alpha blending setup on the first pass to invert the colors.

			if( value == 0xFF )    // Completely transparent
			{
				pixels.Slice( destOffset, 8 ).Fill( 0 );
				return;
			}
			if( value == 0 )    // Completely opaque
			{
				pixels[ destOffset ] |= 0xFF000000;
				pixels[ destOffset + 1 ] |= 0xFF000000;
				pixels[ destOffset + 2 ] |= 0xFF000000;
				pixels[ destOffset + 3 ] |= 0xFF000000;
				pixels[ destOffset + 4 ] |= 0xFF000000;
				pixels[ destOffset + 5 ] |= 0xFF000000;
				pixels[ destOffset + 6 ] |= 0xFF000000;
				pixels[ destOffset + 7 ] |= 0xFF000000;
				return;
			}

			// Deal with individual pixels. For some weird reason, the order of bits is from high to low, i.e. x=0 corresponds to the highest bit, with the value 0x80.
			// Not unrolling this loop because branch predictor helps a lot with these conditions: cursors aren't noise, sequential pixels are likely to have the same mask value.
			for( int i = 0; i < 8; i++, value = value << 1 )
			{
				if( 0 == ( value & 0x80 ) )
					pixels[ destOffset + i ] |= 0xFF000000;
				else
					pixels[ destOffset + i ] = 0;
			}
		}

		internal static void applyMask( Span<uint> pixels, CSize size, ReadOnlySpan<byte> mask, int maskStrideBytes )
		{
			if( 0 == ( size.cx % 8 ) )
			{
				if( 0 == ( size.cx % 32 ) )
				{
					// No padding whatsoever
					int destOffset = 0;
					for( int i = 0; i < mask.Length; i++, destOffset += 8 )
						applyMask( pixels, destOffset, mask[ i ] );
				}
				else
				{
					// There is padding, but it's whole count of bytes
					int maskBytesPerLine = size.cx / 8;
					int sourceOffset = 0;
					int destOffset = 0;
					for( int y = 0; y < size.cy; y++, sourceOffset += maskStrideBytes )
					{
						for( int x = 0; x < maskBytesPerLine; x++, destOffset += 8 )
							applyMask( pixels, destOffset, mask[ sourceOffset + x ] );
					}
				}
			}
			else
			{
				// There is padding, and the last few pixels have less than 8 payload bits per line.
				throw new NotImplementedException( "The library only supports cursors with width being a multiple of 8px" );
			}
		}

		static uint[] decodeRgbIcon( byte[] payload, CSize size )
		{
			// https://devblogs.microsoft.com/oldnewthing/20101019-00/?p=12503
			int totalPixels = size.cx * size.cy;

			int maskLineDwords = ( size.cx + 31 ) / 32;
			int maskStrideBytes = maskLineDwords * 4;
			int bytesMask = size.cy * maskStrideBytes;

			int cbHeader = Marshal.SizeOf<BITMAPINFOHEADER>();
			if( payload.Length != ( totalPixels * 4 ) + bytesMask + cbHeader )
				throw new ArgumentException( "Size doesn't match" );

			uint[] result = new uint[ totalPixels ];

			// Copy the RGB values
			payload.AsSpan()
				.Slice( cbHeader, totalPixels * 4 )
				.castSpan<uint>()
				.CopyTo( result.AsSpan() );

			// Apply the alpha mask, it's 1 bit / pixel.
			ReadOnlySpan<byte> mask = payload.AsSpan().Slice( cbHeader + totalPixels * 4 );
			applyMask( result.AsSpan(), size, mask, maskStrideBytes );
			return result;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		static void decodeMonochrome( Span<uint> pixels, ReadOnlySpan<uint> pal, int destOffset, int value, int bitsCount = 8 )
		{
			if( value == 0xFF )
			{
				pixels.Slice( destOffset, bitsCount ).Fill( pal[ 1 ] );
				return;
			}
			if( value == 0 )
			{
				pixels.Slice( destOffset, bitsCount ).Fill( pal[ 0 ] );
				return;
			}
			// Deal with individual pixels. For some weird reason, the order of bits is from high to low, i.e. x=0 corresponds to the highest bit, with the value 0x80.
			for( int i = 0; i < bitsCount; i++, value = value << 1 )
			{
				if( 0 == ( value & 0x80 ) )
					pixels[ destOffset + i ] |= pal[ 0 ];
				else
					pixels[ destOffset + i ] = pal[ 1 ];
			}
		}

		static uint[] decodeMonochromeIcon( byte[] payload, CSize size )
		{
			int totalPixels = size.cx * size.cy;

			int maskLineDwords = ( size.cx + 31 ) / 32;
			int maskStrideBytes = maskLineDwords * 4;
			int bytesMask = size.cy * maskStrideBytes;

			int cbHeader = Marshal.SizeOf<BITMAPINFOHEADER>();
			// 8 is the color table, immediately after the header
			if( payload.Length != cbHeader + ( bytesMask * 2 ) + 8 )
				throw new ArgumentException( "Size doesn't match" );

			uint[] result = new uint[ totalPixels ];
			Span<uint> colorTable = stackalloc uint[ 2 ];
			payload.AsSpan()
				.Slice( cbHeader, 8 )
				.castSpan<uint>()
				.CopyTo( colorTable );

			ReadOnlySpan<byte> bitmap = payload.AsSpan()
				.Slice( cbHeader + 8, bytesMask );

			if( 0 == ( size.cx % 8 ) )
			{
				if( 0 == ( size.cx % 32 ) )
				{
					// No padding whatsoever
					int destOffset = 0;
					for( int i = 0; i < bitmap.Length; i++, destOffset += 8 )
						decodeMonochrome( result, colorTable, destOffset, bitmap[ i ] );
				}
				else
				{
					// There is padding, but it's whole count of bytes
					int maskBytesPerLine = size.cx / 8;
					int sourceOffset = 0;
					int destOffset = 0;
					for( int y = 0; y < size.cy; y++, sourceOffset += maskStrideBytes )
					{
						for( int x = 0; x < maskBytesPerLine; x++, destOffset += 8 )
							decodeMonochrome( result, colorTable, destOffset, bitmap[ sourceOffset + x ] );
					}
				}
			}
			else
			{
				// There is padding, and the last few pixels have less than 8 payload bits per line.
				throw new NotImplementedException( "The library only supports cursors with width being a multiple of 8px" );
			}

			// Apply the alpha mask, it's 1 bit / pixel.
			ReadOnlySpan<byte> mask = payload.AsSpan().Slice( cbHeader + 8 + bytesMask, bytesMask );
			applyMask( result.AsSpan(), size, mask, maskStrideBytes );
			return result;
		}
	}
}