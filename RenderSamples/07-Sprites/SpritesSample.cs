using Diligent.Graphics;
using System;
using System.Linq;
using System.Numerics;
using Vrmac;
using Vrmac.Animation;
using Vrmac.Draw;

namespace RenderSamples
{
	class SpritesSample: SampleBase, iDeltaTimeUpdate
	{
		const string resourceFolder = "RenderSamples/07-Sprites";

		static readonly Vector4 background = Color.parse( "#777" );

		const int spritesCount = 14;
		readonly Vector2[] randomVertices, randomSpeeds;
		Vector2 speedMultiplier;
		Rect rcRandom;

		protected override void render( ITextureView swapChainRgb, ITextureView swapChainDepthStencil )
		{
			Rect rcSprite = new Rect( 0, 0, 32, 32 );
			using( var dc = context.drawDevice.begin( swapChainRgb, swapChainDepthStencil, background ) )
			{
				Rect rc = new Rect( rcRandom.bottomRight, rcRandom.bottomRight - new Vector2( 256 ) );
				dc.drawSprite( rc, 1 );

				rc = new Rect( rcRandom.topLeft, rcRandom.topLeft + new Vector2( 256 ) );
				dc.drawSprite( rc, 0 );

				for( int i = 0; i < spritesCount; i++ )
				{
					Matrix3x2 imageRotation = Matrix3x2.CreateRotation( rotationAngle );
					Vector2 v = randomVertices[ i ];
					v = rcRandom.getPoint( v );
					Matrix3x2 trans = Matrix3x2.CreateTranslation( v );
					dc.transform.push( imageRotation * trans );
					dc.drawSprite( rcSprite, ( i % 3 ) + 2 );
					dc.transform.pop();
				}
			}
		}

		static int loadSprite( TextureAtlas atlas, iStorageFolder folder, string name, eImageFileFormat format = eImageFileFormat.PNG )
		{
			folder.openRead( name, out var stream );
			using( stream )
				return atlas.loadImage( stream, format );
		}

		public SpritesSample()
		{
			var rnd = new Random( 42 );
			randomVertices = Enumerable.Range( 0, spritesCount )
				.Select( i => new Vector2( (float)rnd.NextDouble(), (float)rnd.NextDouble() ) )
				.ToArray();

			randomSpeeds = Enumerable.Range( 0, spritesCount )
				.Select( i => new Vector2( (float)rnd.NextDouble(), (float)rnd.NextDouble() ) )
				.Select( v => v.normalized() )
				.ToArray();
		}

		protected override void createResources( IRenderDevice device )
		{
			var dev = context.drawDevice;
			var atlas = dev.textureAtlas;
			var assets = StorageFolder.embeddedResources( System.Reflection.Assembly.GetExecutingAssembly(), resourceFolder );

			// Adding larger images first for better atlas packing
			loadSprite( atlas, assets, "Lenna.jpg", eImageFileFormat.JPEG );
			assets.openRead( "beans.tif.gz", out var tiff );
			using( tiff )
				atlas.loadImage( tiff, eImageFileFormat.TIFF );

			loadSprite( atlas, assets, "1.png" );
			loadSprite( atlas, assets, "2.png" );
			loadSprite( atlas, assets, "3.png" );

			onResized( dev.viewportSize, context.dpiScalingFactor );
			dev.resized.add( this, onResized );
			context.animation.startDelta( this );
		}

		const float rotationSpeed = MathF.PI / 11;
		Angle rotationAngle = new Angle();

		void onResized( Vector2 size, double dpi )
		{
			Rect rc = new Rect( Vector2.Zero, size );
			rcRandom = rc.deflate( 40, 20 );
			Vector2 rcRandomSize = rcRandom.size.normalized();
			speedMultiplier = new Vector2( 0.0125f ) / rcRandomSize;
		}

		void iDeltaTimeUpdate.tick( float elapsedSeconds )
		{
			rotationAngle.rotate( rotationSpeed, elapsedSeconds );

			for( int i = 0; i < spritesCount; i++ )
			{
				Vector2 v = randomVertices[ i ];
				Vector2 speed = randomSpeeds[ i ];
				v += speed * speedMultiplier * elapsedSeconds;
				randomVertices[ i ] = v;
				if( v.X < 0 || v.X > 1 )
					speed.X = -speed.X;
				if( v.Y < 0 || v.Y > 1 )
					speed.Y = -speed.Y;
				randomSpeeds[ i ] = speed;
			}
		}
	}
}