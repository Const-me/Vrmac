using Diligent.Graphics;
using System;
using Vrmac.Draw.Text;

namespace Vrmac.Draw
{
	/// <summary>Which brush is used. Currently there s one.</summary>
	enum eBrush: byte
	{
		SolidColor = 1,
		Sprite = 2,
		// Only checked for text draw calls
		OpaqueColor = 3,
	}

	/// <summary>Where the mesh came from</summary>
	enum eMesh: byte
	{
		Filled = 1,
		Stroked = 2,
		Rectangle = 3,
		SpriteRectangle = 4,
		GlyphRun = 5,
		TransformedText = 6,
	}

	enum eVaaKind: byte
	{
		None = 0,
		Filled = 1,
		StrokedFat = 2,
		StrokedThin = 3,
	}

	enum eClearTypeKind: byte
	{
		// Will use grayscale font texture
		None = 0,
		// Layout of subpixel corresponds to the display
		Straight = 1,
		// Layout of subpixels in the atlas is flipped compared to what we need to render: text was rotated by 180 degrees, or flipped horizontally, or the display has BGR order of subpixels.
		Flipped = 2,
	}

	struct DrawCallType
	{
		public readonly int value;

		DrawCallType( int vv )
		{
			value = vv;
		}

		public DrawCallType( eBrush brush, eMesh mesh, eVaaKind vaa )
		{
			value = MiscUtils.combine( (byte)brush, (byte)mesh, (byte)vaa );
		}
		public DrawCallType( eBrush brush, eMesh mesh, eClearTypeKind clearType )
		{
			value = MiscUtils.combine( (byte)brush, (byte)mesh, (byte)clearType );
		}

		public DrawCallType( eMesh mesh, eVaaKind vaa ) :
			this( eBrush.SolidColor, mesh, vaa )
		{ }

		/// <summary>Reinterpret numerical value as 32-bit float.</summary>
		public float packToFloat =>
			BitConverter.Int32BitsToSingle( value );

		public int packToInt => unchecked((int)value);

		public eBrush brush => (eBrush)(byte)( value & 0xFF );
		public eMesh mesh => (eMesh)(byte)( ( value >> 8 ) & 0xFF );
		public eVaaKind vaa => (eVaaKind)(byte)( ( value >> 16 ) & 0xFF );
		public eClearTypeKind clearType => (eClearTypeKind)(byte)( ( value >> 16 ) & 0xFF );

		// Create a copy with different value of vaa field
		public DrawCallType overrideVaa( eVaaKind vaa )
		{
			int v = ( value & 0xFFFF );
			v |= (int)vaa << 16;
			return new DrawCallType( v );
		}

		public override string ToString()
		{
			return $"eVaaKind.{vaa}, eMesh.{mesh}, eBrush.{brush}";
		}

		public bool isText => isTextImpl( mesh );

		static bool isTextImpl( eMesh mesh )
		{
			return mesh == eMesh.GlyphRun || mesh == eMesh.TransformedText;
		}
	}

	struct sDrawRectCommand
	{
		public Rect rect;
		public float? strokeWidth;
	}

	struct sDrawSpriteCommand
	{
		public Rect rect;
		public Sprite sprite;
	}

	struct sDrawTextCommand
	{
		public CRect rectangle;
		public int foreground;
		public Font font;
		public eTextRendering textRendering;
		public int? consoleWidth;
		public string text;
		/// <summary>Upper estimate</summary>
		public sMeshDataSize meshDataSize;
		/// <summary>Precise value here, computed in uploadVertices method</summary>
		public int actualTriangles;
	}
}