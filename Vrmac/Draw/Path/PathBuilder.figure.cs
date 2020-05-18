using Diligent.Graphics;
using System;
using System.Runtime.CompilerServices;

namespace Vrmac.Draw
{
	// This part of the class contains the implementation of iFigureBuilder interface.
	public partial class PathBuilder
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		void addVec2( Vector2 v2 )
		{
			data.Add( v2.X );
			data.Add( v2.Y );
		}

		class FigureBuilder: iFigureBuilder
		{
			readonly PathBuilder pb;
			Vector2? startingPoint;
			int segmentsCount;
			bool filled;
			bool closed;

			public bool isOpen { get; private set; }

			public FigureBuilder( PathBuilder pb )
			{
				this.pb = pb;
				startingPoint = null;
				segmentsCount = 0;
				filled = closed = false;
				isOpen = false;
			}

			public void open( bool resetStart, bool isFilled )
			{
				if( isOpen )
					throw new ApplicationException( "PathBuilder only supports 1 open figure at a time." );

				if( resetStart )
					startingPoint = null;
				segmentsCount = 0;
				closed = false;
				filled = isFilled;
				isOpen = true;
			}

			public void Dispose()
			{
				if( !startingPoint.HasValue || 0 == segmentsCount )
					throw new ApplicationException( "PathBuilder can't close the figure because it's empty" );
				if( filled && !closed )
					closeFigure();

				sPathFigure f = new sPathFigure();
				f.segmentsCount = segmentsCount;
				f.startingPoint = startingPoint.Value;
				f.isFilled = filled;
				f.isClosed = closed;
				pb.figures.Add( f );

				isOpen = false;
			}

			void iFigureBuilder.move( Vector2 point )
			{
				if( segmentsCount > 0 )
				{
					bool wasFilled = filled;
					Dispose();
					open( false, wasFilled );
				}
				startingPoint = point;
			}

			public void closeFigure()
			{
				if( 0 == segmentsCount )
					throw new ApplicationException( "PathBuilder can't close the path because it's empty." );
				closed = true;
			}

			[MethodImpl( MethodImplOptions.AggressiveInlining )]
			void ensureStart()
			{
				if( !startingPoint.HasValue )
					throw new ApplicationException( "PathBuilder can't add points, because no starting point was specified." );
			}

			[MethodImpl( MethodImplOptions.AggressiveInlining )]
			void addPoint( eSegmentKind kind, byte flags = 0 )
			{
				sPathSegment segment;
				if( segmentsCount > 0 )
				{
					int idx = pb.segments.Count - 1;
					segment = pb.segments[ idx ];
					if( segment.kind == kind && segment.flags == flags )
					{
						segment.pointsCount++;
						pb.segments[ idx ] = segment;
						return;
					}
				}

				segment = new sPathSegment();
				segment.kind = kind;
				segment.flags = flags;
				segment.pointsCount = 1;
				pb.segments.Add( segment );
				segmentsCount++;
			}

			[MethodImpl( MethodImplOptions.AggressiveInlining )]
			void iFigureBuilder.line( Vector2 endpoint )
			{
				ensureStart();
				pb.addVec2( endpoint );
				addPoint( eSegmentKind.Line );
			}

			[MethodImpl( MethodImplOptions.AggressiveInlining )]
			void iFigureBuilder.arc( Vector2 endpoint, Vector2 size, float angleDegrees, eArcFlags flags )
			{
				ensureStart();
				pb.addVec2( endpoint );
				pb.addVec2( size );
				pb.data.Add( angleDegrees );
				addPoint( eSegmentKind.Arc, (byte)flags );
			}

			[MethodImpl( MethodImplOptions.AggressiveInlining )]
			void iFigureBuilder.cubicBezier( Vector2 c1, Vector2 c2, Vector2 endpoint )
			{
				ensureStart();
				pb.addVec2( c1 );
				pb.addVec2( c2 );
				pb.addVec2( endpoint );
				addPoint( eSegmentKind.Bezier );
			}

			[MethodImpl( MethodImplOptions.AggressiveInlining )]
			void iFigureBuilder.quadraticBezier( Vector2 c1, Vector2 endpoint )
			{
				ensureStart();
				pb.addVec2( c1 );
				pb.addVec2( endpoint );
				addPoint( eSegmentKind.QuadraticBezier );
			}
		}
	}
}