using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Vrmac.Utils;

namespace Vrmac.Animation
{
	/// <summary>Manages playing animations.</summary>
	/// <remarks>
	/// <para>It doesn’t keep strong references, uses weak tables. If an object is garbage collected while playing an animation, so be it, this class doesn’t stop it.</para>
	/// <para>If you start an animation while another one is playing for the same updating object, the previous one will be dropped silently, the new one will start playing.</para>
	/// </remarks>
	public sealed class Animations
	{
		/// <summary>Animation timers, all 3 of them.</summary>
		public readonly Timers timers = new Timers();

		readonly Context content;

		internal Animations( Context content )
		{
			this.content = content;
		}

		static readonly TimeSpan minDuration = TimeSpan.FromMilliseconds( 1 );
		static readonly TimeSpan warnDuration = TimeSpan.FromHours( 38.83614815 );  // assuming 120Hz display; for 60Hz will be 2 times longer.

		static void validateDuration( TimeSpan duration, bool max )
		{
			if( duration < minDuration )
				throw new ArgumentOutOfRangeException();
			if( max && duration > warnDuration )
				ConsoleLogger.logWarning( "Animation is too long, float precision issues might cause glitches" );
		}

		/// <summary>Start a finite animation that receives total time elapsed</summary>
		public void startAbs( TimeSpan duration, iAbsoluteTimeUpdate obj, eAnimationTimer time = eAnimationTimer.AbsoluteTime )
		{
			validateDuration( duration, false );
			absolute.AddOrUpdate( obj, new Animation( time, timers[ time ] + duration ) );
			RunPolicy.animationStarted( content );
		}
		/// <summary>Start a finite animation that receives time passed since previous frame rendered</summary>
		public void startDelta( TimeSpan duration, iDeltaTimeUpdate obj, eAnimationTimer time = eAnimationTimer.RelativeTime )
		{
			validateDuration( duration, false );
			delta.AddOrUpdate( obj, new Animation( time, timers[ time ] + duration ) );
			RunPolicy.animationStarted( content );
		}
		/// <summary>Start a finite animation that receives a progress value, from 0 to 1.</summary>
		public void startProgress( TimeSpan duration, iAnimationProgressUpdate obj, eAnimationTimer time = eAnimationTimer.RelativeTime )
		{
			validateDuration( duration, true );
			progress.AddOrUpdate( obj, new ProgressAnimation( time, timers[ time ], duration ) );
			RunPolicy.animationStarted( content );
		}

		/// <summary>Start an infinite animation that receives total time elapsed</summary>
		public void startAbs( iAbsoluteTimeUpdate obj, eAnimationTimer time = eAnimationTimer.AbsoluteTime )
		{
			absolute.AddOrUpdate( obj, new Animation( time ) );
			RunPolicy.animationStarted( content );
		}
		/// <summary>Start an infinite animation that receives time passed since previous frame rendered</summary>
		public void startDelta( iDeltaTimeUpdate obj, eAnimationTimer time = eAnimationTimer.RelativeTime )
		{
			delta.AddOrUpdate( obj, new Animation( time ) );
			RunPolicy.animationStarted( content );
		}

		/// <summary>Cancel animation</summary>
		public void cancelAbs( iAbsoluteTimeUpdate obj, bool complete = true )
		{
			if( absolute.TryGetValue( obj, out var pa ) )
			{
				if( complete && pa.finish != TimeSpan.MaxValue )
					obj.tick( pa.finish );
				absolute.Remove( obj );
				cancelled();
				return;
			}
			// ConsoleLogger.logWarning( "Cancel animation did nothing, it wasn't running" );
		}
		/// <summary>Cancel animation</summary>
		public void cancelDelta( iDeltaTimeUpdate obj )
		{
			if( delta.Remove( obj ) )
				cancelled();
			// else
			// ConsoleLogger.logWarning( "Cancel animation did nothing, it wasn't running" );
		}
		/// <summary>Cancel animation</summary>
		public void cancelProgress( iAnimationProgressUpdate obj, bool complete = true )
		{
			if( progress.TryGetValue( obj, out var pa ) )
			{
				if( complete )
					obj.tick( 1 );
				progress.Remove( obj );
				cancelled();
				return;
			}
			// ConsoleLogger.logWarning( "Cancel animation did nothing, it wasn't running" );
		}

