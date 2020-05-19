using System;
using System.Collections.Generic;
using System.Numerics;

namespace Vrmac.Draw
{
	/// <summary>An interface to build paths with PathBuilder</summary>
	public interface iFigureBuilder: IDisposable
	{
		/// <summary>Start a new figure from the specified point. If this is called after some segments already added, will create a new figure instead.</summary>
		void move( Vector2 point );
		/// <summary>Add line segment to the figure</summary>
		void line( Vector2 endpoint );
		/// <summary>Add circular arc segment to the figure</summary>
		void arc( Vector2 endpoint, Vector2 size, float angleDegrees, eArcFlags flags );
		/// <summary>Add cubic Bezier segment to the figure</summary>
		void cubicBezier( Vector2 c1, Vector2 c2, Vector2 endpoint );
		/// <summary>Add quadratic Bezier segment to the figure</summary>
		void quadraticBezier( Vector2 c1, Vector2 endpoint );
		/// <summary>Close the figure</summary>
		void closeFigure();
	}

	/// <summary>Utility class to build vector shapes in system RAM</summary>
	public partial class PathBuilder
	{
		readonly List<float> data;
		readonly List<sPathSegment> segments;
		readonly List<sPathFigure> figures;
		/// <summary>Fill mode for the path</summary>
		public eFillMode fillMode = eFillMode.Winding;

		FigureBuilder fb;

		/// <summary>Construct the builder</summary>
		public PathBuilder()
		{
			data = new List<float>();
			segments = new List<sPathSegment>();
			figures = new List<sPathFigure>();
			fb = new FigureBuilder( this );
		}

		internal PathBuilder( int expectedFloats, int expectedSegments )
		{
			data = new List<float>( expectedFloats );
			segments = new List<sPathSegment>( expectedSegments );
			figures = new List<sPathFigure>( 1 );
			fb = new FigureBuilder( this );
		}

		/// <summary>Add a figure to the path. You must dispose the object once you’re done building the new figure.</summary>
		public iFigureBuilder newFigure( bool isFilled = true )
		{
			fb.open( true, isFilled );
			return fb;
		}

		/// <summary>Build the path, return path data in a readonly object in system RAM</summary>
		public iPathData build()
		{
			if( fb.isOpen )
				throw new ApplicationException( "You must dispose the current figure before calling PathBuilder.build()" );
			return new VectorPathShape( fillMode, figures, segments, data );
		}

		/// <summary>Clear everything built. This way you can reuse the builder to build multiple paths, saves non-trivial amount of CPU time wasted on GC.</summary>
		public void clear()
		{
			if( fb.isOpen )
				throw new ApplicationException( "You must dispose the current figure before calling PathBuilder.clear()" );

			data.Clear();
			segments.Clear();
			figures.Clear();
			fillMode = eFillMode.Winding;
			fb = new FigureBuilder( this );
		}
	}
}