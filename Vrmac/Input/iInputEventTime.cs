using System;
using System.Runtime.CompilerServices;

namespace Vrmac.Input
{
	/// <summary>Interface to receive the time source object</summary>
	public interface iInputEventTime
	{
		/// <summary>Called exactly once, before any input events. Cache the object somewhere, and use it to find the timestamps of messages.</summary>
		void sourceInitialized( iInputEventTimeSource source );
	}

	/// <summary>Adapter to convert milliseconds to DateTime</summary>
	class InputEventTime: iInputEventTimeSource
	{
		TimeSpan iInputEventTimeSource.messageTime => getTime();

		readonly iInput input;
		long offset = 0;
		uint lastReading = 0;

		internal InputEventTime( iInput input )
		{
			this.input = input;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		TimeSpan milliseconds( uint val )
		{
			return TimeSpan.FromTicks( TimeSpan.TicksPerMillisecond * val + offset );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		TimeSpan getTime()
		{
			uint now = input.lastMessageTime;
			if( now < lastReading )
				offset += ( 1L << 32 );
			lastReading = now;
			return milliseconds( now ) ;
		}
	}
}