using Diligent.Graphics;
using System;
using System.Runtime.InteropServices;

namespace Vrmac.Draw
{
	/// <summary>Flags about the data currently held in iPathGeometry instance</summary>
	[Flags]
	public enum ePathFlags: byte
	{
		/// <summary>None of the below</summary>
		None = 0,
		/// <summary>At least 1 figure has sPathFigure.isFilled = true</summary>
		AnyFilledFigures = 1,
		/// <summary>At least 1 figure has sPathFigure.isFilled = false</summary>
		AnyStrokedFigures = 2,
		/// <summary>The path contains splines, circular arcs and/or Bezier patches. Unlike polygons, paths with splines are resolution dependent.</summary>
		AnySplines = 4,
	}

	/// <summary>Size of the mesh built by iPathGeometry object</summary>
	[StructLayout( LayoutKind.Sequential )]
	public struct sMeshDataSize
	{
		/// <summary>Count of vertices in the buffers</summary>
		public int vertices;
		/// <summary>Count of triangles in the mesh</summary>
		public int triangles;

		/// <summary>A string for debugger</summary>
		public override string ToString()
		{
			return $"{ vertices } vertices, { triangles } triangles";
		}

		/// <summary>Construct from 2 values</summary>
		public sMeshDataSize( int verts, int tris )
		{
			vertices = verts;
			triangles = tris;
		}

		/// <summary>Add them together</summary>
		public static sMeshDataSize operator +( sMeshDataSize a, sMeshDataSize b )
		{
			return new sMeshDataSize( a.vertices + b.vertices, a.triangles + b.triangles );
		}
	};

	/// <summary>2D vertex with extra 32 bits ID field</summary>
	[StructLayout( LayoutKind.Sequential )]
	public struct sVertexWithId
	{
		/// <summary>2D position of the vertex</summary>
		public Vector2 position;
		/// <summary>This thing looks trivial, but allows huge savings in draw calls and PCIe bandwidth. The 2D shaders use this index to access extra bound buffers, quite a few of them.</summary>
		public uint id;

		/// <summary>A string for debugger</summary>
		public override string ToString()
		{
			return $"Position [ { position.X }, { position.Y } ], draw call { id >> 8 }, VAA { id & 0xFF }";
		}
	};

	/// <summary>Same as above, but uses position fields for packed 16-bit integers</summary>
	[StructLayout( LayoutKind.Sequential )]
	public struct sGlyphVertex
	{
		/// <summary>Position in physical pixels</summary>
		public ushort x, y;
		/// <summary>Texture coordinates, they use 1.15 fixed point format</summary>
		public uint uv;
		/// <summary></summary>
		public uint misc;

		/// <summary>A string for debugger</summary>
		public override string ToString()
		{
			return $"Position [ {x}, {y} ], texture [ 0x{ uv & 0xFFFF:x}, 0x{ uv >> 16:x} ], draw call { misc >> 8 }, atlas layer { misc & 0xFF }";
		}
	}
}