using Diligent.Graphics;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Vrmac.Utils.Cursor.Load
{
	static class Monochrome
	{
		public static ITextureView load( IRenderDevice device, byte[] payload, CSize size )
		{
			byte[] data = decodeMonochromeIcon( payload, size );

			TextureDesc desc = new TextureDesc( false );
			desc.Type = ResourceDimension.Tex2d;
			desc.Size = size;
			desc.Format = TextureFormat.R8Unorm;
			desc.Usage = Usage.Static;
			desc.BindFlags = BindFlags.ShaderResource;
			ITexture texture = device.CreateTexture( ref desc, data, size.cx, "Monochrome cursor" );

			return texture.GetDefaultView( TextureViewType.ShaderResource );
		}

		enum ePalette: byte
		{
			FirstBlack,
			FirstWhite
		}

		static ePalette getPalette( ReadOnlySpan<uint> colorTable )
		{
			uint rgbMask = 0xFFFFFF;

			uint c0 = colorTable[ 0 ] & rgbMask;
			uint c1 = colorTable[ 1 ] & rgbMask;
			if( c0 == 0 && c1 == rgbMask )
				return ePalette.FirstBlack;
			if( c0 == rgbMask && c1 == 0 )
				return ePalette.FirstWhite;

			throw new ArgumentException( "Monochrome cursors must only use black and white colors" );
		}

		static void decodePalette( ePalette pal, Span<byte> values )
		{
			Debug.Assert( values.Length == 4 );
			// https://devblogs.microsoft.com/oldnewthing/20101018-00/?p=12513
			switch( pal )
			{
				case ePalette.FirstBlack:
					values[ 0 ] = 0;        // mask 0 image 0 = black
					values[ 1 ] = 0x55;     // mask 0 image 1 = white
					values[ 2 ] = 0xAA;     // mask 1 image 0 = nop
					values[ 3 ] = 0xFF;     // mask 1 image 1 = invert
					break;
				case ePalette.FirstWhite:
					values[ 0 ] = 0x55;     // mask 0 image 1 = white
					values[ 1 ] = 0;        // mask 0 image 0 = black
					values[ 2 ] = 0xFF;     // mask 1 image 1 = invert
					values[ 3 ] = 0xAA;     // mask 1 image 0 = nop
					break;
			}
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		static int interleaveBits( byte low, byte high )
		{
			// http://graphics.stanford.edu/~seander/bithacks.html#Interleave64bitOps
			ulong x = low;
			ulong y = high;
			unchecked
			{
				int wtf1 = (int)( ( ( x * 0x0101010101010101UL & 0x8040201008040201UL ) * 0x0102040810204081UL >> 49 ) & 0x5555 );
				int wtf2 = (int)( ( ( y * 0x0101010101010101UL & 0x8040201008040201UL ) * 0x0102040810204081UL >> 48 ) & 0xAAAA );
				return wtf1 | wtf2;
			}
		}

		static void decodeMonochrome( Span<byte> image, ReadOnlySpan<byte> bits, ReadOnlySpan<byte> mask, ReadOnlySpan<byte> palette, int widthPixels )
		{
			if( 0 != ( widthPixels % 8 ) )
				throw new NotImplementedException( "The library only supports cursors with width being a multiple of 8px" );

			int source = 0;
			for( int i = 0; i < widthPixels; i += 8, source++ )
			{
				int interleaved = interleaveBits( bits[ source ], mask[ source ] );
				image[ i ] = palette[ ( interleaved >> 14 ) & 3 ];
				image[ i + 1 ] = palette[ ( interleaved >> 12 ) & 3 ];
				image[ i + 2 ] = palette[ ( interleaved >> 10 ) & 3 ];
				image[ i + 3 ] = palette[ ( interleaved >> 8 ) & 3 ];
				image[ i + 4 ] = palette[ ( interleaved >> 6 ) & 3 ];
				image[ i + 5 ] = palette[ ( interleaved >> 4 ) & 3 ];
				image[ i + 6 ] = palette[ ( interleaved >> 2 ) & 3 ];
				image[ i + 7 ] = palette[ interleaved & 3 ];
			}
		}

		static byte[] decodeMonochromeIcon( byte[] payload, CSize size )
		{
			int totalPixels = size.cx * size.cy;

			int maskLineDwords = ( size.cx + 31 ) / 32;
			int maskStrideBytes = maskLineDwords * 4;
			int bytesMask = size.cy * maskStrideBytes;

			int cbHeader = Marshal.SizeOf<BITMAPINFOHEADER>();
			// 8 is the color table, immediately after the header
			if( payload.Length != cbHeader + ( bytesMask * 2 ) + 8 )
				throw new ArgumentException( "Size doesn't match" );

			byte[] image = new byte[ totalPixels ];
			Span<byte> palette = stackalloc byte[ 4 ];
			{
				Span<uint> colorTable = stackalloc uint[ 2 ];
				payload.AsSpan()
					.Slice( cbHeader, 8 )
					.castSpan<uint>()
					.CopyTo( colorTable );
				ePalette pal = getPalette( colorTable );
				decodePalette( pal, palette );
			}

			ReadOnlySpan<byte> bits = payload.AsSpan()
				.Slice( cbHeader + 8, bytesMask );

			ReadOnlySpan<byte> mask = payload.AsSpan()
				.Slice( cbHeader + 8 + bytesMask, bytesMask );

			if( 0 == ( size.cx % 32 ) )
			{
				// No padding whatsoever
				decodeMonochrome( image.AsSpan(), bits, mask, palette, totalPixels );
			}
			else
			{
				// There is some padding
				Span<byte> imageSpan = image.AsSpan();
				int maskBytesPerLine = ( size.cx + 7 ) / 8;
				int destOffset = 0;
				int sourceOffset = 0;
				for( int y = 0; y < size.cy; y++, destOffset += size.cx, sourceOffset += maskStrideBytes )
				{
					Span<byte> destSpan = imageSpan.Slice( destOffset, size.cx );
					ReadOnlySpan<byte> lineBits = bits.Slice( sourceOffset, maskBytesPerLine );
					ReadOnlySpan<byte> lineMask = mask.Slice( sourceOffset, maskBytesPerLine );
					decodeMonochrome( destSpan, lineBits, lineMask, palette, size.cx );
				}
			}
			return image;
		}
	}
}
