using Diligent.Graphics;

namespace Vrmac.Draw.Shaders
{
	// No longer used, left from the very first implementation of VAA
	static class MagicBytesLookupBuffer
	{
		const float mulToRadians = MathHelper.TwoPi / 252;

		static float[] compute()
		{
			float[] result = new float[ 0x200 ];
			for( int i = 0; i < 255; i++ )
			{
				MathHelper.sinCos( i * mulToRadians, out float sin, out float cos );
				result[ i * 2 ] = cos;
				result[ i * 2 + 1 ] = sin;
			}
			return result;
		}

		public static IBuffer create( IRenderDevice dev )
		{
			BufferDesc desc = new BufferDesc( false )
			{
				uiSizeInBytes = 0x200 * 4,
				BindFlags = BindFlags.UniformBuffer,
				Usage = Usage.Static,
			};
			return dev.CreateBuffer( desc, compute(), "MagicBytes lookup" );
		}
	}
}