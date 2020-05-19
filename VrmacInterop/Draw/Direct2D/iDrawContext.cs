// TODO: comment them
#pragma warning disable CS1591  // CS1591: Missing XML comment for publicly visible type or member
using ComLight;
using System;
using System.Numerics;
using System.Runtime.InteropServices;
using Vrmac.Draw;
using Matrix = Vrmac.Draw.Matrix;

namespace Vrmac.Direct2D
{
	/// <summary>2D draw context backed by Direct2D. Only works on Windows.</summary>
	[ComInterface( "8af90031-dc2d-4314-a13d-fdf35b3f17da", eMarshalDirection.ToManaged )]
	public interface iDrawContext: IDisposable
	{
		void beginDraw();
		void endDraw();

		void clear( [In] ref Vector4 color );

		void fillRectangle( [In] ref Rect rect, iBrush brush );
		void fillGeometry( iGeometry geometry, iBrush brush, iBrush opacityBrush = null );

		void drawGeometry( iGeometry geometry, iBrush brush, float strokeWidth, iStrokeStyle strokeStyle );
		void drawRectangle( [In] ref Rect rect, float strokeWidth, iBrush brush );

		void fillMesh( id2Mesh mesh, iBrush brush );

		void setTransform( [In] ref Matrix transform );

		void setDpiScaling( double scale );
		void getDpiScaling( out double scale );
		double dpiScaling { get; set; }

		[RetValIndex] iGeometryRealization createFilledGeometryRealization( iGeometry geometry, [In] ref Matrix transform );
		[RetValIndex] iGeometryRealization createStrokedGeometryRealization( iGeometry geometry, [In] ref Matrix transform, float strokeWidth, iStrokeStyle strokeStyle );
		void drawGeometryRealization( iGeometryRealization geometry, iBrush brush );
	}
}