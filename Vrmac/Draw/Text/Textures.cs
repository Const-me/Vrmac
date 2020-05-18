using System;

namespace Vrmac.Draw.Text
{
	sealed class Textures
	{
		public readonly TextureAtlas grayscale, cleartype;

		public Textures( Context context, iVrmacDraw factory )
		{
			grayscale = new TextureAtlas( context, factory, eTextureAtlasFormat.R8 );
			cleartype = new TextureAtlas( context, factory, eTextureAtlasFormat.RGBA8 );
		}

		public void subscriveResized( object subscriber, Action act )
		{
			grayscale.resized.add( subscriber, act );
			cleartype.resized.add( subscriber, act );
		}

		public void update()
		{
			grayscale.update();
			cleartype.update();
		}
	}
}