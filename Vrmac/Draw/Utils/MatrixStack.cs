using System;
using System.Collections.Generic;
using System.Numerics;

namespace Vrmac.Draw
{
	/// <summary>A stack of 2D transformation matrices</summary>
	public sealed class MatrixStack
	{
		readonly Matrix3x2 root;
		readonly Stack<Matrix3x2> stack = new Stack<Matrix3x2>();

		/// <summary>Construct with identity root transform</summary>
		public MatrixStack()
		{
			root = Matrix3x2.Identity;
		}

		/// <summary>The current transform: either top of the stack, or the root one that was passed to constructor</summary>
		public Matrix3x2 current
		{
			get
			{
				if( stack.TryPeek( out var m ) )
					return m;
				return root;
			}
		}

		/// <summary>Multiply the current matrix by the provided one, and push the product to the stack</summary>
		public void push( Matrix3x2 matrix )
		{
			Matrix3x2 m = matrix * current;
			stack.Push( m );
			if( matrix != Matrix3x2.Identity )
				changed = true;
		}

		/// <summary>Push identity matrix, undoing whatever transforms were applied.</summary>
		public void pushIdentity()
		{
			stack.Push( Matrix3x2.Identity );
			if( stack.Count > 1 )
				changed = true;
		}

		/// <summary>Pop matrix from the stack, restoring the transform</summary>
		public void pop()
		{
			Matrix3x2 m = stack.Pop();
			changed = true;
		}

		/// <summary>Utility structure implementing IDisposable to restore state of the stack</summary>
		public struct TForm: IDisposable
		{
			readonly Stack<Matrix3x2> stack;
			readonly int trimToCount;

			internal TForm( Stack<Matrix3x2> stack, int oldCount )
			{
				this.stack = stack;
				trimToCount = oldCount;
			}
			/// <summary>Restores the previous state of the stack</summary>
			public void Dispose()
			{
				while( stack.Count > trimToCount )
					stack.Pop();
			}
		}

		/// <summary>Push a matrix, return a structure that will restore the old one once disposed.</summary>
		public TForm transform( Matrix3x2 matrix )
		{
			int count = stack.Count;
			push( matrix );
			return new TForm( stack, count );
		}

		// Clear the whole stack. Called from within iDrawDevice.begin, i.e. the stack doesn't accumulate transforms across frames.
		internal void clear()
		{
			if( stack.Count <= 0 )
				return;
			stack.Clear();
			changed = true;
		}

		internal bool changed { get; private set; } = false;

		internal void clearChanged() => changed = false;
	}
}