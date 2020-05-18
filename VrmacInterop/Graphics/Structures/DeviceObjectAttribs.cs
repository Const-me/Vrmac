using System;
using System.Runtime.InteropServices;

namespace Diligent.Graphics
{
	/// <summary>Base structure for GPU object attributes</summary>
	[StructLayout( LayoutKind.Sequential )]
	public struct DeviceObjectAttribs
	{
		IntPtr m_name;
		/// <summary>Object name</summary>
		public string Name => Marshal.PtrToStringUTF8( m_name );

		/// <summary>Create and set fields to their defaults</summary>
		public DeviceObjectAttribs( bool unused )
		{
			m_name = IntPtr.Zero;
		}
	}
}