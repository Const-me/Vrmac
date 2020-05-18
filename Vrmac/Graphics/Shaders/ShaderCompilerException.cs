using System;
using System.IO;
using System.Text.RegularExpressions;
using Vrmac;

namespace Diligent.Graphics
{
	/// <summary>The library throws this exception when a shader fails to compile.</summary>
	public class ShaderCompilerException: ApplicationException
	{
		/// <summary>Name of the shader</summary>
		public string name { get; }
		/// <summary>Output of whatever shader compiler was used</summary>
		public string compilerOutput { get; }
		/// <summary>Full source code of the shader, after the preprocessor</summary>
		public string fullSourceCode { get; }

		static readonly Regex reHlslError = new Regex( @"error\s+([A-Za-z]\d+):", RegexOptions.IgnoreCase );

		static string extractHlslContext( string warnings, int idxError )
		{
			int closing = warnings.LastIndexOf( ')' );
			if( closing < 0 )
				return null;
			int opening = warnings.LastIndexOf( '(', closing );
			if( opening < 0 )
				return null;
			return warnings.Substring( opening, closing - opening + 1 );
		}

		static string errorMessage( string warnings )
		{
			if( warnings.isEmpty() )
				return "Unknown shader compilation error";

			var m = reHlslError.Match( warnings );
			if( m.Success )
			{
				string msg = warnings.Substring( m.indexAfter() ).Trim();
				string code = m.group();
				string loc = extractHlslContext( warnings, m.Index );
				if( loc != null )
					return $"Shader compiler error { code } at source location { loc }: { msg }";
				return $"Shader compiler error { code }: { msg }";
			}

			string errorMarker = "error:";
			int idx = warnings.IndexOf( errorMarker, StringComparison.InvariantCultureIgnoreCase );
			if( idx >= 0 )
			{
				idx += errorMarker.Length;
				int end = warnings.IndexOfAny( "\n\r".ToCharArray(), idx );
				if( end > 0 && end - idx > 8 )
				{
					string ss = warnings.Substring( idx, end - idx ).Trim();
					return ss;
				}
				else if( idx + 8 < warnings.Length )
					warnings.Substring( idx ).Trim();
			}
			// TODO: extract better messages from GLES errors, too.
			return warnings.Trim();
		}

		internal ShaderCompilerException( string warnings, string source, string shaderName ) :
			base( errorMessage( warnings ) )
		{
			name = shaderName;
			compilerOutput = warnings?.Trim();
			fullSourceCode = source;
		}

		/// <summary>Save preprocessed source code in a file. If the file is empty, will write a reasonably named file to the temp folder.</summary>
		public void saveSourceCode( string where = null )
		{
			if( null == where )
			{
				string ext = RuntimeEnvironment.runningWindows ? "hlsl" : "glsl";
				string fileName = Path.GetFileNameWithoutExtension( name );
				where = Path.Combine( Path.GetTempPath(), $"{ fileName }-fail.{ ext }" );
			}
			File.WriteAllText( where, fullSourceCode );
			ConsoleLogger.logInfo( "Saved failed shader to \"{0}\"", where );
		}
	}
}