// This source file was automatically generated from "ShaderResourceVariable.h" C++ header by a custom tool.
// The tool is based on CppSharp https://github.com/mono/CppSharp which is based on clang AST https://clang.llvm.org/
#pragma warning disable CS1591	// CS1591: Missing XML comment for publicly visible type or member
using ComLight;
using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Numerics;

namespace Diligent.Graphics
{
	/// <summary>Describes the type of the shader resource variable</summary>
	public enum ShaderResourceVariableType : byte
	{
		/// <summary>
		/// <para>Shader resource bound to the variable is the same for all SRB instances.</para>
		/// <para>It must be set *once* directly through Pipeline State object.</para>
		/// </summary>
		Static = 0,
		/// <summary>
		/// <para>Shader resource bound to the variable is specific to the shader resource binding</para>
		/// <para>instance (see Diligent::IShaderResourceBinding). It must be set *once* through</para>
		/// <para>Diligent::IShaderResourceBinding interface. It cannot be set through Diligent::IPipelineState</para>
		/// <para>interface and cannot be change once bound.</para>
		/// </summary>
		Mutable = 1,
		/// <summary>
		/// <para>Shader variable binding is dynamic. It can be set multiple times for every instance of shader resource</para>
		/// <para>binding (see Diligent::IShaderResourceBinding). It cannot be set through Diligent::IPipelineState interface.</para>
		/// </summary>
		Dynamic = 2,
		/// <summary>Total number of shader variable types</summary>
		NumTypes = 3
	}

	/// <summary>Shader resource binding flags</summary>
	public enum BindShaderResourcesFlags : uint
	{
		/// <summary>Indicates that static shader variable bindings are to be updated.</summary>
		UpdateStatic = 0x1,
		/// <summary>Indicates that mutable shader variable bindings are to be updated.</summary>
		UpdateMutable = 0x2,
		/// <summary>Indicates that dynamic shader variable bindings are to be updated.</summary>
		UpdateDynamic = 0x4,
		/// <summary>Indicates that all shader variable types (static, mutable and dynamic) are to be updated.</summary>
		/// <remarks>
		/// <para>If none of BIND_SHADER_RESOURCES_UPDATE_STATIC, BIND_SHADER_RESOURCES_UPDATE_MUTABLE,</para>
		/// <para>and BIND_SHADER_RESOURCES_UPDATE_DYNAMIC flags are set, all variable types are updated</para>
		/// <para>as if BIND_SHADER_RESOURCES_UPDATE_ALL was specified.</para>
		/// </remarks>
		UpdateAll = 7,
		/// <summary>
		/// <para>If this flag is specified, all existing bindings will be preserved and</para>
		/// <para>only unresolved ones will be updated.</para>
		/// <para>If this flag is not specified, every shader variable will be</para>
		/// <para>updated if the mapping contains corresponding resource.</para>
		/// </summary>
		KeepExisting = 0x8,
		/// <summary>
		/// <para>If this flag is specified, all shader bindings are expected</para>
		/// <para>to be resolved after the call. If this is not the case, debug message</para>
		/// <para>will be displayed.</para>
		/// </summary>
		/// <remarks>
		/// <para>Only these variables are verified that are being updated by setting</para>
		/// <para>BIND_SHADER_RESOURCES_UPDATE_STATIC, BIND_SHADER_RESOURCES_UPDATE_MUTABLE, and</para>
		/// <para>BIND_SHADER_RESOURCES_UPDATE_DYNAMIC flags.</para>
		/// </remarks>
		VerifyAllResolved = 0x10
	}

	/// <summary>Shader resource variable</summary>
	[ComInterface( "0d57df3f-977d-4c8f-b64c-6675814bc80c", eMarshalDirection.ToManaged ), CustomConventions( typeof( Vrmac.Utils.NativeErrorMessages ) )]
	public interface IShaderResourceVariable: IDisposable
	{
		/// <summary>Binds resource to the variable</summary>
		/// <remarks>
		/// <para>The method performs run-time correctness checks.</para>
		/// <para>For instance, shader resource view cannot be assigned to a constant buffer variable.</para>
		/// </remarks>
		void Set( IDeviceObject pObject );

		/// <summary>Binds resource array to the variable</summary>
		/// <param name="ppObjects">pointer to the array of objects</param>
		/// <param name="FirstElement">first array element to set</param>
		/// <param name="NumElements">number of objects in ppObjects array</param>
		/// <remarks>
		/// <para>The method performs run-time correctness checks.</para>
		/// <para>For instance, shader resource view cannot be assigned to a constant buffer variable.</para>
		/// </remarks>
		void SetArray( IDeviceObject[] ppObjects, int FirstElement, int NumElements );

		/// <summary>Returns the shader resource variable type</summary>
		[RetValIndex] ShaderResourceVariableType GetType();

		/// <summary>Returns shader resource description. See <see cref="ShaderResourceDesc" /> .</summary>
		[RetValIndex] ShaderResourceDesc GetResourceDesc();

		/// <summary>Returns the variable index that can be used to access the variable.</summary>
		[RetValIndex] int GetIndex();

		/// <summary>Returns true if non-null resource is bound to this variable.</summary>
		/// <param name="ArrayIndex">Resource array index. Must be 0 for non-array variables.</param>
		bool IsBound( int ArrayIndex );
	}
}
