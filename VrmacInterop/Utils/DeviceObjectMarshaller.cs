using ComLight;
using ComLight.Marshalling;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Diligent.Graphics
{
	/// <summary>Custom marshaller to construct appropriate IDeviceObject-derived interfaces from COM pointers returned by C++ code.</summary>
	class DeviceObjectMarshaller: iCustomMarshal
	{
		public override Type getNativeType( ParameterInfo managedParameter )
		{
			Type managed = managedParameter.ParameterType;
			if( managed == typeof( IDeviceObject ) )
				return typeof( IntPtr );
			if( managed == typeof( IDeviceObject ).MakeByRefType() )
			{
				if( managedParameter.IsIn )
					throw new ArgumentException( "COM interfaces can only be marshalled in or out, ref is not supported for them" );
				return typeof( IntPtr ).MakeByRefType();
			}
			throw new ApplicationException( @"DeviceObjectMarshaller is only compatible with IDeviceObject arguments" );
		}

		static readonly object syncRoot = new object();
		/// <summary>Key = interface ID</summary>
		static readonly Dictionary<Guid, Func<IntPtr, object>> interfaces = new Dictionary<Guid, Func<IntPtr, object>>();
		/// <summary>Key = C++ vtable address</summary>
		static readonly Dictionary<IntPtr, Func<IntPtr, object>> vtables = new Dictionary<IntPtr, Func<IntPtr, object>>();

		/// <summary>marshalNative static method of this class</summary>
		static readonly MethodInfo miMarshalNative;

		static DeviceObjectMarshaller()
		{
			// Use reflection to find all IDeviceObject-derived interfaces in this assembly
			Type deviceObject = typeof( IDeviceObject );
			Assembly ass = Assembly.GetExecutingAssembly();
			foreach( var tp in ass.GetTypes() )
			{
				var attrib = tp.GetCustomAttribute<ComInterfaceAttribute>();
				if( null == attrib )
					continue;
				if( !deviceObject.IsAssignableFrom( tp ) )
					continue;
				if( tp == deviceObject )
					continue;
				interfaces.Add( attrib.iid, NativeWrapper.getFactory( tp ) );
			}

			miMarshalNative = typeof( DeviceObjectMarshaller ).GetMethod( "marshalNative" );
		}

		[UnmanagedFunctionPointer( RuntimeClass.defaultCallingConvention )]
		delegate int pfnGetIid( IntPtr pThis, out Guid iid );

		static Func<IntPtr, object> getFactory( IntPtr nativePtr )
		{
			IntPtr vtable = Marshal.ReadIntPtr( nativePtr );
			Func<IntPtr, object> factory;
			lock( syncRoot )
			{
				if( vtables.TryGetValue( vtable, out factory ) )
					return factory;

				int offset = 5 * Marshal.SizeOf<IntPtr>();
				IntPtr getIid = Marshal.ReadIntPtr( vtable + offset );
				pfnGetIid pfn = Marshal.GetDelegateForFunctionPointer<pfnGetIid>( getIid );
				pfn( nativePtr, out Guid iid );

				if( interfaces.TryGetValue( iid, out factory ) )
				{
					vtables.Add( vtable, factory );
					return factory;
				}

				throw new ApplicationException( $"Unable to marshal IDeviceObject, unknown runtime type" );
			}
		}

		static IDeviceObject marshalNative( IntPtr nativePtr )
		{
			return (IDeviceObject)getFactory( nativePtr )( nativePtr );
		}

		public override Expressions managed( ParameterExpression eNative, bool isInput )
		{
			throw new NotSupportedException();
		}

		public override Expressions native( ParameterExpression eManaged, bool isInput )
		{
			if( isInput )
				throw new NotSupportedException();

			var eNative = Expression.Variable( typeof( IntPtr ) );
			var eWrap = Expression.Call( miMarshalNative, eNative );
			var eResult = Expression.Assign( eManaged, eWrap );
			return Expressions.output( eNative, eResult );
		}
	}
}