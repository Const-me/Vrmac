// This source file was automatically generated from "CommandList.h" C++ header by a custom tool.
// The tool is based on CppSharp https://github.com/mono/CppSharp which is based on clang AST https://clang.llvm.org/
#pragma warning disable CS1591	// CS1591: Missing XML comment for publicly visible type or member
using ComLight;
using System;
using System.Runtime.InteropServices;
using System.Security;

namespace Diligent.Graphics
{
	/// <summary>
	/// <para>Command list has no methods. When command list recording is finished, it is executed by</para>
	/// <para>IDeviceContext::ExecuteCommandList().</para>
	/// </summary>
	[ComInterface( "c38c68f2-8a8c-4ed5-b7ee-69126e75dcd8", eMarshalDirection.ToManaged ), CustomConventions( typeof( Vrmac.Utils.NativeErrorMessages ) )]
	public interface ICommandList: IDeviceObject
	{
		/// <summary>Get interface ID of the top-level IDeviceObject-based interface implemented by the object.</summary>
		new void getIid( out Guid iid );
		/// <summary>Returns unique identifier assigned to an object</summary>
		new int GetUniqueID();
		/// <summary>Cast interface pointer from interop to native COM interface</summary>
		new IntPtr nativeCast();
		/// <summary>Description structure that was used to create the object</summary>
		void GetDesc( out DeviceObjectAttribs objectAttribs );

	}
}
