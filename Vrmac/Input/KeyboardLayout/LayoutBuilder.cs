using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;

namespace Vrmac.Input.KeyboardLayout
{
	/// <summary>Utility class to compile keyboard layouts</summary>
	/// <remarks>Until it’s compiled, the data being built is in LINQ expressions. Once compiled it stops being data, becomes code.</remarks>
	public class LayoutBuilder
	{
		readonly SortedDictionary<eKey, SwitchCase> map = new SortedDictionary<eKey, SwitchCase>();

		/// <summary>Expression for current keyboard state, casted to integer</summary>
		public readonly Expression eStateInt;
		/// <summary>0 != ( state &amp; eKeyboardState.ShiftDown )</summary>
		public readonly Expression eShift;

		// <summary>0 != ( state &amp; ( eKeyboardState.ShiftDown | eKeyboardState.CapsLock ) )</summary>
		// readonly Expression eShiftOrCapsLock;

		/// <summary>Evaluates to true when either shift or capslock are set, but not both.</summary>
		public readonly Expression eShiftXorCapsLock;
		/// <summary>Evaluates to true when numlock LED is lit and shift is released</summary>
		public readonly Expression eNumLockAndNotShift;

		/// <summary>Evaluates to true when control and alt are both down, but the shift is up.</summary>
		public readonly Expression eControlAltDown;

		/// <summary>Evaluates to true when either control or alt are down</summary>
		public readonly Expression eControlOrAltIsDown;

		/// <summary>Evaluates to true when control is down</summary>
		public readonly Expression eControlDown;

		/// <summary>Return target of the function being built</summary>
		readonly LabelTarget returnTarget;
		/// <summary>'\0' constant</summary>
		public readonly Expression eNullChar = Expression.Constant( '\0' );

		/// <summary>For performance reason, "this" pointer is compiled as a constant into the code.</summary>
		public LayoutBuilder( LayoutBase thisPointer )
		{
			var eThis = Expression.Constant( thisPointer );
			var piState = typeof( LayoutBase ).GetProperty( "state" );
			Expression eStateEnum = Expression.Property( eThis, piState.GetGetMethod() );
			eStateInt = Expression.Convert( eStateEnum, typeof( int ) );

			Expression eZeroInt = Expression.Constant( 0, typeof( int ) );

			eShift = Expression.NotEqual
			(
				eZeroInt,
				Expression.And
				(
					eStateInt,
					Expression.Constant( (int)eKeyboardState.ShiftDown )
				)
			);

			/* eShiftOrCapsLock = Expression.NotEqual
			(
				eZeroInt,
				Expression.And
				(
					eStateInt,
					Expression.Constant( (int)( eKeyboardState.ShiftDown | eKeyboardState.CapsLock ) )
				)
			); */

			//    ( state ^ ( state >> 4 ) )  &  1  !=  0
			Debug.Assert( ( (int)eKeyboardState.CapsLock >> 4 ) == (int)eKeyboardState.ShiftDown );
			eShiftXorCapsLock = Expression.NotEqual
			(
				eZeroInt,
				Expression.And
				(
					Expression.Constant( 1 ),
					Expression.ExclusiveOr( eStateInt,
						Expression.RightShift( eStateInt, Expression.Constant( 4 ) ) )
				)
			);

			eNumLockAndNotShift = Expression.Equal
			(
				Expression.Constant( (int)eKeyboardState.NumLock ),
				Expression.And
				(
					eStateInt,
					Expression.Constant( (int)( eKeyboardState.NumLock | eKeyboardState.ShiftDown ) )
				)
			);

			Expression cControlAlt = Expression.Constant( (int)( eKeyboardState.ControlDown | eKeyboardState.AltDown ) );
			Expression cControlAltShift = Expression.Constant( (int)( eKeyboardState.ControlDown | eKeyboardState.AltDown | eKeyboardState.ShiftDown ) );
			eControlAltDown = Expression.Equal( cControlAlt, Expression.And( eStateInt, cControlAltShift ) );
			eControlDown = Expression.NotEqual(
				Expression.Constant( 0 ),
				Expression.And( eStateInt,
					Expression.Constant( (int)eKeyboardState.ControlDown ) ) );

			eControlOrAltIsDown = Expression.NotEqual
			(
				eZeroInt,
				Expression.And
				(
					eStateInt,
					Expression.Constant( (int)( eKeyboardState.ControlDown | eKeyboardState.AltDown ) )
				)
			);

			returnTarget = Expression.Label( typeof( char ) );
		}

		/// <summary>Use a custom expression. The expression must take no parameters, and return char.</summary>
		public void custom( eKey key, Expression keyChar )
		{
			Expression body = Expression.Return( returnTarget, keyChar );
			SwitchCase sc = Expression.SwitchCase( body, Expression.Constant( key, typeof( eKey ) ) );
			map.Add( key, sc );
		}