		/// <summary>Cancel all animations of an object</summary>
		public void cancel( iAnimationBase obj, bool complete = true )
		{
			if( obj is iAbsoluteTimeUpdate a )
				cancelAbs( a, complete );
			if( obj is iDeltaTimeUpdate d )
				cancelDelta( d );
			if( obj is iAnimationProgressUpdate p )
				cancelProgress( p );
		}

		void cancelled()
		{
			if( !any )
				timers.pause();
		}

		/// <summary>True if the animation is playing</summary>
		public bool playingAbs( iAbsoluteTimeUpdate obj )
		{
			return absolute.TryGetValue( obj, out var unused );
		}
		/// <summary>True if the animation is playing</summary>
		public bool playingDelta( iDeltaTimeUpdate obj )
		{
			return delta.TryGetValue( obj, out var unused );
		}
		/// <summary>True if the animation is playing</summary>
		public bool playingProgress( iAnimationProgressUpdate obj )
		{
			return progress.TryGetValue( obj, out var unused );
		}
		/// <summary>True if any animation is playing</summary>
		public bool any => anyPlaying();

		bool anyPlaying()
		{
			return absolute.Any() || delta.Any() || progress.Any();
		}

		class Animation
		{
			public readonly eAnimationTimer timer;
			public readonly TimeSpan finish;

			public Animation( eAnimationTimer timer, TimeSpan finish )
			{
				this.timer = timer;
				this.finish = finish;
			}

			public Animation( eAnimationTimer timer )
			{
				this.timer = timer;
				finish = TimeSpan.MaxValue;
			}
		}

		class ProgressAnimation
		{
			public readonly eAnimationTimer timer;
			public readonly float progressMul;
			public readonly TimeSpan start;

			public ProgressAnimation( eAnimationTimer timer, TimeSpan now, TimeSpan duration )
			{
				this.timer = timer;
				progressMul = (float)( 1.0 / duration.Ticks );
				start = now;
			}
		}

		readonly ConditionalWeakTable<iAbsoluteTimeUpdate, Animation> absolute = new ConditionalWeakTable<iAbsoluteTimeUpdate, Animation>();
		readonly ConditionalWeakTable<iDeltaTimeUpdate, Animation> delta = new ConditionalWeakTable<iDeltaTimeUpdate, Animation>();
		readonly ConditionalWeakTable<iAnimationProgressUpdate, ProgressAnimation> progress = new ConditionalWeakTable<iAnimationProgressUpdate, ProgressAnimation>();

		readonly List<object> dropList = new List<object>();

		internal void update()
		{
			timers.update();

			bool anyLeft = false;

			foreach( var kvp in absolute )
			{
				TimeSpan now = timers[ kvp.Value.timer ];
				if( now < kvp.Value.finish )
				{
					anyLeft = true;
					kvp.Key.tick( now );
					continue;
				}
				kvp.Key.tick( kvp.Value.finish );
				dropList.Add( kvp.Key );
			}
			foreach( iAbsoluteTimeUpdate d in dropList )
				absolute.Remove( d );
			dropList.Clear();

			foreach( var kvp in delta )
			{
				kvp.Key.tick( timers.delta( kvp.Value.timer ) );

				TimeSpan now = timers[ kvp.Value.timer ];
				if( now < kvp.Value.finish )
				{
					anyLeft = true;
					continue;
				}
				dropList.Add( kvp.Key );
			}
			foreach( iDeltaTimeUpdate d in dropList )
				delta.Remove( d );
			dropList.Clear();

			foreach( var kvp in progress )
			{
				TimeSpan elapsed = timers[ kvp.Value.timer ] - kvp.Value.start;
				float progress = elapsed.Ticks * kvp.Value.progressMul;
				if( progress < 1 )
				{
					anyLeft = true;
					kvp.Key.tick( progress );
					continue;
				}
				kvp.Key.tick( 1 );
				dropList.Add( kvp.Key );
			}
			foreach( iAnimationProgressUpdate d in dropList )
				progress.Remove( d );
			dropList.Clear();

			if( !anyLeft )
				RunPolicy.allAnimationsFinished( content );
		}
	}
}