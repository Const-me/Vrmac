// This source file was automatically generated from "ShaderResourceBinding.h" C++ header by a custom tool.
// The tool is based on CppSharp https://github.com/mono/CppSharp which is based on clang AST https://clang.llvm.org/
#pragma warning disable CS1591	// CS1591: Missing XML comment for publicly visible type or member
using ComLight;
using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Numerics;

namespace Diligent.Graphics
{
	/// <summary>Shader resource binding interface</summary>
	[ComInterface( "061f8774-9a09-48e8-8411-b5bd20560104", eMarshalDirection.ToManaged ), CustomConventions( typeof( Vrmac.Utils.NativeErrorMessages ) )]
	public interface IShaderResourceBinding: IDisposable
	{
		/// <summary>Returns pointer to the referenced buffer object.</summary>
		/// <remarks>The method calls AddRef() on the returned interface, so Release() must be called to avoid memory leaks.</remarks>
		[RetValIndex] IPipelineState GetPipelineState();

		/// <summary>Binds mutable and dynamice resources using the resource mapping</summary>
		/// <param name="ShaderFlags">
		/// <para>Flags that specify shader stages, for which resources will be bound.</para>
		/// <para>Any combination of <see cref="ShaderType" /> may be used.</para>
		/// </param>
		/// <param name="pResMapping">Shader resource mapping, where required resources will be looked up</param>
		/// <param name="Flags">Additional flags. See <see cref="BindShaderResourcesFlags" /> .</param>
		void BindResources( int ShaderFlags, IResourceMapping pResMapping, int Flags );

		/// <summary>Returns variable</summary>
		/// <param name="ShaderType">
		/// <para>Type of the shader to look up the variable.</para>
		/// <para>Must be one of <see cref="ShaderType" /> .</para>
		/// </param>
		/// <param name="Name">Variable name</param>
		/// <remarks>This operation may potentially be expensive. If the variable will be used often, it is recommended to store and reuse the pointer as it never changes.</remarks>
		[RetValIndex( 2 )] IShaderResourceVariable GetVariableByName( ShaderType ShaderType, [In, MarshalAs( UnmanagedType.LPUTF8Str )] string Name );

		/// <summary>Returns the total variable count for the specific shader stage.</summary>
		/// <param name="ShaderType">Type of the shader.</param>
		/// <remarks>The method only counts mutable and dynamic variables that can be accessed through the Shader Resource Binding object. Static variables are accessed through the Shader object.</remarks>
		[RetValIndex( 1 )] int GetVariableCount( ShaderType ShaderType );

		/// <summary>Returns variable</summary>
		/// <param name="ShaderType">
		/// <para>Type of the shader to look up the variable.</para>
		/// <para>Must be one of <see cref="ShaderType" /> .</para>
		/// </param>
		/// <param name="Index">Variable index. The index must be between 0 and the total number of variables in this shader stage as returned by IShaderResourceBinding::GetVariableCount().</param>
		/// <remarks>
		/// <para>Only mutable and dynamic variables can be accessed through this method.</para>
		/// <para>Static variables are accessed through the Shader object.</para>
		/// <para>This operation may potentially be expensive. If the variable will be used often, it is recommended to store and reuse the pointer as it never changes.</para>
		/// </remarks>
		[RetValIndex( 2 )] IShaderResourceVariable GetVariableByIndex( ShaderType ShaderType, int Index );

		/// <summary>Initializes static resources</summary>
		/// <param name="pPipelineState">
		/// <para>Pipeline state to copy static shader resource bindings from. The pipeline state must be compatible with this shader resource binding object.</para>
		/// <para>If null pointer is provided, the pipeline state that this SRB object was created from is used.</para>
		/// </param>
		/// <remarks>
		/// <para>If the parent pipeline state object contain static resources (see <see cref="ShaderResourceVariableType.Static" /> ), this method must be called once to initialize static resources in this shader resource binding
		/// object.</para>
		/// <para>The method must be called after all static variables are initialized in the PSO.</para>
		/// <para>The method must be called exactly once. If static resources have already been initialized and the method is called again, it will have no effect and a warning messge will be displayed.</para>
		/// </remarks>
		void InitializeStaticResources( IPipelineState pPipelineState );
	}
}
