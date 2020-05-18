using Diligent.Graphics;
using System;
using System.Linq;
using Vrmac;
using Vrmac.Animation;
using Vrmac.Draw;
using Matrix = Vrmac.Draw.Matrix;

namespace RenderSamples
{
	class ShapesSample: SampleBase, iDeltaTimeUpdate
	{
		const float strokeWidth = 11;
		static readonly bool splinePath = false;
		static readonly bool playAnimation = true;
		static readonly bool weirdShapeFilled = true;
		const eFillMode pathFillMode = eFillMode.Winding;
		// const eFillMode pathFillMode = eFillMode.Alternate;
		// const eLineJoin pathLineJoin = eLineJoin.Miter;
		// const eLineJoin pathLineJoin = eLineJoin.Bevel;
		const eLineJoin pathLineJoin = eLineJoin.Round;
		// const eLineJoin pathLineJoin = eLineJoin.MiterOrBevel;
		// const eLineCap lineCaps = eLineCap.Triangle;
		const eLineCap lineCaps = eLineCap.Round;

		const int pointsCount = 14;
		// const float radiansPerSecond = MathHelper.Pi / 11;
		const float radiansPerSecond = 0;

		iBrush[] brushes;
		readonly iGeometry[] shapes = new iGeometry[ 3 ];

		readonly string[] brushColors = new string[ 6 ]
		{
			"#700F", "#F00F", "#70F0", "#F0F0", "#7F00", "#FF00"
		};

		Vector2[] randomVertices;
		Vector2[] randomSpeeds;
		Vector2 speedMultiplier;
		Rect rcRandom;
		iStrokeStyle strokeStyle;
		Angle rotationAngle = new Angle();

		void iDeltaTimeUpdate.tick( float elapsedSeconds )
		{
			for( int i = 0; i < pointsCount; i++ )
			{
				Vector2 v = randomVertices[ i ];
				Vector2 speed = randomSpeeds[ i ];
				v += speed * speedMultiplier * elapsedSeconds;
				randomVertices[ i ] = v;
				if( v.X < 0 || v.X > 1 )
					speed.X = -speed.X;
				if( v.Y < 0 || v.Y > 1 )
					speed.Y = -speed.Y;
				randomSpeeds[ i ] = speed;
			}
			shapes[ 2 ] = createWeirdShape();
			rotationAngle.rotate( radiansPerSecond, elapsedSeconds );
		}

		public ShapesSample()
		{
			var rnd = new Random( 42 );
			randomVertices = Enumerable.Range( 0, pointsCount )
				.Select( i => new Vector2( (float)rnd.NextDouble(), (float)rnd.NextDouble() ) )
				.ToArray();

			randomSpeeds = Enumerable.Range( 0, pointsCount )
				.Select( i => new Vector2( (float)rnd.NextDouble(), (float)rnd.NextDouble() ) )
				.Select( v => v.normalized() )
				.ToArray();
		}

		protected override void createResources( IRenderDevice rd )
		{
			var dd = context.drawDevice;
			sStrokeStyle ss = new sStrokeStyle()
			{
				lineJoin = pathLineJoin,
				miterLimit = 2,
				startCap = lineCaps,
				endCap = lineCaps,
			};
			strokeStyle = dd.createStrokeStyle( ss );

			onResized( dd.viewportSize, context.dpiScalingFactor );
			dd.resized.add( this, onResized );
			brushes = brushColors.Select( dd.createSolidBrush ).ToArray();
			if( playAnimation )
				context.animation.startDelta( this );
		}

		static readonly Vector4 clearColor = Color.parse( "white" );

		void onResized( Vector2 size, double dpi )
		{
			foreach( var s in shapes )
				s?.Dispose();
			Rect rc = new Rect( Vector2.Zero, size );
			rc = rc.deflate( 40, 20 );

			shapes[ 0 ] = context.drawDevice.createPathGeometry( Shapes.roundedRectangle( rc, 10 ) );
			shapes[ 1 ] = createPolygon( rc.center, rc.size.minCoordinate * 0.45f, 7 );
			rcRandom = rc.deflate( 40, 20 );
			shapes[ 2 ] = createWeirdShape();
			Vector2 rcRandomSize = rcRandom.size.normalized();
			speedMultiplier = new Vector2( 0.025f ) / rcRandomSize;
		}

