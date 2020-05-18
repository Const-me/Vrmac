using Diligent.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Vrmac;
using Vrmac.Draw;
using Matrix = Vrmac.Draw.Matrix;

namespace RenderSamples
{
	struct SvgPathStyle
	{
		public readonly Vector4? fillColor;
		public readonly Vector4? strokeColor;
		public readonly float strokeWidth;
		public readonly Matrix matrix;

		public SvgPathStyle( Vector4? fillColor, Vector4? strokeColor, float strokeWidth, Matrix matrix )
		{
			this.fillColor = fillColor;
			this.strokeColor = strokeColor;
			this.strokeWidth = strokeWidth;
			this.matrix = matrix;
		}

		public override string ToString()
		{
			return $"fill { fillColor }, stroke { strokeColor }, stroke-width { strokeWidth }";
		}
	}

	sealed class SvgPath
	{
		public readonly iGeometry geometry;
		public readonly SvgPathStyle style;

		public SvgPath( iGeometry geometry, SvgPathStyle style )
		{
			this.geometry = geometry;
			this.style = style;
		}
	}

	sealed class SvgImage
	{
		static readonly bool renderStrokes = true;
		static readonly bool renderFills = true;

		public readonly SvgPath[] paths;
		public readonly Rect? viewBox;

		public SvgImage( IEnumerable<SvgPath> paths, Rect? viewBox )
		{
			this.paths = paths.ToArray();
			this.viewBox = viewBox;
		}

		void renderBox( iDrawContext context, iGeometry geometry, int id, float boundingBoxesOpacity, ref Matrix tform )
		{
			// if( id != 0 ) return;

			Rect box;
			if( geometry is iPathGeometry pathGeometry )
				box = pathGeometry.getApproximateBounds( ref tform );
			else
				box = geometry.getBounds( ref tform );

			Vector4 color = Color.moreRandomColor( id );
			if( context.device.premultipliedAlphaBrushes )
				color *= boundingBoxesOpacity;
			else
				color.W = boundingBoxesOpacity;
			context.transform.pushIdentity();
			context.drawRectangle( box, context.device.createSolidColorBrush( color ), 1 );
			context.transform.pop();
		}

		public void render( iDrawContext context, Rect? box, float boundingBoxesOpacity )
		{
			Matrix view = Matrix.identity;
			if( viewBox.HasValue && box.HasValue )
				view = Matrix.createViewbox( box.Value, viewBox.Value );

			if( boundingBoxesOpacity > 0 )
				boundingBoxesOpacity = MathF.Min( boundingBoxesOpacity, 1 );

			int i = 0;
			using( var t = context.transform.transform( view ) )
			{
				Matrix m = paths[ 0 ].style.matrix;
				context.transform.push( m );

				Matrix transform = context.transform.current;

				foreach( var p in paths )
				{
					if( p.style.matrix != m )
					{
						m = p.style.matrix;
						context.transform.pop();
						context.transform.push( m );
						if( boundingBoxesOpacity > 0 )
						{
							transform = context.transform.current;
						}
					}

					if( boundingBoxesOpacity > 0 )
						renderBox( context, p.geometry, i++, boundingBoxesOpacity, ref transform );

					int mask = 0;
					if( p.style.fillColor.HasValue && renderFills )
						mask |= 1;
					if( p.style.strokeColor.HasValue && renderStrokes )
						mask |= 2;

					var dev = context.device;
					switch( mask )
					{
						case 1:
							context.fillGeometry( p.geometry, dev.createSolidColorBrush( p.style.fillColor.Value ) );
							break;
						case 2:
							context.drawGeometry( p.geometry, dev.createSolidColorBrush( p.style.strokeColor.Value ), p.style.strokeWidth );
							break;
						case 3:
							context.fillAndStroke( p.geometry, dev.createSolidColorBrush( p.style.fillColor.Value) , dev.createSolidColorBrush( p.style.strokeColor.Value ), p.style.strokeWidth );
							break;
					}
				}
				// No need to pop, the `using` statement cleans up everything pushed on top of the transform stack
			}
		}
	}
}