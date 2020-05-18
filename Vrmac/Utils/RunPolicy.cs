using System;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Vrmac.Utils
{
	static class RunPolicy
	{
		static iDispatcher dispatcher => Dispatcher.currentDispatcher.nativeDispatcher;

		[ThreadStatic]
		static readonly ConditionalWeakTable<Context, object> animations = new ConditionalWeakTable<Context, object>();

		public static void animationStarted( Context content )
		{
			animations.AddOrUpdate( content, true );
			dispatcher.runPolicy = eDispatcherRunPolicy.GameStyle;
			content.animation.timers.resume();
		}

		public static void allAnimationsFinished( Context content )
		{
			var a = animations;
			a.Remove( content );
			if( !a.Any() )
				dispatcher.runPolicy = eDispatcherRunPolicy.EnvironmentFriendly;
		}

		public static void minimized()
		{
			dispatcher.runPolicy = eDispatcherRunPolicy.EnvironmentFriendly;
		}

		public static void restored()
		{
			if( animations.Any() )
				dispatcher.runPolicy = eDispatcherRunPolicy.GameStyle;
		}
	}
}