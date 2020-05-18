using System.Runtime.CompilerServices;
using Vrmac.Draw.Shaders;
using Vrmac.Draw.Text;

namespace Vrmac.Draw.Main
{
	struct DrawMeshes
	{
		/// <summary>Meshes pending from the tessellator</summary>
		public readonly Buffer<sPendingDrawCall> meshes;

		// iDrawContext.drawRectangle commands are special, they don't have any mesh objects, the vertex / index buffers are procedurally generated on the fly.
		// This is of course for performance reasons. If you have 100 rectangles in the scene, you don't want to allocate any objects per rectangle, at 60 FPS would cause 6000 pieces of garbage per second.
		// .NET GC is fast, but on Raspberry Pi you will notice performance impact of that.
		// Using this list of structures instead. They are value types, don't cause any allocations to add or clear.
		public readonly Buffer<sDrawRectCommand> rectCommands;

		public readonly Buffer<sDrawSpriteCommand> spriteCommands;

		public readonly Buffer<sDrawTextCommand> textCommands;

		public DrawMeshes( bool unused )
		{
			meshes = new Buffer<sPendingDrawCall>( 32 );
			rectCommands = new Buffer<sDrawRectCommand>( 32 );
			spriteCommands = new Buffer<sDrawSpriteCommand>( 32 );
			textCommands = new Buffer<sDrawTextCommand>( 32 );
		}

		public void clear()
		{
			meshes.clear();
			rectCommands.clear();
			spriteCommands.clear();
			textCommands.clear();
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public Order addRectangle( ref int z, ref Rect rectangle, float? width )
		{
			Order result = new Order( -1 - rectCommands.length, z );
			z++;

			var cmd = new sDrawRectCommand() { rect = rectangle, strokeWidth = width };
			rectCommands.add( ref cmd );
			return result;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public Order addSprite( ref int z, ref Rect rectangle, ref Sprite uv )
		{
			Order result = new Order( -1 - spriteCommands.length, z );
			z++;

			var cmd = new sDrawSpriteCommand() { rect = rectangle, sprite = uv };
			spriteCommands.add( ref cmd );
			return result;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public Order addText( ref int z, string text, Font font, ref CRect rect, int solidColor, eTextRendering textRendering, int? consoleWidth = null )
		{
			Order result = new Order( -1 - textCommands.length, z );
			z++;

			var cmd = new sDrawTextCommand();
			cmd.rectangle = rect;
			cmd.foreground = solidColor;
			cmd.font = font;
			cmd.text = text;
			cmd.textRendering = textRendering;
			cmd.meshDataSize = font.prepareGlyphs( text, textRendering );
			cmd.consoleWidth = consoleWidth;
			textCommands.add( ref cmd );
			return result;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public void summarize( ref eRenderPassFlags renderPasses, out eShaderMacros drawFeatures )
		{
			eRenderPassFlags flags = eRenderPassFlags.None;
			eShaderMacros macros = eShaderMacros.None;

			foreach( var m in meshes.read() )
				flags |= m.mesh.drawInfo.renderPassFlags;

			if( rectCommands.length > 0 )
				flags |= eRenderPassFlags.Opaque;

			if( spriteCommands.length > 0 )
			{
				flags |= eRenderPassFlags.Transparent;
				macros |= eShaderMacros.TextureAtlas;
			}

			if( textCommands.length > 0 )
			{
				macros |= eShaderMacros.TextRendering;
				flags |= eRenderPassFlags.Transparent;
			}

			renderPasses |= flags;
			drawFeatures = macros;
		}
	}
}