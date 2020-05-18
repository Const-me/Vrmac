using ComLight;
using System;
using System.Runtime.InteropServices;

namespace Diligent.Graphics
{
	/// <summary>Implementing this interface improves startup by caching shader binaries.</summary>
	/// <remarks>Unfortunately, at the time of writing Raspberry Pi4 GPU driver doesn’t support the required functionality.</remarks>
	[ComInterface( "761268b2-63d6-4f11-9f7a-2e4ce4448bf3", eMarshalDirection.BothWays )]
	public interface iShaderCache
	{
		/// <summary>Shader factory is created early while creation of devices. When device is created, it calls this method to set the newly created device.</summary>
		/// <remarks>In your tryLoadShader implementation, you must use this very fectory to call <see cref="iShaderFactory.loadCachedShader(ref Guid, byte[], uint, int, string, ShaderType, bool, string)" />.</remarks>
		void setFactory( iShaderFactory factory );

		/// <summary>Lookup a shader by GUID, if found, call <see cref="iShaderFactory.loadCachedShader(ref Guid, byte[], uint, int, string, ShaderType, bool, string)" />.</summary>
		int tryLoadShader( [In] ref Guid guid, out IShader shader );

		/// <summary>Cache compiled shader binary.</summary>
		/// <param name="guid"></param>
		/// <param name="binary"></param>
		/// <param name="length"></param>
		/// <param name="binaryFormat">The format of the program binary ​ may be implementation dependent</param>
		/// <param name="name"></param>
		/// <param name="shaderType"></param>
		/// <param name="combinedTextureSampler"></param>
		/// <param name="combinedSamplerSuffix"></param>
		/// <returns></returns>
		int saveShader( [In] ref Guid guid, [MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 2 )] byte[] binary, uint length, int binaryFormat,
			[MarshalAs( UnmanagedType.LPUTF8Str )]
			string name,
			ShaderType shaderType,
			[MarshalAs( UnmanagedType.U1 )]
			bool combinedTextureSampler,
			[MarshalAs( UnmanagedType.LPUTF8Str )]
			string combinedSamplerSuffix );
	}
}