using System;
using System.Runtime.InteropServices;

namespace VrmacVideo.IO.AAC
{
	enum eModuleId: int
	{
		None = 0,
		Tools = 1,
		SysLib = 2,
		AacDec = 3,
		AacEnc = 4,
		SbrDec = 5,
		SbrEnc = 6,
		TpDec = 7,
		TpEnc = 8,
		MpsDec = 9,
		MpegFileRead = 10,
		MpegFileWrite = 11,
		Mp2Dec = 12,
		DabDec = 13,
		DabParse = 14,
		DrmDec = 15,
		DrmParse = 16,
		AacldEnc = 17,
		Mp2Enc = 18,
		Mp3Enc = 19,
		Mp3Dec = 20,
		Mp3Headphone = 21,
		Mp3sDec = 22,
		Mp3sEnc = 23,
		Eaec = 24,
		DabEnc = 25,
		DmbDec = 26,
		FdReverb = 27,
		DrmEnc = 28,
		MetadataTranscoder = 29,
		Ac3Dec = 30,
		PcmDmx = 31,
	}

	/* unsafe struct LibraryInfo
	{
		readonly IntPtr m_title, m_build_date, m_build_time;
		public string title => Marshal.PtrToStringUTF8( m_title );
		public string buildDate => Marshal.PtrToStringUTF8( m_build_date );
		public string buildTime => Marshal.PtrToStringUTF8( m_build_time );

		public readonly eModuleId moduleId;
		public readonly int version;
		public readonly uint flags;
		fixed byte m_versionStr[ 32 ];
		public string versionString
		{
			get
			{
				unsafe
				{
					fixed ( byte* pointer = m_versionStr )
						return StringMarshal.copy( pointer, 32 );
				}
			}
		}

		public override string ToString() =>
			$"title { title }, buildDate { buildDate }, buildTime { buildTime }, moduleId { moduleId }, version { version }, flags { flags.ToString( "x" ) }, versionString { versionString }";
	} */
}