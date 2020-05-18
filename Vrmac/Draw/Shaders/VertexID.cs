using System;

namespace Vrmac.Draw.Shaders
{
	static class VertexID
	{
		public static uint vertex( int drawCallSerial )
		{
			return (uint)( drawCallSerial << 8 );
		}
	}
}