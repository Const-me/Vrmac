// This source file was automatically generated from "BufferView.h" C++ header by a custom tool.
// The tool is based on CppSharp https://github.com/mono/CppSharp which is based on clang AST https://clang.llvm.org/
#pragma warning disable CS1591	// CS1591: Missing XML comment for publicly visible type or member
using ComLight;
using System;
using System.Runtime.InteropServices;
using System.Security;

namespace Diligent.Graphics
{
	/// <summary>Buffer format description</summary>
	[StructLayout( LayoutKind.Sequential )]
	public struct BufferFormat
	{
		/// <summary>Type of components. For a formatted buffer views, this value cannot be VT_UNDEFINED</summary>
		public GpuValueType ValueType;

		/// <summary>
		/// <para>Number of components. Allowed values: 1, 2, 3, 4.</para>
		/// <para>For a formatted buffer, this value cannot be 0</para>
		/// </summary>
		public byte NumComponents;

		byte m_IsNormalized;
		/// <summary>
		/// <para>For signed and unsigned integer value types</para>
		/// <para>(VT_INT8, VT_INT16, VT_INT32, VT_UINT8, VT_UINT16, VT_UINT32)</para>
		/// <para>indicates if the value should be normalized to [-1,+1] or</para>
		/// <para>[0, 1] range respectively. For floating point types</para>
		/// <para>(VT_FLOAT16 and VT_FLOAT32), this member is ignored.</para>
		/// </summary>
		public bool IsNormalized
		{
			get => ( 0 != m_IsNormalized );
			set => m_IsNormalized = MiscUtils.byteFromBool( value );
		}

		/// <summary>To initialize this structure with default value, use this constructor. The argument value is ignored.</summary>
		/// <remarks>This is only here because C# doesn’t support parameterless constructors for structures.</remarks>
		public BufferFormat( bool unused )
		{
			ValueType = GpuValueType.Undefined;
			NumComponents = 0;
			m_IsNormalized = 0;
		}
	}

	/// <summary>Buffer view description</summary>
	[StructLayout( LayoutKind.Sequential )]
	public struct BufferViewDesc
	{
		/// <summary>Structures in C# can’t inherit from other structures. Using encapsulation instead.</summary>
		public DeviceObjectAttribs baseStruct;

		/// <summary>View type. See Diligent::BUFFER_VIEW_TYPE for details.</summary>
		public BufferViewType ViewType;

		/// <summary>
		/// <para>Format of the view. This member is only used for formatted and raw buffers.</para>
		/// <para>To create raw view of a raw buffer, set Format.ValueType member to VT_UNDEFINED</para>
		/// <para>(default value).</para>
		/// </summary>
		public BufferFormat Format;

		/// <summary>
		/// <para>Offset in bytes from the beginnig of the buffer to the start of the</para>
		/// <para>buffer region referenced by the view</para>
		/// </summary>
		public int ByteOffset;

		/// <summary>Size in bytes of the referenced buffer region</summary>
		public int ByteWidth;

		/// <summary>To initialize this structure with default value, use this constructor. The argument value is ignored.</summary>
		/// <remarks>This is only here because C# doesn’t support parameterless constructors for structures.</remarks>
		public BufferViewDesc( bool unused )
		{
			baseStruct = new DeviceObjectAttribs( true );
			ViewType = BufferViewType.Undefined;
			Format = new BufferFormat( true );
			ByteOffset = 0;
			ByteWidth = 0;
		}
	}

	/// <summary>To create a buffer view, call IBuffer::CreateView().</summary>
	/// <remarks>
	/// <para>Buffer view holds strong references to the buffer. The buffer</para>
	/// <para>will not be destroyed until all views are released.</para>
	/// </remarks>
	[ComInterface( "e2e83490-e9d2-495b-9a83-abb413a38b07", eMarshalDirection.ToManaged ), CustomConventions( typeof( Vrmac.Utils.NativeErrorMessages ) )]
	public interface IBufferView: IDeviceObject
	{
		/// <summary>Get interface ID of the top-level IDeviceObject-based interface implemented by the object.</summary>
		new void getIid( out Guid iid );
		/// <summary>Returns unique identifier assigned to an object</summary>
		new int GetUniqueID();
		/// <summary>Cast interface pointer from interop to native COM interface</summary>
		new IntPtr nativeCast();
		[RetValIndex] BufferViewDesc GetDesc();


		/// <summary>Returns pointer to the referenced buffer object.</summary>
		/// <remarks>The method does *NOT* call AddRef() on the returned interface, so Release() must not be called.</remarks>
		[RetValIndex] IBuffer GetBuffer();
	}
}
