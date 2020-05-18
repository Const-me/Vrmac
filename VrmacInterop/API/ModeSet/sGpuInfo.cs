using System;
using System.Runtime.InteropServices;

namespace Vrmac.ModeSet
{
	/// <summary>Information about the GPU</summary>
	[StructLayout( LayoutKind.Sequential )]
	public struct sGpuInfo
	{
		IntPtr m_description;
		/// <summary>A description returned by the OS</summary>
		public string description => MiscUtils.stringFromPointer( m_description );

		/// <summary>On Linux it's zero because the OS lacks the API to query that information.</summary>
		public ulong DedicatedVideoMemory;
		/// <summary>On Linux it's zero because the OS lacks the API to query that information.</summary>
		public ulong DedicatedSystemMemory;
		/// <summary>On Linux it's zero because the OS lacks the API to query that information.</summary>
		public ulong SharedSystemMemory;

		/// <summary>Vendor and device ID. On Linux it's all zeros, because there's no sane API or protocol to enumerate hardware devices.</summary>
		public ushort VendorId, DeviceId;
		/// <summary>Count of output connectors</summary>
		public int numConnectors;
		/// <summary>Minimum and maximum resolution in pixels</summary>
		public CSize resolutionMin, resolutionMax;

		/// <summary>Returns a string that represents the current object.</summary>
		public override string ToString()
		{
			return $"\"{ description }\", numConnectors = { numConnectors }";
		}
	}
}