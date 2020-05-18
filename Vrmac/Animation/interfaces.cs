using System;

namespace Vrmac.Animation
{
	/// <summary>Base interface for animations</summary>
	public interface iAnimationBase { }

	/// <summary>Interface to receive total duration since the animation was started</summary>
	public interface iAbsoluteTimeUpdate: iAnimationBase
	{
		/// <summary>Update with absolute duration since the animation was started</summary>
		void tick( TimeSpan elapsed );
	}

	/// <summary>Interface to receive time in seconds since previously rendered frame.</summary>
	public interface iDeltaTimeUpdate: iAnimationBase
	{
		/// <summary>Update with time in seconds since the previous one</summary>
		void tick( float elapsedSeconds );
	}

	/// <summary>Interface to receive progress of the animation.</summary>
	public interface iAnimationProgressUpdate: iAnimationBase
	{
		/// <summary>Update with the progress value, in [ 0 .. 1 ] interval</summary>
		void tick( float relativeDuration );
	}
}