using System;

namespace VrmacVideo.Containers.MP4
{
	public interface iEditList
	{
		long presentationTime( long track );
		long trackTime( long presentation );
	}
}

namespace VrmacVideo.Containers.MP4.EditList
{
	static class Mpeg4EditList
	{
		sealed class Identity: iEditList
		{
			long iEditList.presentationTime( long track ) => track;
			long iEditList.trackTime( long presentation ) => presentation;

			public override string ToString() => "identity";
		}

		sealed class Offset: iEditList
		{
			long offsetValue;

			public Offset( long offsetValue )
			{
				this.offsetValue = offsetValue;
			}

			long iEditList.presentationTime( long track ) => track + offsetValue;
			long iEditList.trackTime( long presentation ) => presentation - offsetValue;

			public override string ToString() => $"offset { offsetValue }";
		}

		public static iEditList create( EditListBox box, uint mediaScale, uint trackScale )
		{
			if( null == box || null == box.entries || box.entries.Length <= 0 )
				return new Identity();

			if( box.entries.Length > 1 )
				throw new NotSupportedException( "The mp4 file has an edit list with more than 1 entry, this is not supported" );
			if( box.mediaRate != 0x10000 )
				throw new NotSupportedException( $"The mp4 file has an edit list with a custom time scale { box.mediaRateDbl }, this is not supported" );

			Entry64 entry;
			if( box.entries is Entry32[] list32 )
			{
				Entry32 e = list32[ 0 ];
				entry.mediaTime = e.mediaTime;
				entry.segmentDuration = e.segmentDuration;
			}
			else if( box.entries is Entry64[] list64 )
				entry = list64[ 0 ];
			else
				throw new ApplicationException( "Unknown entries type" );

			long offsetValue = -entry.mediaTime;
			return new Offset( offsetValue );
		}
	}
}