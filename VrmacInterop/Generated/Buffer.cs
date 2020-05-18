// This source file was automatically generated from "Buffer.h" C++ header by a custom tool.
// The tool is based on CppSharp https://github.com/mono/CppSharp which is based on clang AST https://clang.llvm.org/
#pragma warning disable CS1591	// CS1591: Missing XML comment for publicly visible type or member
using ComLight;
using System;
using System.Runtime.InteropServices;
using System.Security;

namespace Diligent.Graphics
{
	/// <summary>This enumeration is used by BufferDesc structure.</summary>
	public enum BufferMode : byte
	{
		/// <summary>Undefined mode.</summary>
		Undefined = 0,
		/// <summary>
		/// <para>Formated buffer. Access to the buffer will use format conversion operations.</para>
		/// <para>In this mode, ElementByteStride member of BufferDesc defines the buffer element size.</para>
		/// <para>Buffer views can use different formats, but the format size must match ElementByteStride.</para>
		/// </summary>
		Formatted = 1,
		/// <summary>
		/// <para>Structured buffer.</para>
		/// <para>In this mode, ElementByteStride member of BufferDesc defines the structure stride.</para>
		/// </summary>
		Structured = 2,
		/// <summary>
		/// <para>Raw buffer.</para>
		/// <para>In this mode, the buffer is accessed as raw bytes. Formatted views of a raw</para>
		/// <para>buffer can also be created similar to formatted buffer. If formatted views</para>
		/// <para>are to be created, the ElementByteStride member of BufferDesc must specify the</para>
		/// <para>size of the format.</para>
		/// </summary>
		Raw = 3,
		/// <summary>Helper value storing the total number of modes in the enumeration.</summary>
		NumModes = 4
	}

	/// <summary>Buffer description</summary>
	[StructLayout( LayoutKind.Sequential )]
	public struct BufferDesc
	{
		/// <summary>Structures in C# can’t inherit from other structures. Using encapsulation instead.</summary>
		public DeviceObjectAttribs baseStruct;

		/// <summary>Size of the buffer, in bytes. For a uniform buffer, this must be multiple of 16.</summary>
		public int uiSizeInBytes;

		/// <summary>
		/// <para>The following bind flags are allowed:</para>
		/// <para>Diligent::BIND_VERTEX_BUFFER, Diligent::BIND_INDEX_BUFFER, Diligent::BIND_UNIFORM_BUFFER,</para>
		/// <para>Diligent::BIND_SHADER_RESOURCE, Diligent::BIND_STREAM_OUTPUT, Diligent::BIND_UNORDERED_ACCESS,</para>
		/// <para>Diligent::BIND_INDIRECT_DRAW_ARGS</para>
		/// </summary>
		public BindFlags BindFlags;

		/// <summary>Buffer usage, see Diligent::USAGE for details</summary>
		public Usage Usage;

		/// <summary>
		/// <para>CPU access flags or 0 if no CPU access is allowed,</para>
		/// <para>see Diligent::CPU_ACCESS_FLAGS for details.</para>
		/// </summary>
		public CpuAccessFlags CPUAccessFlags;

		/// <summary>Buffer mode, see Diligent::BUFFER_MODE</summary>
		public BufferMode Mode;

		/// <summary>
		/// <para>For a structured buffer (BufferDesc::Mode equals Diligent::BUFFER_MODE_STRUCTURED) this member</para>
		/// <para>defines the size of each buffer element. For a formatted buffer</para>
		/// <para>(BufferDesc::Mode equals Diligent::BUFFER_MODE_FORMATTED) and optionally for a raw buffer</para>
		/// <para>(Diligent::BUFFER_MODE_RAW), this member defines the size of the format that will be used for views</para>
		/// <para>created for this buffer.</para>
		/// </summary>
		public int ElementByteStride;

		/// <summary>Defines which command queues this buffer can be used with</summary>
		public ulong CommandQueueMask;

		/// <summary>To initialize this structure with default value, use this constructor. The argument value is ignored.</summary>
		/// <remarks>This is only here because C# doesn’t support parameterless constructors for structures.</remarks>
		public BufferDesc( bool unused )
		{
			baseStruct = new DeviceObjectAttribs( true );
			uiSizeInBytes = 0;
			BindFlags = BindFlags.None;
			Usage = Usage.Default;
			CPUAccessFlags = CpuAccessFlags.None;
			Mode = BufferMode.Undefined;
			ElementByteStride = 0;
			CommandQueueMask = 1;
		}
	}

	/// <summary>Defines the methods to manipulate a buffer object</summary>
	[ComInterface( "ec47ead3-a2c4-44f2-81c5-5248d14f10e4", eMarshalDirection.ToManaged ), CustomConventions( typeof( Vrmac.Utils.NativeErrorMessages ) )]
	public interface IBuffer: IDeviceObject
	{
		/// <summary>Get interface ID of the top-level IDeviceObject-based interface implemented by the object.</summary>
		new void getIid( out Guid iid );
		/// <summary>Returns unique identifier assigned to an object</summary>
		new int GetUniqueID();
		/// <summary>Cast interface pointer from interop to native COM interface</summary>
		new IntPtr nativeCast();
		[RetValIndex] BufferDesc GetDesc();


		/// <summary>Creates a new buffer view</summary>
		/// <param name="ViewDesc">View description. See <see cref="BufferViewDesc" /> for details.</param>
		/// <returns>Address of the memory location where the pointer to the view interface will be written to.</returns>
		/// <remarks>To create a view addressing the entire buffer, set only BufferViewDesc::ViewType member of the ViewDesc structure and leave all other members in their default values. Buffer view will contain strong reference to
		/// the buffer, so the buffer will not be destroyed until all views are released. The function calls AddRef() for the created interface, so it must be released by a call to Release() when it is no longer needed.</remarks>
		[RetValIndex( 1 )] IBufferView CreateView( [In] ref BufferViewDesc ViewDesc );

		/// <summary>Returns the pointer to the default view.</summary>
		/// <param name="ViewType">Type of the requested view. See <see cref="BufferViewType" /> .</param>
		/// <remarks>
		/// <para>Default views are only created for structured and raw buffers. As for formatted buffers the view format is unknown at buffer initialization time, no default views are created.</para>
		/// <para>The function does not increase the reference counter for the returned interface, so Release() must *NOT* be called.</para>
		/// </remarks>
		[RetValIndex( 1 )] IBufferView GetDefaultView( BufferViewType ViewType );

		/// <summary>Returns native buffer handle specific to the underlying graphics API</summary>
		[RetValIndex] IntPtr GetNativeHandle();

		/// <summary>Sets the buffer usage state.</summary>
		/// <remarks>
		/// <para>This method does not perform state transition, but resets the internal buffer state to the given value.</para>
		/// <para>This method should be used after the application finished manually managing the buffer state and wants to hand over state management back to the engine.</para>
		/// </remarks>
		void SetState( ResourceState State );

		/// <summary>Returns the internal buffer state</summary>
		[RetValIndex] ResourceState GetState();
	}
}
