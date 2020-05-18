using Diligent.Graphics;
using System;

namespace Vrmac.Draw
{
	/// <summary>Readonly path geometry in system RAM for a path that only has 1 segment, i.e. all points share the same type.</summary>
	sealed class SimpleShape: iPathData
	{
		readonly Vector2 startingPoint;
		readonly eFillMode fillMode;
		readonly eSegmentKind segmentKind;
		readonly bool isFilled, isClosed;
		readonly byte arcFlags;
		readonly int pointsCount;
		readonly float[] data;

		void createNativeStructures( out Span<float> p, out sPathSegment s, out sPathFigure f, out sPathData pd )
		{
			p = data.AsSpan();

			s = new sPathSegment();
			s.kind = segmentKind;
			s.pointsCount = pointsCount;
			s.flags = arcFlags;

			f = new sPathFigure();
			f.startingPoint = startingPoint;
			f.segmentsCount = 1;
			f.isFilled = isFilled;
			f.isClosed = isClosed;

			pd = new sPathData();
			pd.fillMode = fillMode;
			pd.figuresCount = 1;
		}
		Direct2D.iPathGeometry iPathData.createPathGeometry( Direct2D.iDrawDevice device )
		{
			createNativeStructures( out var p, out var s, out var f, out var pd );
			return device.createPathGeometry( ref p.GetPinnableReference(), ref s, ref f, pd );
		}
		iPathGeometry iPathData.createPathGeometry( iVrmacDraw utils )
		{
			createNativeStructures( out var p, out var s, out var f, out var pd );
			return utils.createPathGeometry( ref p.GetPinnableReference(), ref s, ref f, pd );
		}

		static int getPointsCount( eSegmentKind segmentKind, int floatsCount )
		{
			int nf;
			switch( segmentKind )
			{
				case eSegmentKind.Line:
					nf = 2;
					break;
				case eSegmentKind.Arc:
					nf = 5;
					break;
				case eSegmentKind.Bezier:
					nf = 6;
					break;
				case eSegmentKind.QuadraticBezier:
					nf = 4;
					break;
				default:
					throw new ArgumentException( $"Unexpected eSegmentKind value { (int)segmentKind }" );
			}

			if( 0 != floatsCount % nf )
				throw new ArgumentException( $"Invalid floats count; for { segmentKind } must be a multiple of { nf }" );
			return floatsCount / nf;
		}

		internal SimpleShape( eSegmentKind segmentKind, Vector2 startingPoint, float[] data, eFillMode fillMode = eFillMode.Winding, byte arcFlags = 0, bool isFilled = true, bool isClosed = true )
		{
			this.startingPoint = startingPoint;
			this.fillMode = fillMode;
			this.segmentKind = segmentKind;
			this.isFilled = isFilled;
			this.isClosed = isClosed;
			this.arcFlags = arcFlags;
			pointsCount = getPointsCount( segmentKind, data.Length );
			this.data = data;
		}
	}
}