namespace Vrmac.ModeSet
{
	/// <summary>A base class implementing iVideoSetup interface, useful when you only want to select one thing and don't care about others.</summary>
	public abstract class VideoSetup: iVideoSetup
	{
		/// <summary>Doesn't pick anything, returns -1</summary>
		public virtual int pickEglConfig( sEglConfig[] configs, int configsCount ) => -1;

		/// <summary>Doesn't pick anything, returns -1</summary>
		public virtual int pickDrmFormat( sDrmFormat[] available, int availableCount ) => -1;
	}
}