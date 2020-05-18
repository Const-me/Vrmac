namespace Vrmac.Utils
{
	/// <summary>Built-in mouse cursors</summary>
	public enum eCursor: byte
	{
		/// <summary>No cursor whatsoever</summary>
		None = 0,
		/// <summary>Arrow cursor</summary>
		Arrow = 1,
		/// <summary>Beam cursor, often used to select text.</summary>
		Beam = 2,
		/// <summary>Hand cursor, often used on hyper links</summary>
		Hand = 3,
		/// <summary>Arrow + hourglass animated cursor</summary>
		Working = 4,
		/// <summary>Hourglass animated cursor without arrows</summary>
		Busy = 5,
	}
}