using Diligent.Graphics;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace Vrmac.Utils.Cursor.Load
{
	class AniBitmaps: iTextureArrayData
	{
		static readonly TextureFormat textureFormat = RuntimeEnvironment.operatingSystem == eOperatingSystem.Windows ? TextureFormat.Bgra8Unorm : TextureFormat.Rgba8Unorm;
		TextureFormat iTextureArrayData.format => textureFormat;
		TextureSubResData[] iTextureArrayData.data => frames;

		void IDisposable.Dispose()
		{
			if( pinned.IsAllocated )
				pinned.Free();
		}

		readonly uint[] data;
		readonly GCHandle pinned;
		TextureSubResData[] frames;
		readonly CSize size;
		readonly int pixelsPerFrame;

		public AniBitmaps( Stream stream, AniFile.Frame[] frames, int formatIndex )
		{
			size = frames[ 0 ].images[ formatIndex ].size;
			int countFrames = frames.Length;
			pixelsPerFrame = size.cx * size.cy;
			data = new uint[ pixelsPerFrame * countFrames ];
			Span<uint> destSpan = data.AsSpan();

			int streamOffset = 0;
			int bihSize = Marshal.SizeOf<BITMAPINFOHEADER>();

			for( int i = 0; i < frames.Length; i++ )
			{
				AniFile.Payload src = frames[ i ].payloads[ formatIndex ];
				stream.skip( src.offset - streamOffset );
				streamOffset = src.offset;

				BITMAPINFOHEADER header = stream.read<BITMAPINFOHEADER>();
				streamOffset += bihSize;
				Debug.Assert( bihSize == header.biSize );

				if( header.biWidth != size.cx )
					throw new ArgumentException( "Size in BMP doesn't match" );

				Span<uint> destFrame = destSpan.Slice( i * pixelsPerFrame, pixelsPerFrame );
				if( header.biCompression == BitmapCompressionMode.BI_RGB && header.biHeight == size.cy * 2 )
					decodeRgbIcon( stream, destFrame, ref streamOffset, ref header, src.size );
				else
					throw new NotImplementedException();
			}

			bool flipBgr = RuntimeEnvironment.operatingSystem != eOperatingSystem.Windows;
			GraphicsUtils.premultiplyAlpha( data, flipBgr );

			this.frames = new TextureSubResData[ countFrames ];
			pinned = GCHandle.Alloc( data, GCHandleType.Pinned );
			IntPtr pointer = pinned.AddrOfPinnedObject();
			for( int i = 0; i < countFrames; i++ )
			{
				TextureSubResData srd = new TextureSubResData();
				srd.pData = pointer + ( i * pixelsPerFrame * 4 );
				srd.Stride = size.cx * 4;
				srd.DepthStride = pixelsPerFrame * 4;
				this.frames[ i ] = srd;
			}
		}

		void decodeRgbIcon( Stream stm, Span<uint> dest, ref int streamOffset, ref BITMAPINFOHEADER header, int sourceLengthBytes )
		{
			int bytesRgb = size.cx * size.cy * 4;

			int maskLineDwords = ( size.cx + 31 ) / 32;
			int maskStrideBytes = maskLineDwords * 4;
			int bytesMask = size.cy * maskStrideBytes;

			int cbHeader = Marshal.SizeOf<BITMAPINFOHEADER>();
			if( sourceLengthBytes != bytesRgb + bytesMask + cbHeader )
				throw new ArgumentException( "Size doesn't match" );

			// Read RGB values
			{
				Span<byte> destBytes = MemoryMarshal.Cast<uint, byte>( dest );
				if( destBytes.Length != stm.Read( destBytes ) )
					throw new EndOfStreamException();
				streamOffset += destBytes.Length;
			}

			// Ignore the mask, the data for the embedded animated icons is already RGBA, just need to pre-multiply the alpha.
			/*
			// Read the mask
			Span<byte> mask = stackalloc byte[ bytesMask ];
			if( mask.Length != stm.Read( mask ) )
				throw new EndOfStreamException();
			streamOffset += mask.Length;

			// Apply the mask
			LoadCursor.applyMask( dest, size, mask, maskStrideBytes );
			*/
		}
	}
}