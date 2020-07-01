using System;
using System.Diagnostics;
using System.Text;

namespace VrmacVideo.Containers.MP4
{
	public struct MediaHandler
	{
		public readonly eMediaHandler mediaHandler;
		public readonly string name;

		internal MediaHandler( Mp4Reader reader )
		{
			Debug.Assert( reader.currentBox == eBoxType.hdlr );

			// 8.4.3.2
			int cb = checked((int)reader.remainingBytes);
			Span<byte> data = stackalloc byte[ cb ];
			reader.read( data );

			uint handlerType = BitConverter.ToUInt32( data.Slice( 8 ) );
			mediaHandler = (eMediaHandler)handlerType;

			ReadOnlySpan<byte> utf8 = data.Slice( 8 + 4 + 4 * 3 );

			// Trim the null
			for( int i = 0; i < utf8.Length; i++ )
				if( 0 == utf8[ i ] )
				{
					utf8 = utf8.Slice( 0, i );
					break;
				}

			name = Encoding.UTF8.GetString( utf8 );
		}
	}
}