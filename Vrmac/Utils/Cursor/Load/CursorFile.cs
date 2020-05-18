using Diligent.Graphics;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace Vrmac.Utils.Cursor.Load
{
	/// <summary>Utility class to read cursor files.</summary>
	public class CursorFile: IDisposable
	{
		// https://en.wikipedia.org/wiki/ICO_(file_format)
		// https://devblogs.microsoft.com/oldnewthing/20101018-00/?p=12513

		/// <summary>Close the stream, unless leaveOpen was passed to constructor</summary>
		public void Dispose()
		{
			if( !leaveOpen )
				stream?.Dispose();
		}

		/// <summary>Read file header and directory</summary>
		public CursorFile( Stream stream, bool leaveOpen = false )
		{
			Debug.Assert( 6 == Marshal.SizeOf<ICONDIR>() );

			ICONDIR header = new ICONDIR();
			stream.Read( MiscUtils.asSpan( ref header ) );

			if( header.idReserved != 0 )
				throw new ArgumentException( "The stream is not a valid cursor file" );

			if( header.idType != eImageType.Cursor )
			{
				if( header.idType == eImageType.Icon )
					throw new ArgumentException( "The stream contains an icon, not a cursor" );
				throw new ArgumentException( "The stream is not a valid cursor file" );
			}
			if( header.idCount <= 0 )
				throw new ArgumentException( "The stream doesn't have any images" );

			Debug.Assert( 16 == Marshal.SizeOf<ICONDIRECTORY>() );
			m_images = new ICONDIRECTORY[ header.idCount ];
			stream.read( m_images );

			this.stream = stream;
			this.leaveOpen = leaveOpen;
			if( !stream.CanSeek )
				streamPosition = Marshal.SizeOf<ICONDIR>() + Marshal.SizeOf<ICONDIRECTORY>() * m_images.Length;
		}

		readonly Stream stream;
		readonly bool leaveOpen;
		readonly ICONDIRECTORY[] m_images;
		int streamPosition;

		/// <summary>Count of images in the file</summary>
		public int countImages => m_images.Length;

		/// <summary>Describes the image</summary>
		public struct ImageInfo
		{
			/// <summary>Size of the image</summary>
			public readonly CSize size;

			/// <summary>Position of the pointer within the image</summary>
			public readonly CPoint hotspot;
			/// <summary></summary>
			public readonly byte colorsCount;

			internal ImageInfo( ICONDIRECTORY idi )
			{
				size = idi.size;
				hotspot = idi.hotspot;
				colorsCount = idi.colorsCount;
			}
		}

		/// <summary>Get image directory entry by index</summary>
		public ImageInfo this[ int i ] => new ImageInfo( m_images[ i ] );

		/// <summary>Get all directory entries</summary>
		public ImageInfo[] images => m_images.Select( i => new ImageInfo( i ) ).ToArray();

		/// <summary>Read a single image into a byte array.</summary>
		byte[] read( int i )
		{
			ICONDIRECTORY dir = m_images[ i ];
			if( stream.CanSeek )
				stream.Seek( dir.imageOffset, SeekOrigin.Begin );
			else
			{
				if( streamPosition > dir.imageOffset )
					throw new ArgumentException( "The stream is forward-only, can't rewind it" );
				int skipBytes = dir.imageOffset - streamPosition;
				stream.skip( skipBytes );
				streamPosition += skipBytes;
			}

			int cb = dir.sizeInBytes;
			byte[] result = new byte[ cb ];
			if( cb != stream.Read( result, 0, cb ) )
				throw new EndOfStreamException();
			if( !stream.CanSeek )
				streamPosition += cb;
			return result;
		}

		/// <summary>Decode a cursor image, and upload it to VRAM</summary>
		public CursorTexture load( IRenderDevice renderDevice, int index )
		{
			byte[] data = read( index );
			return LoadCursor.load( renderDevice, data, new ImageInfo( m_images[ index ] ) );
		}
	}
}