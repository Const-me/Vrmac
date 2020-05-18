using ComLight;
using Diligent.Graphics;
using System;
using System.Runtime.InteropServices;

namespace Vrmac.Direct2D
{
	/// <summary>Defines an object that paints an area. Interfaces that derive from iBrush describe how the area is painted.</summary>
	[ComInterface( "2cd906a8-12e2-11dc-9fed-001143a055f9", eMarshalDirection.ToManaged )]
	public interface iBrush: Draw.iBrush
	{
		/// <summary>ID2D1Brush COM pointer</summary>
		IntPtr getNativePointer();
	}

	/// <summary>Paints an area with a solid color.</summary>
	[ComInterface( "2cd906a9-12e2-11dc-9fed-001143a055f9", eMarshalDirection.ToManaged )]
	public interface iSolidColorBrush: iBrush
	{
		/// <summary>ID2D1SolidColorBrush COM pointer</summary>
		new IntPtr getNativePointer();

		/// <summary>Specifies Retrieves the color of the solid color brush</summary>
		void setColor( [In] ref Vector4 color );
		/// <summary>Retrieves the color of the solid color brush</summary>
		void getColor( out Vector4 color );

		/// <summary>The color of the solid color brush</summary>
		Vector4 color { get; set; }
	}
}