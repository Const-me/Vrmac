using System;
using System.Numerics;
using System.Runtime.InteropServices;
using Vrmac.Draw;

namespace Vrmac.Direct2D
{
	/// <summary>Couple extension methods to wrap triangle mesh COM API into into .NET</summary>
	public static class TriMeshExt
	{
		/// <summary>Creates a D2D mesh from a pair of readonly spans</summary>
		public static id2Mesh createMesh( this iDrawDevice device, ReadOnlySpan<Vector2> vertices, ReadOnlySpan<ushort> indices )
		{
			sMeshDataSize mds = new sMeshDataSize( vertices.Length, indices.Length / 3 );
			return device.createMesh( ref MemoryMarshal.GetReference( vertices ), ref MemoryMarshal.GetReference( indices ), mds );
		}
	}
}