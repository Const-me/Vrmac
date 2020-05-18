// TODO: comment them
#pragma warning disable CS1591  // CS1591: Missing XML comment for publicly visible type or member
using System.Runtime.InteropServices;

namespace Vrmac.Utils.Cursor.Load
{
	public enum eImageType: ushort
	{
		Icon = 1,
		Cursor = 2
	}

	[StructLayout( LayoutKind.Sequential )]
	public struct ICONDIR
	{
		/// <summary>Reserved. Must always be 0.</summary>
		public ushort idReserved;
		/// <summary>Image type, 1 for icon (.ICO) image, 2 for cursor (.CUR) image. Other values are invalid.</summary>
		public eImageType idType;
		/// <summary>Number of images in the file.</summary>
		public ushort idCount;
	}

	[StructLayout( LayoutKind.Sequential )]
	public struct ICONDIRECTORY
	{
		byte width, height;
		public byte colorsCount, bReserved;
		public ushort planes, bitCount;
		public int sizeInBytes, imageOffset;

		public CSize size => getSize();

		CSize getSize()
		{
			int w = width != 0 ? width : 0x100;
			int h = height != 0 ? height : 0x100;
			return new CSize( w, h );
		}

		/// <summary>Cursors have different meanings of these two fields</summary>
		public CPoint hotspot => new CPoint( planes, bitCount );
	}

	/// <summary>Information about the dimensions and color format of a device-independent bitmap (DIB).</summary>
	/// <seealso href="https://docs.microsoft.com/en-us/windows/win32/api/wingdi/ns-wingdi-bitmapinfoheader" />
	[StructLayout( LayoutKind.Sequential )]
	public struct BITMAPINFOHEADER
	{
		// http://www.pinvoke.net/default.aspx/Structures/BITMAPINFOHEADER.html

		/// <summary>Number of bytes required by the structure. This value does not include the size of the color table or the size of the color masks, if they are appended to the end of structure.</summary>
		public int biSize;
		/// <summary>width of the bitmap, in pixels</summary>
		public int biWidth;
		/// <summary>Height of the bitmap, in pixels</summary>
		public int biHeight;
		/// <summary>Number of planes for the target device. This value must be set to 1.</summary>
		public ushort biPlanes;
		/// <summary>Number of bits per pixel (bpp)</summary>
		public ushort biBitCount;
		/// <summary></summary>
		public BitmapCompressionMode biCompression;
		/// <summary>Size, in bytes, of the image. This can be set to 0 for uncompressed RGB bitmaps</summary>
		public uint biSizeImage;
		/// <summary>Horizontal resolution, in pixels per meter, of the target device for the bitmap.</summary>
		public int biXPelsPerMeter;
		/// <summary>Vertical resolution, in pixels per meter, of the target device for the bitmap.</summary>
		public int biYPelsPerMeter;
		/// <summary>Number of color indices in the color table that are actually used by the bitmap</summary>
		public uint biClrUsed;
		/// <summary>Number of color indices that are considered important for displaying the bitmap. If this value is zero, all colors are important.</summary>
		public uint biClrImportant;
	}

	public enum BitmapCompressionMode: uint
	{
		BI_RGB = 0,
		BI_RLE8 = 1,
		BI_RLE4 = 2,
		BI_BITFIELDS = 3,
		BI_JPEG = 4,
		BI_PNG = 5
	}

	[StructLayout( LayoutKind.Sequential )]
	public struct ANIHeader
	{
		public uint cbSizeOf; // Num bytes in AniHeader (36 bytes)
		public uint cFrames; // Number of unique Icons in this cursor
		public uint cSteps; // Number of Blits before the animation cycles
		public uint cx, cy; // reserved, must be zero.
		public uint cBitCount, cPlanes; // reserved, must be zero.
		public uint JifRate; // Default Jiffies (1/60th of a second) if rate chunk not present.
		public uint flags; // Animation Flag (see AF_ constants)
	}
}