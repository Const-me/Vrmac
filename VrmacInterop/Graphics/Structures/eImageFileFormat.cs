namespace Diligent.Graphics
{
	/// <summary>Image file format</summary>
	public enum eImageFileFormat: int
	{
		/// <summary>Unknown format</summary>
		Unknown = 0,
		/// <summary>The image is encoded in JPEG format</summary>
		JPEG,
		/// <summary>The image is encoded in PNG format</summary>
		PNG,
		/// <summary>The image is encoded in TIFF format</summary>
		TIFF,
		/// <summary>DDS file</summary>
		DDS,
		/// <summary>KTX file</summary>
		KTX
	}
}