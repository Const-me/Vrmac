namespace VrmacVideo.Containers.MKV
{
	/// <summary>Describes the projection used for this video track.</summary>
	public enum eProjectionType: byte
	{
		Rectangular = 0,
		Equirectangular = 1,
		Cubemap = 2,
		Mesh = 3,
	}
}