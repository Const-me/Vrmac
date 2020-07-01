using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Vrmac
{
	/// <summary>Utility class to save TGA images</summary>
	/// <seealso href="https://en.wikipedia.org/wiki/Truevision_TGA" />
	public static class TrueVision
	{
		enum eColorMapType: byte
		{
			None = 0,
			Present = 1,
		};

		enum eImageType: byte
		{
			None = 0,
			Palette = 1,
			RGB = 2,
			Grayscale = 3,
		};

		[StructLayout( LayoutKind.Sequential, Pack = 1 )]
		struct TgaHeader
		{
			public byte IDLength;
			public eColorMapType ColorMapType;
			public eImageType ImageType;
			public ushort CMapStart;
			public ushort CMapLength;
			public byte CMapDepth;
			public ushort XOffset, YOffset;
			public ushort Width, Height;
			public byte PixelDepth;
			public byte ImageDescriptor;
		};

		/// <summary>Save 8 bit/pixel grayscale TGA image</summary>
		public static void saveGrayscale( Stream stm, ReadOnlySpan<byte> data, CSize size, int stride )
		{
			if( stride < size.cx || size.cx <= 0 || size.cy <= 0 )
				throw new ArgumentOutOfRangeException();
			if( size.cx >= 0x10000 || size.cy >= 0x10000 )
				throw new ArgumentOutOfRangeException();
			if( data.Length != size.cy * stride )
				throw new ArgumentException();

			TgaHeader header = new TgaHeader()
			{
				ColorMapType = eColorMapType.None,
				ImageType = eImageType.Grayscale,
				// By default, rows order in TGAs is bottom to top. Not what we want. That's why setting bit #5 of the ImageDescriptor header field.
				ImageDescriptor = 0b00100000,
				Width = (ushort)size.cx,
				Height = (ushort)size.cy,
				PixelDepth = 8,
			};

			{
				// The usability is not great, BTW
				var span1 = MemoryMarshal.CreateReadOnlySpan( ref header, 1 );
				var span2 = MemoryMarshal.Cast<TgaHeader, byte>( span1 );
				stm.Write( span2 );
			}

			if( stride == size.cx )
			{
				// No padding whatsoever, write the complete file in 1 shot
				stm.Write( data );
			}
			else
			{
				// Rows have padding, need to skip stuff.
				for( int y = 0; y < size.cy; y++ )
				{
					int off = y * stride;
					stm.Write( data.Slice( off, size.cx ) );
				}
			}
		}
	}
}