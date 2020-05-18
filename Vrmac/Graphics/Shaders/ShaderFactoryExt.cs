using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Vrmac;

namespace Diligent.Graphics
{
	/// <summary>For stupid technical reasons, the API iShaderFactory COM interface exposes directly is far from idiomatic .NET. This static class implements extension methods making it more usable.</summary>
	public static class ShaderFactoryExt
	{
		// Pack macros into null-terminated string. The marshaller will add another null automatically, the C++ needs them double-null terminated.
		static string pack( this IEnumerable<(string, string)> macros )
		{
			if( null == macros )
				return null;

			StringBuilder sb = new StringBuilder();
			foreach( var m in macros )
			{
				if( string.IsNullOrWhiteSpace( m.Item1 ) )
					throw new ArgumentException( "Shader preprocessor macros can't be empty." );
				sb.Append( m.Item1 );
				sb.Append( '\0' );

				sb.Append( m.Item2 );
				sb.Append( '\0' );
			}
			return sb.ToString();
		}

		static (string, string) unpack( this IDataBlob blob )
		{
			if( null == blob )
				return (null, null);

			int cb = (int)blob.GetSize();
			IntPtr ptr = blob.GetDataPtr();
			// Decoding warnings to ANSI so we can use string.Length to find out count of bytes, and adjust the pointer for reading the preprocessed source code.
			string warnings = Marshal.PtrToStringAnsi( ptr );
			string fullSource = Marshal.PtrToStringUTF8( ptr + warnings.Length + 1 );
			return (warnings, fullSource);
		}

		static IShader compileFromFileImpl( this iShaderFactory factory, iStorageFolder folder, string path, ref ShaderSourceInfo sourceInfo, string shaderName, string entryPoint, string macros, string combinedSamplerSuffix, out IDataBlob compilerOutput )
		{
			try
			{
				return factory.ioCompileFromFile( folder, path, ref sourceInfo, shaderName, entryPoint, macros, combinedSamplerSuffix, out compilerOutput );
			}
			catch( Exception ex )
			{
				Vrmac.Utils.NativeContext.throwCached( ex.HResult );
				throw;
			}
		}

		static IShader compileFromSourceImpl( this iShaderFactory factory, string sourceCode, iStorageFolder includesFolder, ref ShaderSourceInfo sourceInfo, string shaderName, string entryPoint, string macros, string combinedSamplerSuffix, out IDataBlob compilerOutput )
		{
			try
			{
				return factory.ioCompileFromSource( sourceCode, includesFolder, ref sourceInfo, shaderName, entryPoint, macros, combinedSamplerSuffix, out compilerOutput );
			}
			catch( Exception ex )
			{
				Vrmac.Utils.NativeContext.throwCached( ex.HResult );
				throw;
			}
		}

		/// <summary>Create shader by compiling a source file</summary>
		public static IShader compileFromFile( this iShaderFactory factory,
			iStorageFolder folder, string path,
			ShaderSourceInfo sourceInfo,
			string shaderName = null,
			IEnumerable<(string, string)> macros = null, string entryPoint = null, string combinedSamplerSuffix = null )
		{
			if( null == shaderName )
				shaderName = sourceInfo.shaderType.ToString();
			IShader shader = factory.compileFromFileImpl( folder, path, ref sourceInfo, shaderName, entryPoint, macros.pack(), combinedSamplerSuffix, out IDataBlob output );
			CompiledShader result = new CompiledShader( shader, output.unpack() );
			output?.Dispose();

			result.throwIfFailed( shaderName );
			return result.shader;
		}

		/// <summary>Create shader by compiling a source code from a string</summary>
		public static IShader compileFromSource( this iShaderFactory factory,
			string sourceCode, ShaderSourceInfo sourceInfo,
			string shaderName = null,
			iStorageFolder includesFolder = null,
			IEnumerable<(string, string)> macros = null, string entryPoint = null, string combinedSamplerSuffix = null )
		{
			if( null == shaderName )
				shaderName = sourceInfo.shaderType.ToString();
			IShader shader = factory.ioCompileFromSource( sourceCode, includesFolder, ref sourceInfo, shaderName, entryPoint, macros.pack(), combinedSamplerSuffix, out IDataBlob output );
			CompiledShader result = new CompiledShader( shader, output.unpack() );
			output?.Dispose();

			result.throwIfFailed( shaderName );
			return result.shader;
		}

