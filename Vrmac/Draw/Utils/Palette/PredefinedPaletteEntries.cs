using Diligent.Graphics;
using System;
using System.Collections.Generic;

namespace Vrmac.Draw.Palette
{
	/// <summary>Creates first 17 palette entries, they are hardcoded.</summary>
	/// <remarks>The first 16 of them match ConsoleColor values, HTML 4.01 spec, and CGA. The final predefined value at index 16 is the transparent color.</remarks>
	static class PredefinedPaletteEntries
	{
		// That file has 16 colors, 3 bytes / each, RGB order. 48 bytes don't deserve to be gzipped.
		const string resourceName = "Vrmac.Draw.Utils.Palette.colors.bin";

		const float mul = 1.0f / 255.0f;

		static Vector4 makeColor( ReadOnlySpan<byte> bytes, int readindex )
		{
			Vector3 rgb = new Vector3();
			rgb.X = bytes[ readindex ];
			rgb.Y = bytes[ readindex + 1 ];
			rgb.Z = bytes[ readindex + 2 ];
			rgb *= mul;
			return new Vector4( rgb, 1 );
		}

		/// <summary>Read the palette, encode into FP16, put into dictionary</summary>
		public static void initPalette( Dictionary<ulong, int> colors )
		{
			Span<byte> bytes = stackalloc byte[ 16 * 3 ];
			using( var stm = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream( resourceName ) )
				stm.Read( bytes );

			for( int i = 0; i < 16; i++ )
			{
				Vector4 color = makeColor( bytes, i * 3 );
				ulong fp16 = GraphicsUtils.fp16( ref color );
				colors.Add( fp16, i );
			}
			colors.Add( 0, 16 );
		}

		/// <summary>Read the palette, return FP32 array with the entries</summary>
		public static Vector4[] readPalette()
		{
			Span<byte> bytes = stackalloc byte[ 16 * 3 ];
			using( var stm = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream( resourceName ) )
				stm.Read( bytes );

			Vector4[] result = new Vector4[ 17 ];
			for( int i = 0; i < 16; i++ )
				result[ i ] = makeColor( bytes, i * 3 );
			result[ 16 ] = Color.transparent;
			return result;
		}
	}
}