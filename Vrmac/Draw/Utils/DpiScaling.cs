using Diligent.Graphics;
using System;

namespace Vrmac.Draw
{
	/// <summary>Utility object to scale stuff with DPI, from pixels to units and vice versa.</summary>
	public struct DpiScaling
	{
		/// <summary>Multiplier to get DPI scaled units from pixels</summary>
		public readonly float mulUnits;
		/// <summary>Multiplier to get pixels from DPI scaled units</summary>
		public readonly float mulPixels;

		internal DpiScaling( double scaling )
		{
			mulPixels = (float)scaling;
			mulUnits = (float)( 1.0 / scaling );
		}

		/// <summary>Scale a 2D vector from pixels to units</summary>
		public Vector2 units( Vector2 pixels )
		{
			return pixels * mulUnits;
		}
		/// <summary>Scale a 2D vector from units to pixels</summary>
		public Vector2 pixels( Vector2 units )
		{
			return units * mulPixels;
		}
		/// <summary>Scale a rectangle from pixels to units</summary>
		public Rect units( Rect pixels )
		{
			return pixels * mulUnits;
		}
		/// <summary>Scale a rectangle from units to pixels</summary>
		public Rect pixels( Rect units )
		{
			return units * mulPixels;
		}
		/// <summary>Create rectangle in pixels from size in pixels, the top left corner being [ 0, 0 ]</summary>
		public Rect fullRectPixels( CSize sizePixels )
		{
			Vector2 bottomRight = new Vector2( sizePixels.cx, sizePixels.cy );
			return new Rect( Vector2.Zero, bottomRight );
		}

		/// <summary>Create rectangle in units from size in pixels, the top left corner being [ 0, 0 ]</summary>
		public Rect fullRectUnits( CSize sizePixels )
		{
			Vector2 bottomRight = new Vector2( sizePixels.cx, sizePixels.cy );
			bottomRight *= mulUnits;
			return new Rect( Vector2.Zero, bottomRight );
		}

		/// <summary>Inflate rectangle in units, snapping it to pixels</summary>
		public Rect inflateToPixels( Rect rectUnits )
		{
			Rect rc = pixels( rectUnits );
			rc = rc.inflateToIntegers();
			return units( rc );
		}

		/// <summary>Round rectangle in units to nearest pixels, may result in inflated or deflated rectangle depending on the values</summary>
		public Rect roundToPixels( Rect rectUnits )
		{
			Rect rc = pixels( rectUnits );
			rc = rc.roundToIntegers();
			return units( rc );
		}
		/// <summary>Round an X or Y coordinate in units to nearest pixels</summary>
		public float roundToPixels( float coordUnits )
		{
			return MathF.Round( coordUnits * mulPixels ) * mulUnits;
		}

		/// <summary>Deflate rectangle in units, snapping it to pixels</summary>
		public Rect deflateToPixels( Rect rectUnits )
		{
			Rect rc = pixels( rectUnits );
			rc = rc.deflateToIntegers();
			return units( rc );
		}
	}
}