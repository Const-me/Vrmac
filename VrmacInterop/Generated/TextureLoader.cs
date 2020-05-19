// This source file was automatically generated from "TextureLoader.h" C++ header by a custom tool.
// The tool is based on CppSharp https://github.com/mono/CppSharp which is based on clang AST https://clang.llvm.org/
#pragma warning disable CS1591	// CS1591: Missing XML comment for publicly visible type or member
using ComLight;
using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Numerics;

namespace Diligent.Graphics
{
	/// <summary>Texture loading information</summary>
	[StructLayout( LayoutKind.Sequential )]
	public struct TextureLoadInfo
	{
		/// <summary>Texture name passed over to the texture creation method</summary>
		public IntPtr Name;

		/// <summary>Usage</summary>
		public Usage Usage;

		/// <summary>Bind flags</summary>
		public BindFlags BindFlags;

		/// <summary>Number of mip levels</summary>
		public int MipLevels;

		/// <summary>CPU access flags</summary>
		public CpuAccessFlags CPUAccessFlags;

		byte m_IsSRGB;
		/// <summary>Flag indicating if this texture uses sRGB gamma encoding</summary>
		public bool IsSRGB
		{
			get => ( 0 != m_IsSRGB );
			set => m_IsSRGB = MiscUtils.byteFromBool( value );
		}

		byte m_GenerateMips;
		/// <summary>Flag indicating that the procedure should generate lower mip levels</summary>
		public bool GenerateMips
		{
			get => ( 0 != m_GenerateMips );
			set => m_GenerateMips = MiscUtils.byteFromBool( value );
		}

		/// <summary>Texture format</summary>
		public TextureFormat Format;

		/// <summary>To initialize this structure with default value, use this constructor. The argument value is ignored.</summary>
		/// <remarks>This is only here because C# doesn’t support parameterless constructors for structures.</remarks>
		public TextureLoadInfo( bool unused )
		{
			Name = IntPtr.Zero;
			Usage = Usage.Static;
			BindFlags = BindFlags.ShaderResource;
			MipLevels = 0;
			CPUAccessFlags = CpuAccessFlags.None;
			m_IsSRGB = 0;
			m_GenerateMips = 1;
			Format = TextureFormat.Unknown;
		}
	}
}
