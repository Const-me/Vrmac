using Diligent.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Vrmac.Draw;

namespace RenderSamples
{
	class SvgSink: iSvgSink
	{
		Rect? viewBox = null;

		void iSvgSink.viewbox( Rect rect )
		{
			viewBox = rect;
		}

		sealed class FigureBuilder: iFigureBuilder
		{
			void iFigureBuilder.closePath()
			{
				figure.closeFigure();
			}

			void iFigureBuilder.cubicBezier( Vector2 c1, Vector2 c2, Vector2 end )
			{
				figure.cubicBezier( c1, c2, end );
			}
			void iFigureBuilder.lineTo( Vector2 pt )
			{
				figure.line( pt );
			}
			void IDisposable.Dispose()
			{
				figure.Dispose();
				figure = null;
			}

			void iFigureBuilder.moveTo( Vector2 pt )
			{
				figure.move( pt );
			}

			readonly Vector4? fillColor;
			readonly Vector4? strokeColor;
			readonly float strokeWidth;
			readonly Matrix3x2 matrix;

			static bool hasStroke( float? strokeWidth, Vector4? strokeColor )
			{
				return strokeWidth > 0 && strokeColor.HasValue;
			}
			bool hasStroke()
			{
				return hasStroke( strokeWidth, strokeColor );
			}

			public bool isSame( float strokeWidth, Vector4? strokeColor, Vector4? fillColor, Matrix3x2 matrix )
			{
				if( fillColor != this.fillColor )
					return false;
				bool ts = hasStroke();
				bool ns = hasStroke( strokeWidth, strokeColor );
				if( ts != ns )
					return false;
				if( ts && strokeColor != this.strokeColor )
					return false;
				return this.matrix == matrix;
			}

			readonly PathBuilder path;
			Vrmac.Draw.iFigureBuilder figure;

			public FigureBuilder( float strokeWidth, Vector4? strokeColor, Vector4? fillColor, Matrix3x2 transform )
			{
				this.strokeWidth = strokeWidth;
				this.strokeColor = strokeColor;
				this.fillColor = fillColor;

				matrix = transform;

				path = new PathBuilder();
				open();
			}

			public (SvgPathStyle, iPathData) build() => (new SvgPathStyle( fillColor, strokeColor, strokeWidth, matrix ), path.build());

			public void open()
			{
				figure = path.newFigure( fillColor.HasValue );
			}
		}

		FigureBuilder figure = null;
		readonly List<(SvgPathStyle, iPathData)> paths = new List<(SvgPathStyle, iPathData)>();

		iFigureBuilder iSvgSink.newFigure( float? strokeWidth, Vector4? strokeColor, Vector4? fillColor, Matrix3x2 transform )
		{
			fillColor = SvgUtils.fixFill( fillColor );
			float sw = SvgUtils.fixStroke( strokeWidth, ref strokeColor );

			/* if( true == figure?.isSame( sw, strokeColor, fillColor, transform ) )
			{
				figure.open();
				return figure;
			} */
			if( null != figure )
				paths.Add( figure.build() );
			figure = new FigureBuilder( sw, strokeColor, fillColor, transform );
			return figure;
		}

		public SvgImage build( iDrawDevice device )
		{
			if( null != figure )
				paths.Add( figure.build() );

			var list = paths.Select( t => new SvgPath( device.createPathGeometry( t.Item2 ), t.Item1 ) );
			return new SvgImage( list, viewBox );
		}
	}
}