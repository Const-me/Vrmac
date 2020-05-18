using System.Runtime.InteropServices;

namespace Vrmac
{
	[StructLayout( LayoutKind.Sequential )]
	struct UInt4
	{
		public uint x, y, z, w;
	}

	[StructLayout( LayoutKind.Sequential )]
	struct Int4
	{
		public int x, y, z, w;
	}
}