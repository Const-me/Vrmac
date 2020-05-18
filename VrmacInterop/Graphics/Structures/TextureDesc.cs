using System.Runtime.InteropServices;
using Vrmac;

namespace Diligent.Graphics
{
	/// <summary>Texture description</summary>
	[StructLayout( LayoutKind.Sequential )]
	public struct TextureDesc
	{
		/// <summary>C# structures don't support inheritance, emulating with encapsulation.</summary>
		public DeviceObjectAttribs baseStruct;

		/// <summary>Texture type. See Diligent::RESOURCE_DIMENSION for details.</summary>
		public ResourceDimension Type;

		/// <summary>Texture size in pixels.</summary>
		public CSize Size;

		/// <summary>For a 1D array or 2D array, number of array slices. For a 3D texture, number of depth slices.</summary>
		public uint ArraySizeOrDepth;

		/// <summary>Texture format, see Diligent::TEXTURE_FORMAT.</summary>
		public TextureFormat Format;

		/// <summary>
		/// <para>Number of Mip levels in the texture. Multisampled textures can only have 1 Mip level.</para>
		/// <para>Specify 0 to create full mipmap chain.</para>
		/// </summary>
		public uint MipLevels;

		/// <summary>
		/// <para>Number of samples.</para>
		/// <para>Only 2D textures or 2D texture arrays can be multisampled.</para>
		/// </summary>
		public uint SampleCount;

		/// <summary>Texture usage. See Diligent::USAGE for details.</summary>
		public Usage Usage;

		/// <summary>
		/// <para>Bind flags, see Diligent::BIND_FLAGS for details.</para>
		/// <para>The following bind flags are allowed:</para>
		/// <para>Diligent::BIND_SHADER_RESOURCE, Diligent::BIND_RENDER_TARGET, Diligent::BIND_DEPTH_STENCIL,</para>
		/// <para>Diligent::and BIND_UNORDERED_ACCESS.</para>
		/// <para>Multisampled textures cannot have Diligent::BIND_UNORDERED_ACCESS flag set</para>
		/// </summary>
		public BindFlags BindFlags;

		/// <summary>
		/// <para>CPU access flags or 0 if no CPU access is allowed,</para>
		/// <para>see Diligent::CPU_ACCESS_FLAGS for details.</para>
		/// </summary>
		public CpuAccessFlags CPUAccessFlags;

		/// <summary>Miscellaneous flags, see Diligent::MISC_TEXTURE_FLAGS for details.</summary>
		public MiscTextureFlags MiscFlags;

		/// <summary>Optimized clear value</summary>
		public OptimizedClearValue ClearValue;

		/// <summary>Defines which command queues this texture can be used with</summary>
		public ulong CommandQueueMask;

		/// <summary>Create and set fields to their defaults</summary>
		public TextureDesc( bool unused )
		{
			baseStruct = new DeviceObjectAttribs( true );
			Type = ResourceDimension.Undefined;
			Size = new CSize( 0, 0 );
			ArraySizeOrDepth = 1;
			Format = TextureFormat.Unknown;
			MipLevels = 1;
			SampleCount = 1;
			Usage = Usage.Default;
			BindFlags = BindFlags.None;
			CPUAccessFlags = CpuAccessFlags.None;
			MiscFlags = MiscTextureFlags.None;
			ClearValue = new OptimizedClearValue( true );
			CommandQueueMask = 1;
		}
	}
}