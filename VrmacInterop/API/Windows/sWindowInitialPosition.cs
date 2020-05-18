using System.Runtime.InteropServices;

namespace Vrmac
{
	/// <summary>Controls window position on a desktop.</summary>
	[StructLayout( LayoutKind.Sequential )]
	public struct sWindowInitialPosition
	{
		/// <summary>Rectangle for restored state</summary>
		public CRect restoredRect;
		/// <summary>Minimum size of the window’s client rectangle</summary>
		public CSize minimumSize;
		/// <summary>Controls how the window is shown</summary>
		public eShowWindow show;
	}
}