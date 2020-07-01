using Diligent.Graphics;
using System;
using System.Numerics;
using VrmacVideo;

namespace Vrmac.MediaEngine.Render
{
	struct RectD
	{
		public readonly double left, top, right, bottom;

		public RectD( double left, double top, double right, double bottom )
		{
			this.left = left;
			this.top = top;
			this.right = right;
			this.bottom = bottom;
		}

		public RectD( Vector2D topLeft, Vector2D bottomRight )
		{
			left = topLeft.x;
			top = topLeft.y;
			right = bottomRight.x;
			bottom = bottomRight.y;
		}

		public static RectD operator *( RectD a, RectD b ) =>
			new RectD(
				a.left * b.left,
				a.top * b.top,
				a.right * b.right,
				a.bottom * b.bottom
				);

		public Vector2D relative( Vector2D pt ) =>
			new Vector2D(
				( pt.x - left ) / ( right - left ),
				( pt.y - top ) / ( bottom - top )
				);

		public override string ToString() => $"[ { top }, { left } ] - [ { right }, { bottom } ], size { right - left } × { bottom - top }";
	}

	struct Vector2D
	{
		public readonly double x, y;
		public Vector2D( double a, double b ) { x = a; y = b; }
		public override string ToString() => $"[ { x }, { y } ]";
	}

	static class VertexBuffer
	{
		struct sVideoVertex
		{
			public Vector2 position, texCoords;
		}

		// ( a * b ) / ( c * d )
		static double mulDiv( int a, int b, int c, int d )
		{
			int nom = a * b;
			int den = c * d;
			return (double)nom / (double)den;
		}

		/// <summary>Cropped video rectangle in clip space units</summary>
		static RectD videoRectangle( CSize pxRenderTarget, ref sDecodedVideoSize videoSize )
		{
			CSize pxVideo = videoSize.cropRect.size;
			if( pxVideo.cx * pxRenderTarget.cy >= pxVideo.cy * pxRenderTarget.cx )
			{
				// scale X to fit, center vertically
				double h = mulDiv( pxVideo.cy, pxRenderTarget.cx, pxVideo.cx, pxRenderTarget.cy );
				return new RectD( -1, -h, 1, h );
			}
			else
			{
				// scale Y to fit, center horizontally
				double w = mulDiv( pxVideo.cx, pxRenderTarget.cy, pxVideo.cy, pxRenderTarget.cx );
				return new RectD( -w, -1, w, 1 );
			}
		}

		static CRect computeMargins( ref sDecodedVideoSize videoSize )
		{
			return new CRect( videoSize.cropRect.left,
				videoSize.cropRect.top,
				videoSize.size.cx - videoSize.cropRect.right,
				videoSize.size.cy - videoSize.cropRect.bottom );
		}

		/// <summary>NV12 texture rectangle in clip space units.
		/// NV12 texture is slightly larger than the video 'coz Pi4 h264 decoder ain't smart enough to crop the damn footage on their side.</summary>
		static RectD nv12Rectangle( ref RectD video, ref sDecodedVideoSize videoSize )
		{
			double cx = videoSize.cropRect.size.cx * 0.5;
			double cy = videoSize.cropRect.size.cy * 0.5;
			CRect margins = computeMargins( ref videoSize );

			RectD inflationRel = new RectD(
				( cx + margins.left ) / cx,
				( cy + margins.top ) / cy,
				( cx + margins.right ) / cx,
				( cy + margins.bottom ) / cy
				);

			return video * inflationRel;
		}

		static RectD textureCoordinates( ref RectD nv12 )
		{
			Vector2D topLeft = nv12.relative( new Vector2D( -1, -1 ) );
			Vector2D bottomRight = nv12.relative( new Vector2D( 1, 1 ) );
			return new RectD( topLeft, bottomRight );
		}

		static void produceVertices( Span<sVideoVertex> span, CSize pxRenderTarget, ref sDecodedVideoSize videoSize )
		{
			// Non-trivial amount of arithmetics, hopefully with 64-bit floats the numerical precision won't be too bad as it's pretty critical here.
			// Ideally, need to solve symbolically and copy-paste the solution from Maple solver.
			RectD rc = videoRectangle( pxRenderTarget, ref videoSize );
			rc = nv12Rectangle( ref rc, ref videoSize );
			rc = textureCoordinates( ref rc );

			// Produce the 3 vertices.
			// Positions are set so the triangle covers complete render target.
			// Texture coordinates are set to position video rectangle within the render target.
			span[ 0 ].position = new Vector2( -1, 1 );
			span[ 0 ].texCoords = new Vector2( (float)rc.left, (float)rc.top );

			span[ 1 ].position = new Vector2( 3, 1 );
			span[ 1 ].texCoords = new Vector2( (float)( rc.right * 2 - rc.left ), (float)rc.top );

			span[ 2 ].position = new Vector2( -1, -3 );
			span[ 2 ].texCoords = new Vector2( (float)rc.left, (float)( rc.bottom * 2 - rc.top ) );
		}

		/// <summary>Create immutable VB with the full-screen triangle with cropping-included texture coordinates</summary>
		public static IBuffer createVideoVertexBuffer( IRenderDevice device, CSize renderTargetSize, ref sDecodedVideoSize videoSize )
		{
			Span<sVideoVertex> data = stackalloc sVideoVertex[ 3 ];
			produceVertices( data, renderTargetSize, ref videoSize );

			BufferDesc desc = new BufferDesc( false )
			{
				uiSizeInBytes = 16 * 3,
				BindFlags = BindFlags.VertexBuffer,
				Usage = Usage.Static,
			};

			ReadOnlySpan<sVideoVertex> readOnly = data;
			return device.CreateBuffer( desc, readOnly, "Video VB" );
		}

		/// <summary>Define input layout for the vertex shader</summary>
		public static void setupVideoInputLayout( iPipelineStateFactory stateFactory )
		{
			// Attribute 0 - vertex position
			LayoutElement elt = new LayoutElement( false )
			{
				NumComponents = 2,
				IsNormalized = false
			};
			stateFactory.graphicsLayoutElement( elt );
			// Attribute 1 - texture coordinates
			elt.InputIndex = 1;
			stateFactory.graphicsLayoutElement( elt );
		}

		static double rel( double x, double size )
		{
			return x / size;
		}

		/// <summary>Valid range of UV coordinates; rendering them [ 0 .. 1 ] would produce cropping borders, we don’t want them.</summary>
		/// <remarks>Returns substitution values for HLSL/GLSL, e.g. ( "0.0, 0.0", "1.0, 0.97523452345123235356452363" )</remarks>
		public static (string,string) videoUvCroppedRect( ref sDecodedVideoSize videoSize )
		{
			double x = rel( videoSize.cropRect.left, videoSize.size.cx );
			double y = rel( videoSize.cropRect.top, videoSize.size.cy );
			string minUv = Utils.printFloat2( x, y );

			x = rel( videoSize.cropRect.right, videoSize.size.cx );
			y = rel( videoSize.cropRect.bottom, videoSize.size.cy );
			string maxUv = Utils.printFloat2( x, y );

			return (minUv, maxUv);
		}
	}
}