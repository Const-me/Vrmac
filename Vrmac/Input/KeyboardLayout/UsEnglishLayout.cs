using System.Globalization;

namespace Vrmac.Input.KeyboardLayout
{
	/// <summary>US English keyboard layout, with a couple optional usability enhancements</summary>
	public class UsEnglishLayout: LayoutBase
	{
		static void buildLayout( LayoutBuilder builder, object creationParam )
		{
			bool enhanced = (bool)creationParam;

			builder.symbols( eKey.D1, "1234567890-=", "!@#$%^&*()_+" );
			builder.key( eKey.Backspace, '\b' );

			builder.key( eKey.Tab, '\t' );

			CultureInfo ci = CultureInfo.InvariantCulture;
			builder.letters( eKey.Q, "qwertyuiop", ci );
			builder.symbols( eKey.LeftBrace, "[]", "{}" );
			builder.key( eKey.Enter, '\n' );

			builder.letters( eKey.A, "asdfghjkl", ci );
			builder.symbols( eKey.Semicolon, ";'`", @":""~" );
			builder.symbol( eKey.Backslash, '\\', '|' );

			builder.letters( eKey.Z, "zxcvbnm", ci );
			builder.symbols( eKey.Comma, ",./", "<>?" );

			// Enhancement #1: non-breaking space on ctrl+shift+space
			if( enhanced )
				builder.customCtrlShift( eKey.Space, ' ', '\u00A0' );
			else
				builder.key( eKey.Space, ' ' );

			// Enhancement #2: better dashes on ctrl+alt+(keypad minus) and ctrl+(keypad minus)
			builder.mapKeyPad( enhanced );
		}

		/// <summary>Create the object</summary>
		public UsEnglishLayout( bool enhanced = true ) :
			base( buildLayout, enhanced )
		{ }
	}
}