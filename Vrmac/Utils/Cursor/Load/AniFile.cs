using Diligent.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace Vrmac.Utils.Cursor.Load
{
	internal interface iTextureArrayData: IDisposable
	{
		TextureSubResData[] data { get; }
		TextureFormat format { get; }
	}

	/// <summary>Animated cursor file</summary>
	public struct AniFile
	{
		const uint PngSignature = 0x474E5089;    // "‰PNG" magic number
		const uint ACON = 0x4E4F4341;
		const uint ANIH = 0x68696E61;
		const uint FRAM = 0x6D617266;
		const uint ICON = 0x6E6F6369;

		internal struct Payload
		{
			public readonly int offset, size;
			/// <summary>For bitmaps <see cref="BITMAPINFOHEADER.biSize" /> = 40, or can be <see cref="PngSignature" /></summary>
			public readonly uint BitmapHeaderSize;

			internal Payload( int offset, int size, uint header )
			{
				this.offset = offset;
				this.size = size;
				BitmapHeaderSize = header;
			}
		}

		internal struct Frame
		{
			public readonly ICONDIRECTORY[] images;
			public readonly Payload[] payloads;

			internal Frame( iRiffChunk chunk )
			{
				var header = chunk.read<ICONDIR>();
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

				// Read the directory
				images = new ICONDIRECTORY[ header.idCount ];
				chunk.read( images );

				// Sort the entries so we don't have to rewind the stream
				Array.Sort( images, ( a, b ) => a.imageOffset.CompareTo( b.imageOffset ) );

				// Construct the payloads, and read initial 4 bytes of each image.
				payloads = new Payload[ header.idCount ];
				for( int i = 0; i < header.idCount; i++ )
				{
					int skipBytes = chunk.chunkOffset + images[ i ].imageOffset - chunk.currentOffset;
					chunk.skip( skipBytes );

					int offset = chunk.currentOffset;
					uint hh = chunk.read<uint>();
					int cb = images[ i ].sizeInBytes;
					payloads[ i ] = new Payload( offset, cb, hh );
				}
			}
		}

		class Parser
		{
			public int parsedFrames;
			public ANIHeader header;
			public Frame[] frames;

			public void parse( iRiffChunk chunk )
			{
				if( chunk.parentTag == ACON && chunk.tag == ANIH )
				{
					header = chunk.read<ANIHeader>();
					if( header.cbSizeOf != Marshal.SizeOf<ANIHeader>() )
						throw new ArgumentException( "ANI header has wrong size" );
					frames = new Frame[ header.cFrames ];
					return;
				}

				if( chunk.parentTag == FRAM && chunk.tag == ICON )
				{
					if( null == frames )
						throw new ArgumentException( "ANI header not found" );
					if( chunk.index >= frames.Length )
						throw new ArgumentException( "Too many frames in the ANI" );
					frames[ chunk.index ] = new Frame( chunk );
					parsedFrames = chunk.index + 1;
					return;
				}
			}
		}

		readonly ANIHeader header;
		readonly Frame[] frames;

		/// <summary>Bitmap format of ANI frames</summary>
		public enum eFormat: byte
		{
			/// <summary>Windows bitmap</summary>
			Bitmap,
			/// <summary>ONG encoded frames. They are not currently implemented.</summary>
			PNG
		}

		/// <summary>Each frame in the cursor might have multiple images. This structure describes the format of a single image.</summary>
		public struct ImageFormat
		{
			/// <summary>Size in pixels</summary>
			public readonly CSize size;
			/// <summary>Bitmap format of the frames</summary>
			public readonly eFormat format;
			internal readonly byte colorsCount;
			internal readonly ushort planes, bitCount;

			internal ImageFormat( ref ICONDIRECTORY icd, uint bmp )
			{
				size = icd.size;
				colorsCount = icd.colorsCount;
				planes = icd.planes;
				bitCount = icd.bitCount;
				switch( bmp )
				{
					case 40:
						format = eFormat.Bitmap;
						break;
					case PngSignature:
						format = eFormat.PNG;
						break;
					default:
						throw new ArgumentException( "The image format is not supported" );
				}
			}

			/// <summary>String representation of this object, useful for debugging</summary>
			public override string ToString()
			{
				return $"{ size }, { colorsCount } colors, { planes } planes, { bitCount } bits";
			}
		}

		/// <summary>Available image formats in the file</summary>
		public readonly ImageFormat[] formats;

		static ulong headerValues( ref ICONDIRECTORY icd )
		{
			var bytes = MiscUtils.asSpan( ref icd ).Slice( 0, 8 );
			return MemoryMarshal.Cast<byte, ulong>( bytes )[ 0 ];
		}

		/// <summary>Parse stream with *.ani file, extract all the metadata.</summary>
		public AniFile( Stream stream )
		{
			Parser p = new Parser();
			RiffParser.parse( stream, p.parse );
			if( null == p.frames || p.parsedFrames != p.frames.Length )
				throw new ArgumentException( "The stream is not an animated cursor file" );

			header = p.header;
			frames = p.frames;
			if( frames.Length <= 0 )
				throw new ArgumentException( "The animated cursor doesn’t have any frames" );

			ICONDIRECTORY[] images = frames[ 0 ].images;
			formats = new ImageFormat[ images.Length ];
			Span<ulong> vals = stackalloc ulong[ images.Length ];
			Span<uint> bmpFormats = stackalloc uint[ images.Length ];
			for( int i = 0; i < images.Length; i++ )
			{
				uint bmp = frames[ 0 ].payloads[ i ].BitmapHeaderSize;
				vals[ i ] = headerValues( ref images[ i ] );
				formats[ i ] = new ImageFormat( ref images[ i ], bmp );
				bmpFormats[ i ] = bmp;
			}

			for( int i = 1; i < frames.Length; i++ )
			{
				images = frames[ i ].images;
				for( int j = 0; j < vals.Length; j++ )
				{
					ulong vv = headerValues( ref images[ j ] );
					uint bmp = frames[ i ].payloads[ j ].BitmapHeaderSize;
					if( vv != vals[ j ] || bmp != bmpFormats[ j ] )
						throw new ArgumentException( "Different frames of the animation use different image format, this is not supported" );
				}
			}
		}

		/// <summary>A summary for debugger</summary>
		public override string ToString()
		{
			double fps = 60.0 / header.JifRate;
			HashSet<CSize> sizesSet = new HashSet<CSize>( frames.SelectMany( f => f.images.Select( i => i.size ) ) );
			string sizes = string.Join( ", ", sizesSet );
			return $"{ header.cFrames } frames, { fps } fps; { sizes }";
		}

		/// <summary>Decode frames of the specified format index, and upload them to VRAM.</summary>
		public AnimatedCursorTexture load( IRenderDevice device, Stream stream, string name, int formatIndex = 0 )
		{
			if( formatIndex < 0 || formatIndex >= formats.Length )
				throw new ArgumentOutOfRangeException();

			iTextureArrayData data;
			switch( formats[ formatIndex ].format )
			{
				case eFormat.Bitmap:
					data = new AniBitmaps( stream, this.frames, formatIndex );
					break;
				default:
					throw new NotImplementedException();
			}
			CSize size = formats[ formatIndex ].size;
			int frames = data.data.Length;

			ITexture textureArray;
			using( data )
			{
				TextureDesc desc = new TextureDesc( false );
				desc.Type = ResourceDimension.Tex2dArray;
				desc.Size = size;
				desc.ArraySizeOrDepth = (uint)frames;
				desc.Format = data.format;
				desc.Usage = Usage.Static;
				desc.BindFlags = BindFlags.ShaderResource;

				textureArray = device.CreateTexture( ref desc, data.data, (uint)frames, name );
			}

			ITextureView view = textureArray.GetDefaultView( TextureViewType.ShaderResource );
			double ticks = TimeSpan.TicksPerSecond;
			ticks /= 60;
			ticks *= header.JifRate;
			ticks = Math.Round( ticks );
			TimeSpan duration = TimeSpan.FromTicks( (long)ticks );
			CPoint hotspot = this.frames[ 0 ].images[ formatIndex ].hotspot;
			return new AnimatedCursorTexture( view, size, hotspot, frames, duration );
		}
	}
}