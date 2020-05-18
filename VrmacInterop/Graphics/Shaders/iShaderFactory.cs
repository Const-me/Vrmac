using ComLight;
using System;
using System.Runtime.InteropServices;
using Vrmac;
// TODO: comment them
#pragma warning disable CS1591  // CS1591: Missing XML comment for publicly visible type or member

namespace Diligent.Graphics
{
	/// <summary>Diligent API for shader creation is hard to marshal in C#, too many various pointers in the fields of ShaderCreateInfo structure.
	/// Replacing IRenderDevice.CreateShader C++ method with this COM interface, designed for simplicity of interop with .NET.</summary>
	[ComInterface( "c5c572bc-3879-4731-8d4e-64082e9e4825", eMarshalDirection.ToManaged ), CustomConventions( typeof( Vrmac.Utils.NativeErrorMessages ) )]
	public interface iShaderFactory
	{
		[RetValIndex]
		IRenderDevice getDevice();

		[RetValIndex( 2 )]
		IShader CreateFromByteCode( [In, MarshalAs( UnmanagedType.LPArray )] byte[] bytecode,
			uint length,
			ShaderType shaderType,
			[MarshalAs( UnmanagedType.U1 )]
			bool combinedTextureSampler,
			[MarshalAs( UnmanagedType.LPUTF8Str )]
			string combinedSamplerSuffix = null );

		[RetValIndex( 2 )]
		IShader ioCompileFromFile( iStorageFolder folder, [MarshalAs( UnmanagedType.LPUTF8Str )] string path,
			[In] ref ShaderSourceInfo sourceInfo,
			[MarshalAs( UnmanagedType.LPUTF8Str )] string shaderName,
			[MarshalAs( UnmanagedType.LPUTF8Str )] string entryPoint,
			[MarshalAs( UnmanagedType.LPUTF8Str )] string macros,
			[MarshalAs( UnmanagedType.LPUTF8Str )] string combinedSamplerSuffix,
			out IDataBlob compilerOutput );

		[RetValIndex( 2 )]
		IShader ioCompileFromSource( [MarshalAs( UnmanagedType.LPUTF8Str )] string sourceCode,
			iStorageFolder includesFolder,
			[In] ref ShaderSourceInfo sourceInfo,
			[MarshalAs( UnmanagedType.LPUTF8Str )] string shaderName,
			[MarshalAs( UnmanagedType.LPUTF8Str )] string entryPoint,
			[MarshalAs( UnmanagedType.LPUTF8Str )] string macros,
			[MarshalAs( UnmanagedType.LPUTF8Str )] string combinedSamplerSuffix,
			out IDataBlob compilerOutput );

		/// <summary>The implementation caches user-provided iStorageFolder object, and it may also cache HLSL/GLSL converter. Call this method after you've created all the awesome shaders you use to release most of these objects.</summary>
		void releaseCachedData();

		[RetValIndex( 1 )]
		IShader loadCachedShader( [In] ref Guid guid,
			[In, MarshalAs( UnmanagedType.LPArray )] byte[] binary, uint length, int binaryFormat,
			[MarshalAs( UnmanagedType.LPUTF8Str )]
			string name,
			ShaderType shaderType,
			[MarshalAs( UnmanagedType.U1 )]
			bool combinedTextureSampler,
			[MarshalAs( UnmanagedType.LPUTF8Str )]
			string combinedSamplerSuffix );
	}
}