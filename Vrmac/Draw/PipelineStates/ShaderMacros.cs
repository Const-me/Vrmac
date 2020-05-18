using System;
using System.Collections.Generic;

namespace Vrmac.Draw.Shaders
{
	/// <summary>This enum is what makes the same 2 shaders, drawVS.hlsl / drawPS.hlsl, compile into very different code based on the requested features, and on the rendering pass.</summary>
	[Flags]
	enum eShaderMacros: byte
	{
		None = 0,
		/// <summary>Set when compiling opaque pass shaders.</summary>
		OpaquePass = 1,
		/// <summary>The batch has less than 64 draw calls. As a performance optimization, using cbuffer / uniform for them.</summary>
		FewDrawCalls = 2,
		/// <summary>Some draw commands need the sprite atlas</summary>
		TextureAtlas = 4,
		/// <summary>Some draw commands render text</summary>
		TextRendering = 8,
	}

	static class ShaderMacros
	{
		static string macroValue( eShaderMacros vals, eShaderMacros bit )
		{
			return vals.HasFlag( bit ) ? "1" : "0";
		}

		/// <summary>Produce #define values for a shader compiler</summary>
		public static IEnumerable<(string, string)> defines( this eShaderMacros vals )
		{
			yield return ("OPAQUE_PASS", macroValue( vals, eShaderMacros.OpaquePass ));
			yield return ("FEW_DRAW_CALLS", macroValue( vals, eShaderMacros.FewDrawCalls ));
			yield return ("TEXTURE_ATLAS", macroValue( vals, eShaderMacros.TextureAtlas ));
			yield return ("TEXT_RENDERING", macroValue( vals, eShaderMacros.TextRendering ));
		}
	}
}