		iGeometry createPolygon( Vector2 center, float radius, int verts )
		{
			Span<Vector2> points = stackalloc Vector2[ verts ];
			double angleMul = Math.PI * 2.0 / verts;
			for( int i = 0; i < verts; i++ )
			{
				MathHelper.sinCos( (float)( angleMul * i ), out float sin, out float cos );
				// First point on the top, and counter-clockwise direction.
				Vector2 v = new Vector2( -sin, -cos );
				points[ i ] = v * radius + center;
			}
			return context.drawDevice.createPathGeometry( Shapes.polygon( points ) );
		}

		iGeometry createWeirdShape()
		{
			Vector2[] points = randomVertices
				.Select( v => rcRandom.getPoint( v ) )
				.ToArray();

			PathBuilder pb = new PathBuilder();
			pb.fillMode = pathFillMode;
			if( !splinePath )
			{
				using( var f = pb.newFigure( weirdShapeFilled ) )
				{
					f.move( points[ 0 ] );
					for( int i = 1; i < points.Length - 1; i++ )
						f.line( points[ i ] );
					f.closeFigure();
				}
				return context.drawDevice.createPathGeometry( pb.build() );
			}

			Vector2[] tangents = new Vector2[ points.Length ];

			for( int i = 0; i < points.Length; i++ )
			{
				int prev = ( i - 1 + points.Length ) % points.Length;
				int next = ( i + 1 ) % points.Length;
				Vector2 t = points[ next ] - points[ prev ];
				t.Normalize();
				tangents[ i ] = t;
			}

			using( var f = pb.newFigure() )
			{
				f.move( points[ 0 ] );
				for( int i = 0; i < points.Length; i++ )
				{
					int iNext = ( i + 1 ) % points.Length;
					Vector2 v1 = points[ i ];
					Vector2 v2 = points[ iNext ];
					float distance = ( v2 - v1 ).Length();

					Vector2 t1 = tangents[ i ];
					Vector2 t2 = tangents[ iNext ];
					f.cubicBezier( v1 + t1 * distance / 3, v2 - t2 * distance / 3, v2 );
				}
			}

			return context.drawDevice.createPathGeometry( pb.build() );
		}

		iBrush brush( int i )
		{
			return brushes[ i % brushes.Length ];
		}

		void drawGeometry( iDrawContext dc, int i )
		{
			dc.fillGeometry( shapes[ i ], brush( i * 2 + 2 ) );
			// dc.drawGeometry( shapes[ i ], brush( i * 2 + 1 ), strokeWidth, strokeStyle );
		}

		static Matrix rotationMatrix( Vector2 vps, float angle )
		{
			return Matrix.createRotation( angle, vps / 2 );
		}

		protected override void render( ITextureView swapChainRgb, ITextureView swapChainDepthStencil )
		{
			// var im = context.context;
			// im.SetRenderTarget( swapChainRgb, swapChainDepthStencil );
			// im.ClearRenderTarget( swapChainRgb, Color.white );

			// using( var dc = context.drawDevice.begin( swapChainRgb, swapChainDepthStencil, Color.transparent ) )
			using( var dc = context.drawDevice.begin( swapChainRgb, swapChainDepthStencil, Color.white ) )
			{
				var transform = rotationMatrix( context.drawDevice.viewportSize, rotationAngle );
				// var transform = rotationMatrix( context.drawDevice.viewportSize, 1 );
				dc.transform.push( transform );
				// ConsoleLogger.logDebug( "drawDevice.begin" );
				drawGeometry( dc, 2 ); return;
				for( int i = 0; i < shapes.Length; i++ )
					drawGeometry( dc, i );
			}
		}
	}
}