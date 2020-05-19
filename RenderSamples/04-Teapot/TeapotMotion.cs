using System;
using System.Numerics;
using Vrmac;
using Vrmac.Animation;
using Vrmac.Input;

namespace RenderSamples
{
	/// <summary>Computes and updates orientation of the teapot. Handles mouse input, also runs the animation.</summary>
	class TeapotMotion: iDeltaTimeUpdate, iAnimationProgressUpdate, iButtonHandler, iMouseMoveHandler, iMouseWheelHandler
	{
		/// <summary>Compute acceleration caused by liquid friction. It depends on velocity, and has opposite direction.</summary>
		static Vector3 liquidFriction( Vector3 velocity )
		{
			return -velocity;
		}

		void iDeltaTimeUpdate.tick( float elapsedSeconds )
		{
			// If you wonder WTF is happening here, read this:
			// https://en.wikipedia.org/wiki/Leapfrog_integration#Algorithm
			// Using the "kick-drift-kick" variant.

			float halfStep = elapsedSeconds * 0.5f;
			Vector3 vMid = velocity + acceleration * halfStep;
			rotation = rotation.rotate( vMid, elapsedSeconds );
			acceleration = motorForce + liquidFriction( vMid ) + momentOfGravity( rotation );
			velocity = vMid + acceleration * halfStep;

			if( velocity.LengthSquared() < 1E-4F * zoomFactor * zoomFactor && acceleration.LengthSquared() < 1E-4f )
			{
				anim.cancelDelta( this );
				velocity = Vector3.Zero;
			}
		}

		const float gravityFactor = 0.7f;
		static readonly Vector3 centerOfGravity = Vector3.UnitZ * -gravityFactor;

		static Vector3 momentOfGravity( Quaternion q )
		{
			return Vector3.Cross( q.transformVector( centerOfGravity ), centerOfGravity );
		}

		readonly Context content;
		readonly iInputEventTimeSource timeSource;

		Animations anim => content.animation;

		public Quaternion rotation { get; private set; } = Quaternion.Identity;
		Vector3 velocity = Vector3.Zero;
		Vector3 acceleration = Vector3.Zero;

		const float defaultMotorForce = 0.44f;
		float forceMul = 1.0f;

		public TeapotMotion( Context content, iInputEventTimeSource timeSource )
		{
			this.content = content;
			this.timeSource = timeSource;
			updateForce();
			anim.startDelta( this );
		}

		public void toggleAnimation()
		{
			if( anim.playingDelta( this ) )
				anim.cancelDelta( this );
			else
				anim.startDelta( this );
		}

		public void toggleForce()
		{
			if( forceMul > 0 )
				forceMul = 0;
			else
			{
				forceMul = 1;
				acceleration = Vector3.Zero;
				anim.startDelta( this );
			}
			updateForce();
		}

		public void increaseForce()
		{
			if( forceMul <= 0 )
			{
				forceMul = 1;
				anim.startDelta( this );
			}
			else
				forceMul *= MathF.Sqrt( 2.0f );
			updateForce();
		}

		public void decreaseForce()
		{
			if( forceMul <= 0 )
			{
				forceMul = 1;
				anim.startDelta( this );
			}
			else
				forceMul /= MathF.Sqrt( 2.0f );
			updateForce();
		}

		/// <summary>Acceleration caused by external force.</summary>
		Vector3 motorForce;
		void updateForce()
		{
			motorForce = Vector3.UnitZ * ( defaultMotorForce * forceMul );
		}

		CPoint? prevMouse = null;
		readonly MouseVelocity mouseVelocity = new MouseVelocity();

		void iButtonHandler.buttonDown( CPoint pos, eMouseButton changedButtons, eMouseButtonsState bs )
		{
			if( changedButtons == eMouseButton.Left )
			{
				anim.cancelDelta( this );
				prevMouse = pos;
				mouseVelocity.reset();
				mouseVelocity.add( pos, timeSource.messageTime );
			}
		}

		void iButtonHandler.buttonUp( CPoint pos, eMouseButton changedButtons, eMouseButtonsState bs )
		{
			if( changedButtons == eMouseButton.Left )
			{
				acceleration = Vector3.Zero;
				prevMouse = null;

				mouseVelocity.add( pos, timeSource.messageTime );
				Vector2? velOrNull = mouseVelocity.compute();
				if( velOrNull.HasValue )
				{
					Vector2 velocity = velOrNull.Value;
					velocity /= new Vector2( content.swapChainSize.cx, content.swapChainSize.cy );
					this.velocity = new Vector3( -velocity.Y, 0, -velocity.X );
				}
				anim.startDelta( this );
			}
		}

		const float mouseSpeedMul = 4;

		void iMouseMoveHandler.mouseMove( CPoint pos, eMouseButtonsState bs )
		{
			if( bs.HasFlag( eMouseButtonsState.Left ) && prevMouse.HasValue )
			{
				float dx = pos.x - prevMouse.Value.x;
				float dy = pos.y - prevMouse.Value.y;
				prevMouse = pos;
				dx /= content.swapChainSize.cx;
				dy /= content.swapChainSize.cy;
				dx *= mouseSpeedMul;
				dy *= mouseSpeedMul;

				Quaternion q = Quaternion.CreateFromYawPitchRoll( 0, -dy, -dx );
				rotation = Quaternion.Multiply( q, rotation );
				Dispatcher.currentDispatcher.nativeDispatcher.renderFrame( content.renderContext, true );
				mouseVelocity.add( pos, timeSource.messageTime );
			}
		}

		float zoomLevelStarted = 0;
		float zoomLevelCurrent = 0;
		int zoomLevel = 0;

		const int maxZoom = 16;
		public float zoomFactor { get; private set; } = 1;
		static readonly float zoomFactorMul = MathF.Sqrt( 2 );

		static readonly TimeSpan zoomAnimation = TimeSpan.FromMilliseconds( 500 );
		void iMouseWheelHandler.wheel( CPoint point, int delta, eMouseButtonsState bs )
		{
			if( delta < 0 )
			{
				if( zoomLevel <= 0 )
					return;
				zoomLevelStarted = zoomLevelCurrent;
				zoomLevel--;
			}
			else
			{
				if( zoomLevel >= maxZoom )
					return;

				zoomLevelStarted = zoomLevelCurrent;
				zoomLevel++;
			}
			anim.startProgress( zoomAnimation, this );
		}
		void iAnimationProgressUpdate.tick( float relativeDuration )
		{
			if( relativeDuration >= 1 )
				zoomLevelStarted = zoomLevelCurrent = zoomLevel;
			else
				zoomLevelCurrent = MathUtils.smoothStep( zoomLevelStarted, zoomLevel, relativeDuration );
			zoomFactor = 1.0f / MathF.Pow( zoomFactorMul, zoomLevelCurrent );
		}
	}
}