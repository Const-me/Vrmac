using Diligent.Graphics;
using System;
using System.Diagnostics;
using System.IO;

namespace Vrmac.Draw
{
	/// <summary>Wraps iTextureAtlas COM interface into something more C#</summary>
	public sealed class TextureAtlas
	{
		readonly Context context;
		readonly iTextureAtlas atlas;
		// The C++ object already has the same flag. Duplicating here to eliminate native call overhead, it's a single byte of data.
		bool needsUpdate = false;

		internal TextureAtlas( Context context, iVrmacDraw factory, eTextureAtlasFormat format = eTextureAtlasFormat.RGBA8 )
		{
			this.context = context;
			using( var dev = context.device )
				atlas = factory.createTextureAtlas( context.device, format );
		}

		/// <summary>The event is fired when the texture array is recreated due to expands</summary>
		/// <remarks>You likely need to recreate some shader resource bindings when it happens.</remarks>
		public WeakEvent<Action> resized { get; } = new WeakEvent<Action>();

		/// <summary>Add a sprite by decoding a picture</summary>
		public int loadImage( Stream source, eImageFileFormat format )
		{
			int res = atlas.loadImage( source, format );
			Utils.NativeErrorMessages.throwForHR( res );
			needsUpdate = true;
			return res;
		}

		/// <summary>Apply all pending changes to the texture</summary>
		public bool update()
		{
			if( !needsUpdate )
				return false;
			Debug.Assert( atlas.needsUpdate() );

			bool atlasResized;
			using( var dev = context.device )
				atlasResized = atlas.update( dev, context.context );
			needsUpdate = false;

			if( !atlasResized )
				return true;
			foreach( var sub in resized )
				sub();
			return true;
		}

		/// <summary>Get sprite by index.</summary>
		public Sprite this[ int index ]
		{
			get
			{
				atlas.getSprite( index, out ulong rect, out int layer );
				return new Sprite( rect, layer );
			}
		}

		/// <summary>Get glyph sprite by index.</summary>
		/// <remarks>The underlying C++ object has a single `std::vector` of them. This method returns same data as the indexer, just includes the size of the sprite.</remarks>
		public GlyphSprite getGlyph( int index )
		{
			atlas.getSprite( index, out ulong rect, out CSize size, out int layer );
			return new GlyphSprite( ref size, rect, layer );
		}

		/// <summary>Shader resource view of the atlas.</summary>
		/// <remarks>Beware of the expands, this value expires when they happen.</remarks>
		public ITextureView nativeTexture => atlas.nativeTexture;

		internal int addGlyph( FreeType.iFont font )
		{
			int res = font.buildBitmap( atlas );
			Utils.NativeErrorMessages.throwForHR( res );
			needsUpdate = true;
			return res;
		}

		/// <summary>Count of layers in the atlas</summary>
		public int layersCount => atlas.layersCount;

		/// <summary>Size of each layer in pixels</summary>
		public CSize layerSize => atlas.size;

		/// <summary>Size of 1 texel related to size of the texture, in fixed point 1.15 bits. Lower 16 bits of the result contain X size, upper 16 bits have Y size.</summary>
		/// <remarks>The texture array is sized by powers of 2. The initial size is 64×64 by the way, and the maximum is 2048×2048.
		/// That’s how that fixed point representation doesn’t introduce any rounding errors which would otherwise screw up things badly, especially for the text.</remarks>
		public uint pixelSizeInUvUnits => atlas.pixelSizeInUvUnits;
	}
}