// TODO: comment them
#pragma warning disable CS1591  // CS1591: Missing XML comment for publicly visible type or member

namespace Vrmac.Input.Linux
{
	/// <summary>Codes for <see cref="eEventType.Relative" /> events</summary>
	public enum eRelativeAxis: ushort
	{
		X = 0,
		Y = 1,
		Z = 2,
		RX = 3,
		RY = 4,
		RZ = 5,
		HorizontalWheel = 6,
		Dial = 7,
		VerticalWheel = 8,
		Misc = 9
	}
}