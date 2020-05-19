// This source file was automatically generated from "Fence.h" C++ header by a custom tool.
// The tool is based on CppSharp https://github.com/mono/CppSharp which is based on clang AST https://clang.llvm.org/
#pragma warning disable CS1591	// CS1591: Missing XML comment for publicly visible type or member
using ComLight;
using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Numerics;

namespace Diligent.Graphics
{
	/// <summary>Fence description</summary>
	[StructLayout( LayoutKind.Sequential )]
	public struct FenceDesc
	{
		/// <summary>Structures in C# can’t inherit from other structures. Using encapsulation instead.</summary>
		public DeviceObjectAttribs baseStruct;

		/// <summary>To initialize this structure with default value, use this constructor. The argument value is ignored.</summary>
		/// <remarks>This is only here because C# doesn’t support parameterless constructors for structures.</remarks>
		public FenceDesc( bool unused )
		{
			baseStruct = new DeviceObjectAttribs( true );
		}
	}

	/// <summary>Defines the methods to manipulate a fence object</summary>
	/// <remarks>
	/// <para>When a fence that was previously signaled by IDeviceContext::SignalFence() is destroyed,</para>
	/// <para>it may block the GPU until all prior commands have completed execution.</para>
	/// </remarks>
	[ComInterface( "3b19184d-32ab-4701-84f4-9a0c03ae1672", eMarshalDirection.ToManaged ), CustomConventions( typeof( Vrmac.Utils.NativeErrorMessages ) )]
	public interface IFence: IDeviceObject
	{
		/// <summary>Get interface ID of the top-level IDeviceObject-based interface implemented by the object.</summary>
		new void getIid( out Guid iid );
		/// <summary>Returns unique identifier assigned to an object</summary>
		new int GetUniqueID();
		/// <summary>Cast interface pointer from interop to native COM interface</summary>
		new IntPtr nativeCast();
		[RetValIndex] FenceDesc GetDesc();


		/// <summary>Returns the last completed value signaled by the GPU</summary>
		/// <remarks>This method is not thread safe (even if the fence object is protected by mutex) and must only be called by the same thread that signals the fence via IDeviceContext::SignalFence().</remarks>
		[RetValIndex] ulong GetCompletedValue();

		/// <summary>Resets the fence to the specified value.</summary>
		void Reset( ulong Value );
	}
}
