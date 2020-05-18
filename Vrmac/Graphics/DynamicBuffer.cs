using System;
using System.Runtime.InteropServices;
using Vrmac;
using RuntimeEnvironment = Vrmac.RuntimeEnvironment;

namespace Diligent.Graphics
{
	/// <summary>Dynamically growable GPU buffer for dynamic data. Initially implemented for 2D renderer vertex and index buffers.</summary>
	public sealed class DynamicBuffer: IDisposable
	{
		/// <summary>The buffer</summary>
		public IBuffer buffer { get; private set; }
		/// <summary>Current capacity in bytes</summary>
		public int capacity { get; private set; } = 0;
		/// <summary>Bind flags supported by the buffer</summary>
		public readonly BindFlags usage;

		readonly Context context;
		/// <summary>Maximum capacity of the buffer. Attempts to grow it beyond this will fail.</summary>
		public readonly int maximumCapacity;

		void resize( int capacity )
		{
			if( maximumCapacity > 0 && capacity > maximumCapacity )
				throw new ArgumentException( "Attempting to resize a dynamic buffer over it’s maximum capacity" );

			BufferDesc desc = new BufferDesc( false );
			desc.Usage = Usage.Dynamic;
			desc.BindFlags = usage;
			desc.CPUAccessFlags = CpuAccessFlags.Write;
			desc.uiSizeInBytes = capacity;
			if( usage.HasFlag( BindFlags.ShaderResource ) )
			{
				desc.Mode = BufferMode.Structured;
				desc.ElementByteStride = 16;
			}
			using( var device = context.renderContext.device )
				buffer = device.CreateBuffer( ref desc, null, IntPtr.Zero, 0 );

			this.capacity = capacity;
			if( null != m_resizedEvent )
				foreach( var sub in m_resizedEvent )
					sub( this, capacity );
		}

		/// <summary>Construct the buffer</summary>
		public DynamicBuffer( Context context, BindFlags usage, int initialCapacity, int maxCapacity = 0 )
		{
			if( RuntimeEnvironment.runningLinux && usage.HasFlag( BindFlags.UniformBuffer ) )
				maxCapacity = Math.Min( maxCapacity, 64 * 1024 );   // That 64kb value is reported by GL_MAX_VERTEX_UNIFORM_VECTORS and GL_MAX_FRAGMENT_UNIFORM_VECTORS

			maximumCapacity = maxCapacity;
			this.usage = usage;
			this.context = context;
			initialCapacity = Math.Min( initialCapacity, 1024 * 2 );
			resize( initialCapacity.nextPowerOf2() );
		}

		/// <summary>Interface to write the data</summary>
		public interface iMapped: IDisposable
		{
			/// <summary>Writeable location. For performance reasons, reading from there is not recommended.</summary>
			Span<byte> span { get; }
		}

		/// <summary>Map the buffer for writing.</summary>
		public Mapped<T> map<T>( IDeviceContext context, int length ) where T : unmanaged
		{
			int requestedSize = length * Marshal.SizeOf<T>();
			IntPtr pointer;
			if( requestedSize <= capacity )
			{
				pointer = context.MapBuffer( buffer, MapType.Write, MapFlags.Discard );
				return new Mapped<T>( pointer, length, context, buffer );
			}

			resize( requestedSize.nextPowerOf2() );

			pointer = context.MapBuffer( buffer, MapType.Write, MapFlags.Discard );
			return new Mapped<T>( pointer, length, context, buffer );
		}

		/// <summary>The buffer mapped for writing</summary>
		public struct Mapped<T>: IDisposable where T : unmanaged
		{
			readonly IntPtr pointer;
			readonly int length;
			readonly IDeviceContext context;
			readonly IBuffer buffer;

			/// <summary>Writeable span</summary>
			public Span<T> span => Unsafe.writeSpan<T>( pointer, length );

			void IDisposable.Dispose()
			{
				context?.UnmapBuffer( buffer, MapType.Write );
			}

			internal Mapped( IntPtr pointer, int length, IDeviceContext context, IBuffer buffer )
			{
				this.pointer = pointer;
				this.length = length;
				this.context = context;
				this.buffer = buffer;
			}
		}

		/// <summary>Destroy the buffer</summary>
		public void Dispose()
		{
			buffer?.Dispose();
			buffer = null;
		}

		/// <summary>Delegate to receive notification when buffer is resized</summary>
		public delegate void Resized( DynamicBuffer sender, int newCapacityBytes );

		WeakEvent<Resized> m_resizedEvent;
		/// <summary>Subscribe to this event to get notified when this buffer is resized.</summary>
		public WeakEvent<Resized> resized
		{
			get
			{
				if( null != m_resizedEvent )
					return m_resizedEvent;
				m_resizedEvent = new WeakEvent<Resized>();
				return m_resizedEvent;
			}
		}
	}
}