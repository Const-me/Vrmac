using System;

namespace Vrmac.Draw.Text
{
	enum eBreakAction: byte
	{
		// Do nothing
		None = 0,
		// Flush buffer and move to the new line
		LineFeed = 1,
		// Flush buffer shifting last word of it to the new line
		MoveLastWord = 2,
	}

	struct LineBreaker
	{
		[Flags]
		enum eLineState: byte
		{
			None = 0,
			HaveGlyphs = 1,
			HaveWords = 2,
			Overflow = 4,
		}

		readonly ushort lineBreakPosition;
		eLineState state;

		public LineBreaker( int right )
		{
			lineBreakPosition = checked((ushort)right);
			state = eLineState.None;
		}

		public eBreakAction whitespace()
		{
			if( state == eLineState.Overflow )
			{
				state = eLineState.None;
				return eBreakAction.LineFeed;
			}

			if( state.HasFlag( eLineState.HaveGlyphs ) )
				state = eLineState.HaveWords;

			return eBreakAction.None;
		}

		public eBreakAction newline()
		{
			state = eLineState.None;
			return eBreakAction.LineFeed;
		}

		public eBreakAction glyph( int x )
		{
			if( state == eLineState.Overflow )
				return eBreakAction.None;

			if( x < lineBreakPosition )
			{
				state |= eLineState.HaveGlyphs;
				return eBreakAction.None;
			}

			if( state.HasFlag( eLineState.HaveWords ) )
			{
				state = eLineState.HaveGlyphs;
				return eBreakAction.MoveLastWord;
			}

			state = eLineState.Overflow;
			return eBreakAction.None;
		}
	}
}