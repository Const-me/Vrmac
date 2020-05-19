using Diligent.Graphics;
using System;
using System.Numerics;

namespace Vrmac.Draw
{
	/// <summary>Delegate to receive resized event</summary>
	public delegate void ResizedDelegate( Vector2 newSize, double newDpi );

	/// <summary>2D rendering device of Vrmac graphics engine.</summary>
	/// <remarks>The engine contains 2 implementations of this interface. One is windows-only and backed by Direct2D. Another one is cross-platform backed by Diligent, i.e. Direct3D 12 on Windows, GLES 3.1 on Linux.</remarks>
	public interface iDrawDevice: IDisposable
	{
		/// <summary>Create geometry from data in system memory</summary>
		iGeometry createPathGeometry( iPathData data );

		/// <summary>Create rectangular geometry</summary>
		iGeometry createRectangleGeometry( Rect rect );

		/// <summary>Creates an iStrokeStyle that describes features of a stroke</summary>
		iStrokeStyle createStrokeStyle( sStrokeStyle style );

		/// <summary>True if this device uses pre-multiplied alpha for brushes</summary>
		/// <remarks>Microsoft in their Direct2D uses non-premultiplied colors for brushes, even when the context is set up with pre-multiplied alpha blending :-(</remarks>
		/// <seealso href="https://docs.microsoft.com/en-us/windows/win32/direct2d/supported-pixel-formats-and-alpha-modes#about-alpha-modes" />
		bool premultipliedAlphaBrushes { get; }

		/// <summary>Creates a new brush that can be used to paint areas with a solid color.</summary>
		iBrush createSolidColorBrush( Vector4 color );

		/// <summary>Begin drawing. You must dispose the context once finished drawing this frame.</summary>
		iImmediateDrawContext begin( ITextureView rtv, ITextureView dsv, Vector4 clearColor );

		/// <summary>Utility object to scale stuff with dpiScalingFactor, from pixels to units and vice versa.</summary>
		DpiScaling dpiScaling { get; }
		/// <summary>Size of the complete viewport in DPI scaled units</summary>
		Vector2 viewportSize { get; }
		/// <summary>Subscribe to this event to react on user resizing the window, or changing DPI</summary>
		WeakEvent<ResizedDelegate> resized { get; }

		/// <summary>Get RGBA texture atlas used for sprites</summary>
		/// <remarks>The returned object is created on first use</remarks>
		TextureAtlas textureAtlas { get; }

		/// <summary>Get some OS-supplied fonts</summary>
		iFontCollection fontCollection { get; }
	}

	/// <summary>Vrmac backend specific features</summary>
	public interface iVrmacDrawDevice
	{
		/// <summary>Override values passed to vertex clipping algorithm.</summary>
		/// <remarks>The rectangle is in clip space units, the complete viewport is [ -1 .. -1 ] - [ +1 .. +1 ] rectangle.</remarks>
		Rect? clippingRectangleOverride { get; set; }
	}
}