namespace VrmacVideo.Containers.MP4
{
	/// <summary>h.264 profiles</summary>
	/// <remarks>ISO/IEC 14496-10 Annex A "Profiles and levels"</remarks>
	public enum eAvcProfile: byte
	{
		// The first 3 of them are documented in the old ISO/IEC 14496-10 spec from 2004 I have found in public access somewhere.
		// The rest are from wikipedia: https://en.wikipedia.org/wiki/Advanced_Video_Coding#Profiles

		/// <summary>Baseline Profile = 0x42</summary>
		Baseline = 66,
		/// <summary>Main Profile = 0x4D</summary>
		Main = 77,

		/// <summary>Main Profile = 0x58</summary>
		Extended = 88,
		/// <summary>High Profile = 0x64</summary>
		High = 100,
		/// <summary>High 10 Profile, supports 10 bit/channel color depth</summary>
		High10 = 110,
		/// <summary>High 4:2:2 Profile; supports 4:2:2 chroma sampling format, and 10 bit/channel color depth</summary>
		High422P = 122,
		/// <summary>High 4:4:4 Predictive Profile; supports up to 4:4:4 chroma sampling, and 14 bits/channel color depth</summary>
		High444PP = 244,
		/// <summary>CAVLC 4:4:4 Intra Profile</summary>
		CAVLC = 44,
		/// <summary>Primarily targeting video conferencing, mobile, and surveillance applications, this profile builds on top of the Constrained Baseline profile</summary>
		ScalableBaseline = 83,
		/// <summary>Primarily targeting broadcast and streaming applications</summary>
		ScalableHigh = 85,
		/// <summary>Scalable High Intra profile, Rec. ITU-T H.264 Annex G, G.10.1.3</summary>
		ScalableHighIntra = 86,
		/// <summary>Two-view stereoscopic 3D video</summary>
		StereoHigh = 128,
		/// <summary>Supports two or more views using both inter-picture (temporal) and MVC inter-view prediction</summary>
		MultiviewHigh = 118,
		/// <summary>Multi-resolution Frame-Compatible (MFC) extension</summary>
		MFCHigh = 134,
		/// <summary>A profile for stereoscopic coding with two-layer resolution enhancement</summary>
		MFCDepthHigh = 135,

		/// <summary>Supports joint coding of depth map and video texture information</summary>
		MultiviewDepth = 138,
		/// <summary>An enhanced profile for combined multiview coding with depth information</summary>
		EnhancedMultiviewDepth = 139,
	}
}