		/// <summary>Map a key that doesn’t depend on the state, such as enter</summary>
		public void key( eKey k, char c, bool ignoreControlAlt = false )
		{
			if( ignoreControlAlt )
			{
				custom( k, Expression.Constant( c ) );
				return;
			}
			custom( k, Expression.Condition( eControlOrAltIsDown, eNullChar, Expression.Constant( c ) ) );
		}

		/// <summary>Map a key that depends on shift but ignores caps lock, such as 1</summary>
		public void symbol( eKey k, char normal, char shiftDown )
		{
			custom( k,
				Expression.Condition
				(
					eControlOrAltIsDown,
					eNullChar,
					Expression.Condition
					(
						eShift,
						Expression.Constant( shiftDown ),
						Expression.Constant( normal )
					)
				)
			);
		}

		/// <summary>Map a continuous range of keys which depend on shift but ignore caps lock, such as 1</summary>
		public void symbols( eKey kFirst, string normal, string shiftDown )
		{
			if( normal.Length != shiftDown.Length )
				throw new ArgumentException();
			for( int i = 0; i < normal.Length; i++ )
				symbol( (eKey)( (ushort)kFirst + i ), normal[ i ], shiftDown[ i ] );
		}

		/// <summary>Map a key that changes with either shift or caps lock, such as Q</summary>
		public void letter( eKey k, char normal, char upperCase )
		{
			custom( k,
				Expression.Condition
				(
					eControlOrAltIsDown,
					eNullChar,
					Expression.Condition
					(
						eShiftXorCapsLock,
						Expression.Constant( upperCase ),
						Expression.Constant( normal )
					)
				)
			);
		}

		/// <summary>Map a continuous range of keys which change with either shift or caps lock</summary>
		public void letters( eKey kFirst, string normal, string upperCase )
		{
			if( normal.Length != upperCase.Length )
				throw new ArgumentException();
			for( int i = 0; i < normal.Length; i++ )
				letter( (eKey)( (ushort)kFirst + i ), normal[ i ], upperCase[ i ] );
		}

		/// <summary>Map a continuous range of keys which change with either shift or caps lock, using provided culture info to convert to uppercase / lowercase</summary>
		public void letters( eKey kFirst, string chars, CultureInfo ci )
		{
			letters( kFirst, chars.ToLower( ci ), chars.ToUpper( ci ) );
		}

		/// <summary>Map a key that only produces character when numlock is lit</summary>
		public void keypad( eKey k, char digit )
		{
			custom( k, Expression.Condition( eNumLockAndNotShift, Expression.Constant( digit ), eNullChar ) );
		}

		/// <summary>Map a continuous range of keypad keys</summary>
		public void keypad( eKey kFirst, string digits )
		{
			for( int i = 0; i < digits.Length; i++ )
				keypad( (eKey)( (ushort)kFirst + i ), digits[ i ] );
		}

		/// <summary>Map a key so it produces different characters based on whether Ctrl+Shift are both down</summary>
		public void customCtrlShift( eKey k, char normal, char ctrlShift )
		{
			Expression cControlShift = Expression.Constant( (int)( eKeyboardState.ControlDown | eKeyboardState.ShiftDown ) );
			Expression eControlShiftDown = Expression.Equal( cControlShift, Expression.And( eStateInt, cControlShift ) );
			custom( k,
				Expression.Condition
				(
					eControlShiftDown,
					Expression.Constant( ctrlShift ),
					Expression.Constant( normal )
				) );
		}

		/// <summary>Map a key so it produces different characters when Ctrl or Ctrl+Shift are down</summary>
		public void customCtrlAlt( eKey k, char normal, char ctrl, char ctrlAlt )
		{
			custom( k,
				Expression.Condition
				(
					eControlAltDown,
					Expression.Constant( ctrlAlt ),
					Expression.Condition
					(
						eControlDown,
						Expression.Constant( ctrl ),
						Expression.Constant( normal )
					)
				) );
		}

		/// <summary>Remove a key mapping from the builder</summary>
		public bool remove( eKey k )
		{
			return map.Remove( k );
		}

		/// <summary>Compile the keyboard layout into MSIL; the JIT compiler will take it from there.</summary>
		public Func<eKey, char> compile()
		{
			// Compile the main switch expression with all the keys of the layout.
			var arg = Expression.Parameter( typeof( eKey ), "key" );
			var eSwitch = Expression.Switch( arg, map.Values.ToArray() );
			var block = Expression.Block( eSwitch, Expression.Label( returnTarget, eNullChar ) );
			return Expression.Lambda<Func<eKey, char>>( block, arg ).Compile();
		}
	}
}