		/// <summary>Create shader by compiling HLSL source file</summary>
		public static IShader compileHlslFile( this iShaderFactory factory, iStorageFolder folder, string path, ShaderType type, string shaderName = null )
		{
			ShaderSourceInfo sourceInfo = new ShaderSourceInfo( type, ShaderSourceLanguage.Hlsl );
			sourceInfo.combinedTextureSamplers = true;
			if( null == shaderName )
				shaderName = Path.GetFileNameWithoutExtension( path );

			IShader shader = factory.compileFromFileImpl( folder, path, ref sourceInfo, shaderName, null, null, null, out IDataBlob output );

			CompiledShader result = new CompiledShader( shader, output.unpack() );
			output?.Dispose();
			result.throwIfFailed( shaderName );
			return result.shader;
		}

		static bool testFileExist( iStorageFolder folder, string name )
		{
			try
			{
				folder.openRead( name, out Stream stm );
				if( null == stm )
					return false;
				stm.Dispose();
				return true;
			}
			catch( Exception )
			{
				return false;
			}
		}

		static string shaderTypeSuffix( ShaderType shaderType )
		{
			switch( shaderType )
			{
				default:
				case ShaderType.Unknown:
					throw new ArgumentException();
				case ShaderType.Vertex: return "VS";
				case ShaderType.Pixel: return "PS";
				case ShaderType.Geometry: return "GS";
				case ShaderType.Hull: return "HS";
				case ShaderType.Domain: return "DS";
				case ShaderType.Compute: return "CS";
			}
		}

		static string tryFindShader( Func<string, bool> findFile, string name, string ext, ShaderType shaderType )
		{
			string test = Path.ChangeExtension( name, ext );
			if( findFile( test ) )
				return test;
			string noExt = Path.ChangeExtension( name, null );
			noExt += shaderTypeSuffix( shaderType );
			test = Path.ChangeExtension( noExt, ext );
			if( findFile( test ) )
				return test;
			return null;
		}

		static IShader compileShaderImpl( iShaderFactory factory, iStorageFolder folder, string sourceFile, ShaderType shaderType, ShaderSourceLanguage lang, IEnumerable<(string, string)> defines )
		{
			string shaderName = Path.GetFileName( sourceFile );
			ShaderSourceInfo sourceInfo = new ShaderSourceInfo( shaderType, lang );
			sourceInfo.combinedTextureSamplers = true;
			IShader shader = factory.compileFromFileImpl( folder, sourceFile, ref sourceInfo, shaderName, null, pack( defines ), null, out IDataBlob output );
			CompiledShader result = new CompiledShader( shader, output.unpack() );
			output?.Dispose();
			result.throwIfFailed( shaderName );
			return result.shader;
		}

		/// <summary>Compile shader from a file. The source may be *.hlsl, or on Linux *.glsl is tried first.</summary>
		/// <remarks>The folder is searched for different names which depend on shader type, for example "shaderVS.glsl" or "shaderPS.hlsl".
		/// Both file names and shader type suffixes are case sensitive, the suffix must be uppercase.</remarks>
		public static IShader compileShader( this iShaderFactory factory, iStorageFolder folder, string name, ShaderType shaderType, IEnumerable<(string, string)> defines = null )
		{
			if( null == name )
				throw new ArgumentNullException( "name" );

			Func<string, bool> findFile;
			if( folder is iStorageFolderManaged sf )
				findFile = sf.fileExist;
			else
				findFile = nn => testFileExist( folder, nn );

			if( Vrmac.RuntimeEnvironment.operatingSystem == eOperatingSystem.Linux )
			{
				string glsl = tryFindShader( findFile, name, ".glsl", shaderType );
				if( null != glsl )
					return compileShaderImpl( factory, folder, glsl, shaderType, ShaderSourceLanguage.Glsl, defines );
			}

			string hlsl = tryFindShader( findFile, name, ".hlsl", shaderType );
			if( null != hlsl )
				return compileShaderImpl( factory, folder, hlsl, shaderType, ShaderSourceLanguage.Hlsl, defines );

			throw new FileNotFoundException( $"{ shaderType } shader \"{ name }\" wasn’t found in { folder }" );
		}
	}
}