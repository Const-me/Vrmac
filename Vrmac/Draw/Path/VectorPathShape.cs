using System;
using System.Collections.Generic;
using System.Linq;

namespace Vrmac.Draw
{
	/// <summary>Readonly path geometry in system RAM. To create this class, use <see cref="PathBuilder" />.</summary>
	sealed class VectorPathShape: iPathData
	{
		readonly eFillMode fillMode;
		readonly sPathFigure[] figures;
		readonly sPathSegment[] segments;
		readonly float[] data;

		void createNativeStructures( out Span<float> p, out Span<sPathSegment> s, out Span<sPathFigure> f, out sPathData pd )
		{
			p = data.AsSpan();
			s = segments.AsSpan();
			f = figures.AsSpan();
			pd = new sPathData();
			pd.fillMode = fillMode;
			pd.figuresCount = f.Length;
		}

		Direct2D.iPathGeometry iPathData.createPathGeometry( Direct2D.iDrawDevice device )
		{
			createNativeStructures( out var p, out var s, out var f, out var pd );
			return device.createPathGeometry( ref p.GetPinnableReference(), ref s.GetPinnableReference(), ref f.GetPinnableReference(), pd );
		}

		iPathGeometry iPathData.createPathGeometry( iVrmacDraw utils )
		{
			createNativeStructures( out var p, out var s, out var f, out var pd );
			return utils.createPathGeometry( ref p.GetPinnableReference(), ref s.GetPinnableReference(), ref f.GetPinnableReference(), pd );
		}

		internal VectorPathShape( eFillMode fillMode, IReadOnlyCollection<sPathFigure> figures, IReadOnlyCollection<sPathSegment> segments, IReadOnlyCollection<float> data )
		{
			this.fillMode = fillMode;
			this.figures = figures.ToArray();
			this.segments = segments.ToArray();
			this.data = data.ToArray();
		}

		internal VectorPathShape( eFillMode fillMode, sPathFigure[] figures, sPathSegment[] segments, float[] data )
		{
			this.fillMode = fillMode;
			this.figures = figures;
			this.segments = segments;
			this.data = data;
		}
	}
}