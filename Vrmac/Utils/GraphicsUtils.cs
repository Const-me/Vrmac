using Diligent.Graphics;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using Vrmac.Draw;
using Vrmac.Utils;

namespace Vrmac
{
	/// <summary>Miscellaneous graphic utilities</summary>
	public static class GraphicsUtils
	{
		/// <summary>Setup premultiplied alpha blending on the first render target</summary>
		public static void premultipliedAlphaBlending( ref this PipelineStateDesc psoDesc )
		{
			RenderTargetBlendDesc blendDesc = new RenderTargetBlendDesc( false )
			{
				BlendEnable = true,
				SrcBlend = BlendFactor.One,
				DestBlend = BlendFactor.InvSrcAlpha,
			};
			psoDesc.GraphicsPipeline.BlendDesc.setRenderTarget( blendDesc );
		}

		/// <summary>Setup premultiplied alpha blending on the first render target</summary>
		public static void premultipliedBlendingMaxAlpha( ref this PipelineStateDesc psoDesc )
		{
			RenderTargetBlendDesc blendDesc = new RenderTargetBlendDesc( false )
			{
				BlendEnable = true,
				SrcBlend = BlendFactor.One,
				DestBlend = BlendFactor.InvSrcAlpha,
				SrcBlendAlpha = BlendFactor.One,
				DestBlendAlpha = BlendFactor.One,
				BlendOpAlpha = BlendOperation.Max,
			};
			psoDesc.GraphicsPipeline.BlendDesc.setRenderTarget( blendDesc );
		}

		/// <summary>Setup the pipeline state for rendering to immediate context: exactly 1 render target, and the formats matching the context.</summary>
		public static void setBufferFormats( ref this PipelineStateDesc psoDesc, Context context )
		{
			var formats = context.swapChainFormats;
			psoDesc.GraphicsPipeline.NumRenderTargets = 1;
			psoDesc.GraphicsPipeline.setRTVFormat( 0, formats.color );
			psoDesc.GraphicsPipeline.DSVFormat = formats.depth;
		}

		// Not a memory leak because that particular COM object is statically allocated, AddRef and Release do nothing.
		static iSimdUtils g_utils;

		internal static void engineCreated( iGraphicsEngine engine )
		{
			g_utils = engine.simdUtils;
		}

		/// <summary>Premultiply alpha channel. The array is assumed to be RGBA or BGRA, i.e. the most significant byte of each uint is the alpha.</summary>
		public static void premultiplyAlpha( uint[] pixels, bool flipBgrRgb )
		{
			g_utils.premultiplyAlpha( pixels, pixels.Length, flipBgrRgb );
		}

		/// <summary>Swap red and blue channels in a 32-bit image, making RGBA from BGRA, or vice versa.</summary>
		public static void swapRedBlueChannels( uint[] pixels )
		{
			g_utils.flipBgrRgb( pixels, pixels.Length );
		}

		/// <summary>Convert 4-lanes 32-bit float vector to 16-bit floating point format</summary>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static ulong fp16( ref Vector4 fp32 )
		{
			g_utils.fp16( ref fp32, out ulong res );
			return res;
		}

		/// <summary>Convert 4-lanes 32-bit float vector to 16-bit floating point format</summary>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static uint fp16( ref Vector2 fp32 )
		{
			int res = g_utils.fp16( ref fp32 );
			return unchecked((uint)res);
		}

		/// <summary>Convert 4-lanes 16-bit float vector to 32-bit floating point format</summary>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static Vector4 fp32( ulong f16 )
		{
			g_utils.fp32( f16, out var vec4 );
			return vec4;
		}

		/// <summary>Linearly interpolate between 2 FP16 vectors, return result also in FP16</summary>
		/// <remarks>Neither AMD64 nor NEON support float16 arithmetic.
		/// Both support conversations between FP16 and FP32, _mm_cvtph_ps _mm_cvtps_ph on PC, vcvt_f32_f16 vcvt_f16_f32 on Linux.
		/// Also, both support 4-wide 32-bit float FMA, _mm_fmadd_ps / vmlaq_f32.
		/// Still, I’m not 100% sure it was good idea to do in C++.
		/// Too few instructions in the native method, just 16 instructions on PC, 21 on ARM. fp16 and fp32 functions are even smaller, just a few instructions each.
		/// pinvoke overhead might be larger than win from SIMD versus bit tricks in C# with <see cref="BitConverter.SingleToInt32Bits(float)" />.</remarks>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static ulong fp16Lerp( ulong x, ulong y, float s )
		{
			g_utils.fp16Lerp( x, y, s, out ulong res );
			return res;
		}

		/// <summary>Write glyph vertices with the specified offset</summary>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static void offsetGlyphs( Span<sVertexWithId> dest, ReadOnlySpan<sGlyphVertex> source, CPoint offsetValue )
		{
			g_utils.offsetGlyphs( dest, source, offsetValue );
		}

		/// <summary>Encode mapped texture to PNG</summary>
		public static void encodeRgbaPng( CSize size, MappedTextureSubresource mapped, ePngOptions options, Stream destStream )
		{
			// For performance reason iSimdUtils C# interface class lacks [CustomConventions] as it causes couple extra instructions per call.
			// Unlike these SIMD routines, PNG encoder can, and does, fail, like any other IO.
			// Marshalling errors manually here, normally both NativeErrorMessages calls are embedded in the runtime generated marshalling code.
			NativeErrorMessages.prologue();
			int hr = g_utils.encodeRgbaPng( ref size, ref mapped, options, destStream );
			NativeErrorMessages.throwForHR( hr );
		}

		/// <summary>Save a screenshot</summary>
		public static void saveScreenshot( this Context context, string path )
		{
			context.screenshotLocation = path;
			context.queueRenderFrame();
		}
	}
}