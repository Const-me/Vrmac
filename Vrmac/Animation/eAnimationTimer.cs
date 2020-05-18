namespace Vrmac.Animation
{
	/// <summary>Defines timers used by animations. You can use them too, <see cref="Animations.timers" /></summary>
	public enum eAnimationTimer: byte
	{
		/// <summary>The animation is paused while the window is minimized, resumes running once restored.</summary>
		/// <remarks>It also pauses when no animations are playing.</remarks>
		RelativeTime,
		/// <summary>The animation continues to run while the window is minimized.</summary>
		/// <remarks>
		/// <para>If the animation had finite duration, may complete an once when updates are resumed.</para>
		/// <para>This timer pauses while there’s no swap chain, while transitioning to true fullscreen mode on Windows.</para>
		/// </remarks>
		AbsoluteTime,
		/// <summary>The animation timer behaves like a wall clock, running regardless on what’s going on.</summary>
		WallClock,
	}
}