namespace VrmacVideo.Linux
{
	/// <summary>Describes V4L2 device caps returned by <see cref="eControlCode.QUERYCAP"/>; the C++ type is v4l2_capability structure.</summary>
	/// <seealso href="https://www.kernel.org/doc/html/v4.19/media/uapi/v4l/vidioc-querycap.html?highlight=querycap#c.VIDIOC_QUERYCAP" />
	public unsafe struct sCapability
	{
		/// <summary>name of the driver module (e.g. "bttv")</summary>
		public fixed byte driver[ 16 ];
		/// <summary>name of the card (e.g. "Hauppauge WinTV")</summary>
		public fixed byte card[ 32 ];
		/// <summary>name of the bus (e.g. "PCI:" + pci_name(pci_dev) )</summary>
		public fixed byte bus_info[ 32 ];
		/// <summary>KERNEL_VERSION</summary>
		public uint version;
		/// <summary>capabilities of the physical device as a whole</summary>
		public eCapabilityFlags capabilities;
		/// <summary>capabilities accessed via this particular device (node)</summary>
		public eCapabilityFlags device_caps;
		/// <summary>reserved fields for future extensions</summary>
		public fixed uint reserved[ 3 ];
	}
}