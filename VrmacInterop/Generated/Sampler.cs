// This source file was automatically generated from "Sampler.h" C++ header by a custom tool.
// The tool is based on CppSharp https://github.com/mono/CppSharp which is based on clang AST https://clang.llvm.org/
#pragma warning disable CS1591	// CS1591: Missing XML comment for publicly visible type or member
using ComLight;
using System;
using System.Runtime.InteropServices;
using System.Security;

namespace Diligent.Graphics
{
	/// <summary>
	/// <para>This structure describes the sampler state which is used in a call to</para>
	/// <para>IRenderDevice::CreateSampler() to create a sampler object.</para>
	/// </summary>
	/// <remarks>
	/// <para>To create an anisotropic filter, all three filters must either be Diligent::FILTER_TYPE_ANISOTROPIC</para>
	/// <para>or Diligent::FILTER_TYPE_COMPARISON_ANISOTROPIC.</para>
	/// <para>MipFilter cannot be comparison filter except for Diligent::FILTER_TYPE_ANISOTROPIC if all</para>
	/// <para>three filters have that value.</para>
	/// <para>Both MinFilter and MagFilter must either be regular filters or comparison filters.</para>
	/// <para>Mixing comparison and regular filters is an error.</para>
	/// </remarks>
	[StructLayout( LayoutKind.Sequential )]
	public struct SamplerDesc
	{
		/// <summary>Structures in C# can’t inherit from other structures. Using encapsulation instead.</summary>
		public DeviceObjectAttribs baseStruct;

		/// <summary>
		/// <para>Texture minification filter, see Diligent::FILTER_TYPE for details.</para>
		/// <para>Default value: Diligent::FILTER_TYPE_LINEAR.</para>
		/// </summary>
		public FilterType MinFilter;

		/// <summary>
		/// <para>Texture magnification filter, see Diligent::FILTER_TYPE for details.</para>
		/// <para>Default value: Diligent::FILTER_TYPE_LINEAR.</para>
		/// </summary>
		public FilterType MagFilter;

		/// <summary>
		/// <para>Mip filter, see Diligent::FILTER_TYPE for details.</para>
		/// <para>Only FILTER_TYPE_POINT, FILTER_TYPE_LINEAR, FILTER_TYPE_ANISOTROPIC, and</para>
		/// <para>FILTER_TYPE_COMPARISON_ANISOTROPIC are allowed.</para>
		/// <para>Default value: Diligent::FILTER_TYPE_LINEAR.</para>
		/// </summary>
		public FilterType MipFilter;

		/// <summary>
		/// <para>Texture address mode for U coordinate, see Diligent::TEXTURE_ADDRESS_MODE for details</para>
		/// <para>Default value: Diligent::TEXTURE_ADDRESS_CLAMP.</para>
		/// </summary>
		public TextureAddressMode AddressU;

		/// <summary>
		/// <para>Texture address mode for V coordinate, see Diligent::TEXTURE_ADDRESS_MODE for details</para>
		/// <para>Default value: Diligent::TEXTURE_ADDRESS_CLAMP.</para>
		/// </summary>
		public TextureAddressMode AddressV;

		/// <summary>
		/// <para>Texture address mode for W coordinate, see Diligent::TEXTURE_ADDRESS_MODE for details</para>
		/// <para>Default value: Diligent::TEXTURE_ADDRESS_CLAMP.</para>
		/// </summary>
		public TextureAddressMode AddressW;

		/// <summary>
		/// <para>Offset from the calculated mipmap level. For example, if a sampler calculates that a texture</para>
		/// <para>should be sampled at mipmap level 1.2 and MipLODBias is 2.3, then the texture will be sampled at</para>
		/// <para>mipmap level 3.5. Default value: 0.</para>
		/// </summary>
		public float MipLODBias;

		/// <summary>Maximum anisotropy level for the anisotropic filter. Default value: 0.</summary>
		public int MaxAnisotropy;

		/// <summary>
		/// <para>A function that compares sampled data against existing sampled data when comparsion</para>
		/// <para>filter is used. Default value: Diligent::COMPARISON_FUNC_NEVER.</para>
		/// </summary>
		public ComparisonFunction ComparisonFunc;

		/// <summary>
		/// <para>Border color to use if TEXTURE_ADDRESS_BORDER is specified for AddressU, AddressV, or AddressW.</para>
		/// <para>Default value: {0,0,0,0}</para>
		/// </summary>
		public Vector4 BorderColor;

		/// <summary>
		/// <para>Specifies the minimum value that LOD is clamped to before accessing the texture MIP levels.</para>
		/// <para>Must be less than or equal to MaxLOD.</para>
		/// <para>Default value: 0.</para>
		/// </summary>
		public float MinLOD;

		/// <summary>
		/// <para>Specifies the maximum value that LOD is clamped to before accessing the texture MIP levels.</para>
		/// <para>Must be greater than or equal to MinLOD.</para>
		/// <para>Default value: +FLT_MAX.</para>
		/// </summary>
		public float MaxLOD;

		/// <summary>To initialize this structure with default value, use this constructor. The argument value is ignored.</summary>
		/// <remarks>This is only here because C# doesn’t support parameterless constructors for structures.</remarks>
		public SamplerDesc( bool unused )
		{
			baseStruct = new DeviceObjectAttribs( true );
			MinFilter = FilterType.Linear;
			MagFilter = FilterType.Linear;
			MipFilter = FilterType.Linear;
			AddressU = TextureAddressMode.Clamp;
			AddressV = TextureAddressMode.Clamp;
			AddressW = TextureAddressMode.Clamp;
			MipLODBias = 0;
			MaxAnisotropy = 0;
			ComparisonFunc = ComparisonFunction.Never;
			BorderColor = Vector4.Zero;
			MinLOD = 0;
			MaxLOD = float.MaxValue;
		}
	}

	/// <summary>
	/// <para>The interface holds the sampler state that can be used to perform texture filtering.</para>
	/// <para>To create a sampler, call IRenderDevice::CreateSampler(). To use a sampler,</para>
	/// <para>call ITextureView::SetSampler().</para>
	/// </summary>
	[ComInterface( "595a59bf-fa81-4855-bc5e-c0e048745a95", eMarshalDirection.ToManaged ), CustomConventions( typeof( Vrmac.Utils.NativeErrorMessages ) )]
	public interface ISampler: IDeviceObject
	{
		/// <summary>Get interface ID of the top-level IDeviceObject-based interface implemented by the object.</summary>
		new void getIid( out Guid iid );
		/// <summary>Returns unique identifier assigned to an object</summary>
		new int GetUniqueID();
		/// <summary>Cast interface pointer from interop to native COM interface</summary>
		new IntPtr nativeCast();
		[RetValIndex] SamplerDesc GetDesc();

	}
}
