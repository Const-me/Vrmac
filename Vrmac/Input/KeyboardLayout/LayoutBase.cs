using Vrmac.Input.Linux;
using System;

namespace Vrmac.Input.KeyboardLayout
{
	/// <summary>Base class for keyboard layouts</summary>
	public abstract class LayoutBase: iKeyboardLayout
	{
		/// <summary>State of some buttons and LEDs</summary>
		public eKeyboardState state { get; private set; } = eKeyboardState.None;

		readonly Func<eKey, char> m_keymap;
		char iKeyboardLayout.keyChar( eKey key ) => m_keymap( key );

		/// <summary>Compile the layout</summary>
		protected LayoutBase( Action<LayoutBuilder, object> buildLayout, object creationParam = null )
		{
			LayoutBuilder builder = new LayoutBuilder( this );
			buildLayout( builder, creationParam );
			m_keymap = builder.compile();
		}

		void updateState( eKeyboardState bit, eKeyValue keyValue )
		{
			switch( keyValue )
			{
				case eKeyValue.Pressed:
					state |= bit;
					return;
				case eKeyValue.Released:
					state &= ~bit;
					return;
			}
		}

		void iKeyboardLayout.updateState( eKey key, eKeyValue val )
		{
			switch( key )
			{
				case eKey.LeftShift:
				case eKey.RightShift:
					updateState( eKeyboardState.ShiftDown, val );
					return;
				case eKey.LeftCtrl:
				case eKey.RightCtrl:
					updateState( eKeyboardState.ControlDown, val );
					return;
				case eKey.LeftAlt:
				case eKey.RightAlt:
					updateState( eKeyboardState.AltDown, val );
					return;
			}
		}

		void iKeyboardLayout.updateState( eLed led, int value )
		{
			switch( led )
			{
				case eLed.CapsLock:
					if( 0 != value )
						state |= eKeyboardState.CapsLock;
					else
						state &= ( ~eKeyboardState.CapsLock );
					return;
				case eLed.NumLock:
					if( 0 != value )
						state |= eKeyboardState.NumLock;
					else
						state &= ~eKeyboardState.NumLock;
					return;
			}
		}
	}
}