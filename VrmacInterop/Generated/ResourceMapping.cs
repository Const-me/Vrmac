// This source file was automatically generated from "ResourceMapping.h" C++ header by a custom tool.
// The tool is based on CppSharp https://github.com/mono/CppSharp which is based on clang AST https://clang.llvm.org/
#pragma warning disable CS1591	// CS1591: Missing XML comment for publicly visible type or member
using ComLight;
using System;
using System.Runtime.InteropServices;
using System.Security;

namespace Diligent.Graphics
{
	/// <summary>Describes the resourse mapping object entry</summary>
	[StructLayout( LayoutKind.Sequential )]
	public struct ResourceMappingEntry
	{
		/// <summary>Object name</summary>
		public IntPtr Name;

		/// <summary>Pointer to the object's interface</summary>
		public IntPtr pObject;

		public int ArrayIndex;

		/// <summary>To initialize this structure with default value, use this constructor. The argument value is ignored.</summary>
		/// <remarks>This is only here because C# doesn’t support parameterless constructors for structures.</remarks>
		public ResourceMappingEntry( bool unused )
		{
			Name = IntPtr.Zero;
			pObject = IntPtr.Zero;
			ArrayIndex = 0;
		}
	}

	/// <summary>Resource mapping description</summary>
	[StructLayout( LayoutKind.Sequential )]
	public struct ResourceMappingDesc
	{
		/// <summary>
		/// <para>Pointer to the array of resource mapping entries.</para>
		/// <para>The last element in the array must be default value</para>
		/// <para>created by ResourceMappingEntry::ResourceMappingEntry()</para>
		/// </summary>
		public IntPtr pEntries;

		/// <summary>To initialize this structure with default value, use this constructor. The argument value is ignored.</summary>
		/// <remarks>This is only here because C# doesn’t support parameterless constructors for structures.</remarks>
		public ResourceMappingDesc( bool unused )
		{
			pEntries = IntPtr.Zero;
		}
	}

	/// <summary>
	/// <para>This interface provides mapping between literal names and resource pointers.</para>
	/// <para>It is created by IRenderDevice::CreateResourceMapping().</para>
	/// </summary>
	/// <remarks>Resource mapping holds strong references to all objects it keeps.</remarks>
	[ComInterface( "6c1ac7a6-b429-4139-9433-9e54e93e384a", eMarshalDirection.ToManaged ), CustomConventions( typeof( Vrmac.Utils.NativeErrorMessages ) )]
	public interface IResourceMapping: IDisposable
	{
		/// <summary>Adds a resource to the mapping.</summary>
		/// <param name="Name">Resource name.</param>
		/// <param name="pObject">Pointer to the object.</param>
		/// <param name="bIsUnique">Flag indicating if a resource with the same name is allowed to be found in the mapping. In the latter case, the new resource replaces the existing one.</param>
		/// <remarks>Resource mapping increases the reference counter for referenced objects. So an object will not be released as long as it is in the resource mapping.</remarks>
		void AddResource( [In, MarshalAs( UnmanagedType.LPUTF8Str )] string Name, IDeviceObject pObject, [MarshalAs( UnmanagedType.U1 )] bool bIsUnique );

		/// <summary>Adds resource array to the mapping.</summary>
		/// <param name="Name">Resource array name.</param>
		/// <param name="StartIndex">First index in the array, where the first element will be inserted</param>
		/// <param name="ppObjects">Pointer to the array of objects.</param>
		/// <param name="NumElements">Number of elements to add</param>
		/// <param name="bIsUnique">Flag indicating if a resource with the same name is allowed to be found in the mapping. In the latter case, the new resource replaces the existing one.</param>
		/// <remarks>Resource mapping increases the reference counter for referenced objects. So an object will not be released as long as it is in the resource mapping.</remarks>
		void AddResourceArray( [In, MarshalAs( UnmanagedType.LPUTF8Str )] string Name, int StartIndex, IDeviceObject[] ppObjects, int NumElements, [MarshalAs( UnmanagedType.U1 )] bool bIsUnique );

		/// <summary>Removes a resource from the mapping using its literal name.</summary>
		/// <param name="Name">Name of the resource to remove.</param>
		/// <param name="ArrayIndex">For array resources, index in the array</param>
		void RemoveResourceByName( [In, MarshalAs( UnmanagedType.LPUTF8Str )] string Name, int ArrayIndex );

		/// <summary>Finds a resource in the mapping.</summary>
		/// <param name="Name">Resource name.</param>
		/// <param name="ArrayIndex">for arrays, index of the array element.</param>
		/// <remarks>The method increases the reference counter of the returned object, so Release() must be called.</remarks>
		[RetValIndex( 1 )] IDeviceObject GetResource( [In, MarshalAs( UnmanagedType.LPUTF8Str )] string Name, int ArrayIndex );

		/// <summary>Returns the size of the resource mapping, i.e. the number of objects.</summary>
		[RetValIndex] IntPtr GetSize();
	}
}
