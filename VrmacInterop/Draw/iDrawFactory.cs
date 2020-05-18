using ComLight;
using System;

namespace Vrmac.Draw
{
	/// <summary>Factory object for 2D drawing stuff</summary>
	[ComInterface( "31c53b12-d9c9-43f4-ae3b-89c6ebdbabea", eMarshalDirection.ToManaged )]
	public interface iDrawFactory: IDisposable
	{
		/// <summary>Create Direct2D device and context. This method is Windows-only.</summary>
		/// <remarks>The returned object is dynamically allocated and retains huge pile of GPU resources: D3D11-on-12 device and context, a D2D bitmap per back buffer, and more.</remarks>
		[RetValIndex]
		Direct2D.iDrawDevice createD2dDevice( iDiligentWindow window );

		/// <summary>Returns utility object for dealing with 2D drawing stuff which works on all supported platforms.</summary>
		/// <remarks>The returned object is statically allocated and doesn't own anything, no need to dispose it.</remarks>
		[RetValIndex]
		iVrmacDraw createVrmacDraw();
	}
}