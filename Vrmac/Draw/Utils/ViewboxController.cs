using Diligent.Graphics;
using System;
using System.Runtime.CompilerServices;
using Vrmac.Animation;

namespace Vrmac.Draw
{
	/// <summary>This utility class implements pan and animated zoom for a 2D image</summary>
	public sealed class ViewboxController: iAnimationProgressUpdate
	{
		const float defaultScale = 0.8f;

		readonly float defaultZoom;
		readonly Context context;

		/// <summary>Create the object</summary>
		public ViewboxController( Context context, float zoom = defaultScale )
		{
			this.context = context;
			defaultZoom = zoom;
		}

		/// <summary>Pan with offset in units</summary>
		public void pan( Vector2 amount )
		{
			translationOffset += amount;
		}

		Vector2 translationOffset = Vector2.Zero;

		const int maxZoom = 16;
		float zoomFactor = 1;
		static readonly float zoomFactorMul = MathF.Sqrt( 2 );
		Vector2 zoomFixedPoint;

		float zoomLevelStarted = 0;
		float zoomLevelCurrent = 0;
		int zoomLevel = 0;
		float currentScale => defaultScale / zoomFactor;

		static readonly TimeSpan zoomAnimation = TimeSpan.FromMilliseconds( 500 );

		/// <summary>Start animated zoom around the center point in physical pixels</summary>
		public bool startZoom( CPoint position, int delta )
		{
			if( delta < 0 )
			{
				if( zoomLevel <= 0 )
					return false;
				zoomLevelStarted = zoomLevelCurrent;
				zoomLevel--;
			}
			else
			{
				if( zoomLevel >= maxZoom )
					return false;

				zoomLevelStarted = zoomLevelCurrent;
				zoomLevel++;
			}

			// We want zoomFixedPoint to contain the position relative to the image rectangle
			Vector2 positionUnits = position.asFloat * context.drawDevice.dpiScaling.mulUnits;
			zoomFixedPoint = getImageBox().makeRelative( positionUnits );
			context.animation.startProgress( zoomAnimation, this );
			return true;
		}

		void iAnimationProgressUpdate.tick( float relativeDuration )
		{
			if( relativeDuration >= 1 )
				zoomLevelStarted = zoomLevelCurrent = zoomLevel;
			else
				zoomLevelCurrent = MathHelper.SmoothStep( zoomLevelStarted, zoomLevel, relativeDuration );

			// Deal with the zoom center
			Vector2 prevFixed = getImageBox().getPoint( zoomFixedPoint );
			// The following line is the one actually changing the current zoom level
			zoomFactor = 1.0f / MathF.Pow( zoomFactorMul, zoomLevelCurrent );

			// Adjustment the translation offset value so the fixed point of the zoom ain't moving anywhere
			Vector2 newFixed = getImageBox().getPoint( zoomFixedPoint );
			translationOffset += ( prevFixed - newFixed );
		}

		/// <summary>Get the rectangle; the result changes dynamically by user actions, also with animations</summary>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public Rect getImageBox()
		{
			Rect rcImage = new Rect( Vector2.Zero, context.drawDevice.viewportSize );
			rcImage = rcImage.scaleAroundCenter( currentScale );
			return rcImage.offset( translationOffset );
		}

		/// <summary>Reset everything to defaults. Also, if the animation was playing, stop it.</summary>
		public void reset()
		{
			context.animation.cancelProgress( this, false );

			translationOffset = default;
			zoomFactor = 1;
			zoomLevelStarted = zoomLevelCurrent = 0;
			zoomLevel = 0;
		}
	}
}