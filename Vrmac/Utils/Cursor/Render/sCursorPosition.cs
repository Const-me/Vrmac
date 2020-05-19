using System.Numerics;

namespace Vrmac.Utils.Cursor.Render
{
	/// <summary>Stuff to position the cursor quad</summary>
	struct sCursorPosition
	{
		// 2.0 / window size in pixels
		double positionMulX, positionMulY;
		// Cursor hotspot
		CPoint hotspot;
		// Sprite size in clip space units
		Vector2 size;

		CSize textureSize;

		public void setWindowSize( CSize size )
		{
			// ConsoleLogger.logDebug( "sCursorPosition.setWindowSize: {0}", size );
			positionMulX = 2.0 / size.cx;
			positionMulY = 2.0 / size.cy;
			updateSize();
		}

		public void setTexture( CursorTexture texture )
		{
			textureSize = texture.size;
			hotspot = texture.hotspot;
			updateSize();
		}

		void updateSize()
		{
			size.X = (float)( positionMulX * textureSize.cx );
			size.Y = (float)( positionMulY * textureSize.cy );
		}

		public Vector4 updatePosition( CPoint point )
		{
			CPoint topLeft = point - hotspot;
			float x = (float)( positionMulX * topLeft.x - 1 );
			float y = (float)( 1 - positionMulY * topLeft.y );
			return new Vector4( x, y, size.X, size.Y );
		}
	}
}