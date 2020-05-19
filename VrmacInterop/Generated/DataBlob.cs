// This source file was automatically generated from "DataBlob.h" C++ header by a custom tool.
// The tool is based on CppSharp https://github.com/mono/CppSharp which is based on clang AST https://clang.llvm.org/
#pragma warning disable CS1591	// CS1591: Missing XML comment for publicly visible type or member
using ComLight;
using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Numerics;

namespace Diligent.Graphics
{
	/// <summary>Base interface for a file stream</summary>
	[ComInterface( "f578ff0d-abd2-4514-9d32-7cb454d4a73b", eMarshalDirection.ToManaged ), CustomConventions( typeof( Vrmac.Utils.NativeErrorMessages ) )]
	public interface IDataBlob: IDisposable
	{
		/// <summary>Sets the size of the internal data buffer</summary>
		void Resize( IntPtr NewSize );

		/// <summary>Returns the size of the internal data buffer</summary>
		[RetValIndex] IntPtr GetSize();

		/// <summary>Returns the pointer to the internal data buffer</summary>
		[RetValIndex] IntPtr GetDataPtr();
	}
}
