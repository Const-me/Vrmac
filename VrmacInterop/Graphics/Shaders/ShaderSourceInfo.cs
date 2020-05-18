using System.Runtime.InteropServices;

namespace Diligent.Graphics
{
	/// <summary>Unmanaged structure with various shader compilation parameters.</summary>
	[StructLayout( LayoutKind.Sequential )]
	public struct ShaderSourceInfo
	{
		/// <summary>Shader type, <seealso cref="ShaderType" /></summary>
		public ShaderType shaderType;

		byte zzCombinedTextureSamplers;
		/// <summary>If set to true, textures will be combined with texture samplers.</summary>
		/// <remarks>The CombinedSamplerSuffix member defines the suffix added to the texture variable name to get corresponding sampler name.
		/// When using combined samplers, the sampler assigned to the shader resource view is automatically set when the view is bound.
		/// Otherwise samplers need to be explicitly set similar to other shader variables.</remarks>
		public bool combinedTextureSamplers
		{
			get => zzCombinedTextureSamplers != 0 ? true : false;
			set => zzCombinedTextureSamplers = MiscUtils.byteFromBool( value );
		}

		/* Disabled because the preprocessor is very likely to branch on #if USER_MACRO. After all, such macros is the main reason why you'd want to re-compile shaders from the same source files.
		byte zzReuseShaderConverter;
		/// <summary>If HLSL->GLSL converter is used to convert HLSL shader source to GLSL, set this to true to reuse the conversion stream from the previous shader created by the device..</summary>
		/// <remarks>It is useful when the same file is used to create a number of different shaders.
		/// If this is false (the default), the converter will parse the same file every time new shader is converted.
		/// For all subsequent conversions, file path supplied to iShaderFactory.compile* methods must be the same, or new stream will be crated and warning message will be displayed.</remarks>
		public bool reuseShaderConverter
		{
			get => zzReuseShaderConverter != 0 ? true : false;
			set => zzReuseShaderConverter = MiscUtils.byteFromBool( value );
		} */

		/// <summary>Shader source language, <seealso cref="ShaderSourceLanguage" /></summary>
		public ShaderSourceLanguage language;

		/// <summary>HLSL shader model to use when compiling the shader. When default value is given (0, 0), the engine will attempt to use the highest HLSL shader model supported by the device.</summary>
		/// <remarks>When HLSL source is converted to GLSL, corresponding GLSL/GLESSL version will be used.</remarks>
		public ShaderVersion HLSLVersion;

		/// <summary>GLSL version to use when creating the shader. When default value is given (0, 0), the engine will attempt to use the highest GLSL version supported by the device.</summary>
		public ShaderVersion GLSLVersion;

		/// <summary>GLES shading language version to use when creating the shader. When default value is given (0, 0), the engine will attempt to use the highest GLESSL version supported by the device.</summary>
		public ShaderVersion GLESSLVersion;

		/// <summary>Construct ShaderSourceInfo for specified shader type, set all parameters to defaults.</summary>
		public ShaderSourceInfo( ShaderType type, ShaderSourceLanguage lang = ShaderSourceLanguage.Default )
		{
			shaderType = type;
			zzCombinedTextureSamplers = 0;
			// zzReuseShaderConverter = 0;
			language = lang;
			HLSLVersion = new ShaderVersion( true );
			GLSLVersion = new ShaderVersion( true );
			GLESSLVersion = new ShaderVersion( true );
		}
	}
}