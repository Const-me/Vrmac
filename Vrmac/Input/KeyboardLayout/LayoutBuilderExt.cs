using System.Linq.Expressions;

namespace Vrmac.Input.KeyboardLayout
{
	/// <summary>A few extension methods of LayoutBuilder</summary>
	public static class LayoutBuilderExt
	{
		/// <summary>Map the keypad area, AFAIK they don't often depend on locale.</summary>
		public static void mapKeyPad( this LayoutBuilder builder, bool enhanced )
		{
			builder.key( eKey.Kpasterisk, '*' );

			builder.keypad( eKey.Kp7, "789" );

			// Various dashes on ctrl+alt+(keypad minus) and ctrl+(keypad minus)
			if( enhanced )
				builder.customCtrlAlt( eKey.Kpminus, '-', '–', '—' );
			else
				builder.key( eKey.Kpminus, '-' );

			builder.keypad( eKey.Kp4, "456" );
			builder.key( eKey.Kpplus, '+' );
			builder.keypad( eKey.Kp1, "1230." );
			builder.key( eKey.Kpslash, '/' );
		}

		/// <summary>Make Ctrl+Alt+E print Euro character.</summary>
		public static void bindEuro( this LayoutBuilder builder )
		{
			builder.remove( eKey.E );

			builder.custom
			(
				eKey.E,
				Expression.Condition
				(
					builder.eControlAltDown,
					Expression.Constant( '€' ),
					Expression.Condition
					(
						builder.eControlOrAltIsDown,
						builder.eNullChar,
						Expression.Condition
						(
							builder.eShiftXorCapsLock,
							Expression.Constant( 'E' ),
							Expression.Constant( 'e' )
						)
					)
				)
			);
		}
	}
}