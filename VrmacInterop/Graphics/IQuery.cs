using ComLight;
using System;
using System.Runtime.InteropServices;
// TODO: comment them
#pragma warning disable CS1591  // CS1591: Missing XML comment for publicly visible type or member

namespace Diligent.Graphics
{
	[ComInterface( "70f2a88a-f8be-4901-8f05-2f72fa695ba0" )]
	public interface IQuery: IDeviceObject
	{
		// IDeviceObject methods
		new void getIid( out Guid iid );
		new int GetUniqueID();
		new IntPtr nativeCast();

		// IQuery methods
		void GetDesc( out QueryDesc desc );
		void zzGetData( IntPtr pData, uint DataSize, [MarshalAs( UnmanagedType.U1 )] bool AutoInvalidate );
		void Invalidate();
	}

	public static class QueryExt
	{
		// `unmanaged` was introduced in C# 7.3. With just the struct, the code fails to compile, with the following error:
		// CS0208: Cannot take the address of, get the size of, or declare a pointer to a managed type ('T')
		static void getDataImpl<T>( this IQuery q, out T res, bool autoInvalidate ) where T : unmanaged
		{
			unsafe
			{
				res = new T();
				fixed ( void* pointer = &res )
					q.zzGetData( (IntPtr)pointer, (uint)Marshal.SizeOf<T>(), autoInvalidate );
			}
		}

		public static void GetData( this IQuery q, out QueryDataOcclusion data, bool autoInvalidate )
		{
			q.getDataImpl( out data, autoInvalidate );
		}
		public static void GetData( this IQuery q, out QueryDataBinaryOcclusion data, bool autoInvalidate )
		{
			q.getDataImpl( out data, autoInvalidate );
		}
		public static void GetData( this IQuery q, out QueryDataTimestamp data, bool autoInvalidate )
		{
			q.getDataImpl( out data, autoInvalidate );
		}
		public static void GetData( this IQuery q, out QueryDataPipelineStatistics data, bool autoInvalidate )
		{
			q.getDataImpl( out data, autoInvalidate );
		}
	}
}