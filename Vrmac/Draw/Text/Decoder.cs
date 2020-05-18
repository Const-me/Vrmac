using System;
using System.Runtime.CompilerServices;

namespace Vrmac.Draw.Text
{
	/// <summary>Decode UTF16 into UTF32</summary>
	/// <remarks>This structure is used from deep inside <see cref="Kompiler" />-generated code to actually render these strings.</remarks>
	ref struct Decoder
	{
		readonly ReadOnlySpan<char> chars;
		int position;

		public Decoder( ReadOnlySpan<char> span )
		{
			chars = span;
			position = 0;
		}

		/// <summary>Decode UTF32 character, or return uint.MaxValue when reached the end of the string</summary>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public uint nextChar()
		{
			if( position < chars.Length )
			{
				char c = chars[ position ];
				if( !char.IsHighSurrogate( c ) )
				{
					position++;
					return c;
				}
				uint result = (uint)char.ConvertToUtf32( c, chars[ position + 1 ] );
				position += 2;
				return result;
			}
			return uint.MaxValue;
		}
	}
}