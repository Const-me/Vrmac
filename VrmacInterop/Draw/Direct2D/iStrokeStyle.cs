using ComLight;
using System;

namespace Vrmac.Direct2D
{
	/// <summary>Describes the caps, miter limit, and line join for a stroke.</summary>
	[ComInterface( "2cd9069d-12e2-11dc-9fed-001143a055f9", eMarshalDirection.ToManaged )]
	public interface iStrokeStyle: Draw.iStrokeStyle
	{
		/// <summary>ID2D1StrokeStyle COM pointer</summary>
		IntPtr getNativePointer();

		/// <summary>Get the parameters</summary>
		void getStrokeStyle( out Draw.sStrokeStyle ss );
	}
}