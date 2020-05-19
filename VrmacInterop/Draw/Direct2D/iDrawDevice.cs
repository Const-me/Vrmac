// TODO: comment them
#pragma warning disable CS1591  // CS1591: Missing XML comment for publicly visible type or member
using ComLight;
using System;
using System.Numerics;
using System.Runtime.InteropServices;
using Vrmac.Draw;

namespace Vrmac.Direct2D
{
	/// <summary>A 2D draw device backed by Direct2D. Only works on Windows.</summary>
	[ComInterface( "032e8918-25ac-4a86-af48-f70c92d00d14", eMarshalDirection.ToManaged )]
	public interface iDrawDevice: IDisposable
	{
		[RetValIndex] iDrawContext createContext();

		[RetValIndex] iRectangleGeometry createRectangleGeometry( [In] ref Rect rect );

		[RetValIndex] iPathGeometry createPathGeometry( [In] ref float points, [In] ref sPathSegment segments, [In] ref sPathFigure figures, sPathData pathData );

		[RetValIndex] iSolidColorBrush createSolidColorBrush( [In] ref Vector4 color );

		[RetValIndex] iStrokeStyle createStrokeStyle( [In] ref sStrokeStyle style );

		[RetValIndex] id2Mesh createMesh( ref Vector2 vertices, ref ushort indices, sMeshDataSize size );

		void releaseResources( eReleaseResources what );
	}
}