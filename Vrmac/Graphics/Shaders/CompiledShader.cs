namespace Diligent.Graphics
{
	/// <summary>Result of shader compilation. </summary>
	public class CompiledShader
	{
		/// <summary>The shader object</summary>
		public readonly IShader shader;
		/// <summary>Compiler warnings of whatever shader compiler was used</summary>
		public readonly string compilerWarnings;
		/// <summary>Full source code of the shader, after the preprocessor</summary>
		public readonly string fullSourceCode;

		/// <summary>Construct the object</summary>
		public CompiledShader( IShader shader, (string, string) output )
		{
			this.shader = shader;
			compilerWarnings = output.Item1;
			fullSourceCode = output.Item2;
		}

		internal void throwIfFailed( string name )
		{
			if( null != shader )
				return;
			throw new ShaderCompilerException( compilerWarnings, fullSourceCode, name );
		}
	}
}