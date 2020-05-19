// This source file was automatically generated from "Shader.h" C++ header by a custom tool.
// The tool is based on CppSharp https://github.com/mono/CppSharp which is based on clang AST https://clang.llvm.org/
#pragma warning disable CS1591	// CS1591: Missing XML comment for publicly visible type or member
using ComLight;
using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Numerics;

namespace Diligent.Graphics
{
	/// <summary>Describes the shader type</summary>
	[Flags]
	public enum ShaderType : uint
	{
		/// <summary>Unknown shader type</summary>
		Unknown = 0x0,
		/// <summary>Vertex shader</summary>
		Vertex = 0x1,
		/// <summary>Pixel (fragment) shader</summary>
		Pixel = 0x2,
		/// <summary>Geometry shader</summary>
		Geometry = 0x4,
		/// <summary>Hull (tessellation control) shader</summary>
		Hull = 0x8,
		/// <summary>Domain (tessellation evaluation) shader</summary>
		Domain = 0x10,
		/// <summary>Compute shader</summary>
		Compute = 0x20
	}

	/// <summary>Describes shader source code language</summary>
	public enum ShaderSourceLanguage : uint
	{
		/// <summary>Default language (GLSL for OpenGL/OpenGLES devices, HLSL for Direct3D11/Direct3D12 devices)</summary>
		Default = 0,
		/// <summary>The source language is HLSL</summary>
		Hlsl = 1,
		/// <summary>The source language is GLSL</summary>
		Glsl = 2
	}

	/// <summary>Describes shader resource type</summary>
	public enum ShaderResourceType : byte
	{
		/// <summary>Shader resource type is unknown</summary>
		Unknown = 0,
		/// <summary>Constant (uniform) buffer</summary>
		ConstantBuffer = 1,
		/// <summary>Shader resource view of a texture (sampled image)</summary>
		TextureSrv = 2,
		/// <summary>Shader resource view of a buffer (read-only storage image)</summary>
		BufferSrv = 3,
		/// <summary>Unordered access view of a texture (sotrage image)</summary>
		TextureUav = 4,
		/// <summary>Unordered access view of a buffer (storage buffer)</summary>
		BufferUav = 5,
		/// <summary>Sampler (separate sampler)</summary>
		Sampler = 6
	}

	/// <summary>Shader description</summary>
	[StructLayout( LayoutKind.Sequential )]
	public struct ShaderDesc
	{
		/// <summary>Structures in C# can’t inherit from other structures. Using encapsulation instead.</summary>
		public DeviceObjectAttribs baseStruct;

		/// <summary>Shader type. See Diligent::SHADER_TYPE.</summary>
		public ShaderType ShaderType;

		/// <summary>To initialize this structure with default value, use this constructor. The argument value is ignored.</summary>
		/// <remarks>This is only here because C# doesn’t support parameterless constructors for structures.</remarks>
		public ShaderDesc( bool unused )
		{
			baseStruct = new DeviceObjectAttribs( true );
			ShaderType = ShaderType.Unknown;
		}
	}

	[StructLayout( LayoutKind.Sequential )]
	public struct ShaderMacro
	{
		public IntPtr Name;

		public IntPtr Definition;

		/// <summary>To initialize this structure with default value, use this constructor. The argument value is ignored.</summary>
		/// <remarks>This is only here because C# doesn’t support parameterless constructors for structures.</remarks>
		public ShaderMacro( bool unused )
		{
			Name = IntPtr.Zero;
			Definition = IntPtr.Zero;
		}
	}

	/// <summary>Shader version</summary>
	[StructLayout( LayoutKind.Sequential )]
	public struct ShaderVersion
	{
		/// <summary>Major revision</summary>
		public byte Major;

		/// <summary>Minor revision</summary>
		public byte Minor;

		/// <summary>To initialize this structure with default value, use this constructor. The argument value is ignored.</summary>
		/// <remarks>This is only here because C# doesn’t support parameterless constructors for structures.</remarks>
		public ShaderVersion( bool unused )
		{
			Major = 0;
			Minor = 0;
		}
	}

	/// <summary>Shader resource description</summary>
	[StructLayout( LayoutKind.Sequential )]
	public struct ShaderResourceDesc
	{
		/// <summary>Shader resource name</summary>
		public IntPtr Name;

		/// <summary>Shader resource type, see Diligent::SHADER_RESOURCE_TYPE.</summary>
		public ShaderResourceType Type;

		/// <summary>Array size. For non-array resource this value is 1.</summary>
		public int ArraySize;

		/// <summary>To initialize this structure with default value, use this constructor. The argument value is ignored.</summary>
		/// <remarks>This is only here because C# doesn’t support parameterless constructors for structures.</remarks>
		public ShaderResourceDesc( bool unused )
		{
			Name = IntPtr.Zero;
			Type = ShaderResourceType.Unknown;
			ArraySize = 0;
		}
	}

	/// <summary>Shader interface</summary>
	[ComInterface( "2989b45c-143d-4886-b89c-c3271c2dcc5d", eMarshalDirection.ToManaged ), CustomConventions( typeof( Vrmac.Utils.NativeErrorMessages ) )]
	public interface IShader: IDeviceObject
	{
		/// <summary>Get interface ID of the top-level IDeviceObject-based interface implemented by the object.</summary>
		new void getIid( out Guid iid );
		/// <summary>Returns unique identifier assigned to an object</summary>
		new int GetUniqueID();
		/// <summary>Cast interface pointer from interop to native COM interface</summary>
		new IntPtr nativeCast();
		[RetValIndex] ShaderDesc GetDesc();


		/// <summary>Returns the total number of shader resources</summary>
		[RetValIndex] int GetResourceCount();

		/// <summary>Returns the pointer to the array of shader resources</summary>
		[RetValIndex( 1 )] ShaderResourceDesc GetResourceDesc( int Index );

		/// <summary>Shader GUID is an MD4 hash of the shader’s source data, the subset of that data which affects the output. Name doesn’t affect the output and is not hashed.</summary>
		[RetValIndex] Guid getGuid();
	}
}
