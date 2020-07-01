using System;

namespace VrmacVideo.IO
{
	/// <summary>Protections are chosen from these bits, OR'd together. The implementation does not necessarily support PROT_EXEC or PROT_WRITE without PROT_READ.</summary>
	[Flags]
	public enum eMemoryProtection: int
	{
		None = 0,
		/// <summary>Page can be read, PROT_READ</summary>
		Read = 1,
		/// <summary>Page can be written, PROT_WRITE</summary>
		Write = 2,
		/// <summary>Page can be executed, PROT_EXEC</summary>
		Execute = 4,

		/// <summary>Enables both read and write access to that memory</summary>
		ReadWrite = Read | Write,
	}

	[Flags]
	public enum eMapFlags: int
	{
		// Sharing types (must choose one and only one of these)

		/// <summary>Share changes</summary>
		Shared = 1,
		/// <summary>Changes are private</summary>
		Private = 2,

		// Other flags

		/// <summary>Interpret addr exactly</summary>
		Fixed = 0x10,
		/// <summary>Don't use a file</summary>
		Anonymous = 0x20,

		/// <summary>populate (prefault) pagetables, MAP_POPULATE</summary>
		Populate = 0x8000,

		/// <summary>create a huge page mapping, MAP_HUGETLB</summary>
		HugePage = 0x40000,
	}
}