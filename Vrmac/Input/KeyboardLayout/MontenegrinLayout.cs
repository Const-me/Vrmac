namespace Vrmac.Input.KeyboardLayout
{
	/// <summary>Montenegrin keyboard layout</summary>
	public class MontenegrinLayout: LayoutBase
	{
		static void buildLayout( LayoutBuilder builder, object creationParam )
		{
			bool enhanced = (bool)creationParam;

			builder.symbols( eKey.D1, "1234567890'+", @"!""#$%&/()=?*" );
			builder.key( eKey.Backspace, '\b' );

			builder.key( eKey.Tab, '\t' );

			builder.letters( eKey.Q, "qwertzuiopšđ", "QWERTZUIOPŠĐ" );
			builder.key( eKey.Enter, '\n' );

			builder.letters( eKey.A, "asdfghjklčć", "ASDFGHJKLČĆ" );
			builder.symbol( eKey.Grave, ',', '~' );

			builder.letter( eKey.Backslash, 'ž', 'Ž' );

			builder.letters( eKey.Z, "yxcvbnm", "YXCVBNM" );
			builder.symbols( eKey.Comma, ",.-", ";:_" );

			// Enhancement #1: non-breaking space on ctrl+shift+space
			if( enhanced )
				builder.customCtrlShift( eKey.Space, ' ', '\u00A0' );
			else
				builder.key( eKey.Space, ' ' );

			// Enhancement #2: better dashes on ctrl+alt+(keypad minus) and ctrl+(keypad minus)
			builder.mapKeyPad( enhanced );

			// That line below ain't an enhancement, it's actually how it works here in Windows 10, even in notepad
			builder.bindEuro();
		}

		/// <summary>Create the object</summary>
		public MontenegrinLayout( bool enhanced = true ) :
			base( buildLayout, enhanced )
		{ }
	}
}