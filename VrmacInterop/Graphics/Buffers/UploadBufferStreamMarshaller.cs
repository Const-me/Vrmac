using ComLight;
using ComLight.Marshalling;
using System;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;

namespace Diligent.Graphics.Buffers
{
	class UploadBufferStreamMarshaller: iCustomMarshal
	{
		public override Type getNativeType( ParameterInfo managedParameter )
		{
			Type managed = managedParameter.ParameterType;
			if( managed == typeof( Stream ).MakeByRefType() )
			{
				if( managedParameter.IsIn )
					throw new ArgumentException( "COM interfaces can only be marshaled in or out, ref is not supported for them" );
				return typeof( IntPtr ).MakeByRefType();
			}
			throw new ApplicationException( @"UploadBufferStreamMarshaller is only compatible with `out Stream` arguments" );
		}

		static readonly MethodInfo miWrapNative = typeof( UploadBufferStreamMarshaller ).GetMethod( "createWrapper", BindingFlags.Static | BindingFlags.NonPublic );

		static Stream createWrapper( IntPtr nativeComPointer )
		{
			return new BufferUploadStream( nativeComPointer );
		}

		public override Expressions native( ParameterExpression eManaged, bool isInput )
		{
			if( isInput )
				throw new NotSupportedException();

			var eNative = Expression.Variable( typeof( IntPtr ) );
			var eWrap = Expression.Call( miWrapNative, eNative );
			var eResult = Expression.Assign( eManaged, eWrap );
			return Expressions.output( eNative, eResult );
		}
	}
}