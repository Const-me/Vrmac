namespace Vrmac.MediaEngine
{
	/// <summary>Specifies the likelihood that the Media Engine can play a specified type of media resource.</summary>
	public enum eCanPlay: byte
	{
		/// <summary>The Media Engine cannot play the resource.</summary>
		NotSupported = 0,
		/// <summary>The Media Engine might be able to play the resource.</summary>
		Maybe = 1,
		/// <summary>The Media Engine can probably play the resource.</summary>
		Probably = 2,
	}
}