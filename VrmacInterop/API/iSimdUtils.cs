using ComLight;
using Diligent.Graphics;
using System;
using System.IO;
using System.Numerics;
using System.Runtime.InteropServices;
using Vrmac.Draw;

namespace Vrmac.Utils
{
	/// <summary>Flags for LibPNG encoder</summary>
	[Flags]
	public enum ePngOptions: byte
	{
		/// <summary>None of the below</summary>
		None = 0,
		/// <summary>Flip BGR into RGB order</summary>
		FlipChannels = 1,
		/// <summary>Include a metadata packet which says the PNG contains SRGB data. Doesn't affect image data, only the file.</summary>
		SrgbMetadata = 2,
		/// <summary>Flip image vertically in the process</summary>
		FlipRows = 4,
	}

	/// <summary>Utility object with a couple manually optimized SIMD routines.</summary>
	/// <remarks>SIMD is available in .NET Core 3.0, but it's way easier to use in C++.
	/// Also I have doubts about NEON support in .NET, while GCC support for NEON intrinsics is awesome.</remarks>
	[ComInterface( "dd164462-27f4-4e87-a9e0-23e7543db755", eMarshalDirection.ToManaged )]
	public interface iSimdUtils
	{
		/// <summary>Premultiply alpha in 32-bit RGBA image, and optionally swap red and blue channels.</summary>
		void premultiplyAlpha( [In, Out, MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 1 )] uint[] pixels, int count, [MarshalAs( UnmanagedType.U1 )] bool flipBgrRgb );

		/// <summary>Swap red and blue channels.</summary>
		void flipBgrRgb( [In, Out, MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 1 )] uint[] pixels, int count );

		/// <summary>Convert vector from 32 to 16-bit floating points</summary>
		int fp16( [In] ref Vector4 vec, out ulong fp16 );
		/// <summary>Convert 2D vector to 16-bit floating points</summary>
		int fp16( [In] ref Vector2 vec );

		/// <summary>Convert FP16 vector to FP32</summary>
		int fp32( ulong f16, out Vector4 f32 );

		/// <summary>Linearly interpolate between 2 FP16 vectors, return result also in FP16</summary>
		int fp16Lerp( ulong x, ulong y, float s, out ulong result );

		/// <summary>Write glyph vertices with the specified offset</summary>
		void offsetGlyphs( ref sVertexWithId dest, IntPtr sourcePtr, int countVertices, CPoint offsetValue );

		/// <summary>Not exactly SIMD, just a thin wrapper around libpng.</summary>
		/// <remarks>On Linux OS-provided libpng is used. On Windows Vrmac.dll includes some ancient version shipped with Diligent engine, specifically 1.6.17 from 2015.</remarks>
		int encodeRgbaPng( [In] ref CSize size, [In] ref MappedTextureSubresource mapped, ePngOptions options, [ WriteStream] Stream destination );
	}
}