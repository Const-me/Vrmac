using Diligent.Graphics;
using System;

namespace Vrmac.Draw
{
	/// <summary>An interface to 2D render stuff</summary>
	public interface iDrawContext: IDisposable
	{
		/// <summary>Fill geometry with a brush</summary>
		void fillGeometry( iGeometry geometry, iBrush brush );

		/// <summary>Fill + stroke a geometry</summary>
		void fillAndStroke( iGeometry geometry, iBrush fill, iBrush stroke, float strokeWidth = 1, iStrokeStyle strokeStyle = null );

		/// <summary>Draws the outline of the specified geometry using the specified stroke style.</summary>
		void drawGeometry( iGeometry geometry, iBrush brush, float strokeWidth = 1, iStrokeStyle strokeStyle = null );

		/// <summary>Draw outline of the axis-aligned rectangle</summary>
		void drawRectangle( Rect rect, iBrush brush, float width = 1 );
		/// <summary>Fill rectangle</summary>
		void fillRectangle( Rect rect, iBrush brush );

		/// <summary>Render a sprite</summary>
		void drawSprite( Rect rect, int spriteIndex );

		/// <summary>Measure a block of text, return size in physical pixels</summary>
		CSize measureText( string text, float width, iFont font );
		/// <summary>Render some text</summary>
		void drawText( string text, iFont font, Rect layoutRect, iBrush foreground, iBrush background );

		/// <summary>Measure a block of console-like formatted text, return size in physical pixels</summary>
		CSize measureConsoleText( string text, int widthChars, float fontSize );
		/// <summary>Render some text with console-like formatting: wrapping by fixed characters count.</summary>
		void drawConsoleText( string text, int width, float fontSize, Vector2 position, iBrush foreground, iBrush background );

		/// <summary>Get the 2D device who owns this context</summary>
		iDrawDevice device { get; }

		/// <summary>Manipulates current 2D transformation matrix.</summary>
		/// <remarks>
		/// <para>The matrices stack is reset to default with every <see cref="iDrawDevice.begin" /> call.
		/// In other words, the stack doesn’t accumulate transforms across frames, every frame you’ll get an empty one.</para>
		/// <para>This object manipulates user part of the transform. There’s another part which applies DPI scaling, then transforms pixels into clip space units.
		/// That system-provided transform is immutable, only changes when window is resized or moved.
		/// Eventually in some future version, that system-provided transform will probably include integer matrix for display orientation, portrait / landscape / etc.</para>
		/// </remarks>
		MatrixStack transform { get; }
	}

	/// <summary>Represents a 2D drawing context. You must dispose the interface once finished drawing your 2D graphics in a given frame.</summary>
	public interface iImmediateDrawContext: iDrawContext
	{
		/// <summary>Flush pending draw command. Calling this is optional, Dispose() also does that.</summary>
		void flush();
	}
}