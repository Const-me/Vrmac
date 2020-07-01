using System.Runtime.InteropServices;
using Vrmac;

namespace VrmacVideo.Linux
{
	public enum eFrameSizeType: int
	{
		/// <summary>Discrete frame size.</summary>
		Discrete = 1,
		/// <summary>Continuous frame size.</summary>
		Continuous = 2,
		/// <summary>Step-wise defined frame size.</summary>
		Stepwise = 3,
	}

	public struct sFrameSizeStepwise
	{
		public int minWidth;
		public int maxWidth;
		public int stepWidth;
		public int minHeight;
		public int maxHeight;
		public int stepHeight;
	}

	/// <summary>The C++ type is v4l2_frmsizeenum structure</summary>
	/// <seealso href="https://www.kernel.org/doc/html/v4.19/media/uapi/v4l/vidioc-enum-framesizes.html?highlight=v4l2_frmsizeenum#c.v4l2_frmsizeenum" />
	[StructLayout( LayoutKind.Sequential, Pack = 4 )]
	public unsafe struct sFrameSizeEnum
	{
		[StructLayout( LayoutKind.Explicit, Size = 6 * 4 )]
		struct sUnion
		{
			[FieldOffset( 0 )] public CSize discreteFrameSize;
			[FieldOffset( 0 )] public sFrameSizeStepwise stepwise;
		}

		/// <summary>Index of the given frame size in the enumeration.</summary>
		public int index;
		/// <summary>Pixel format for which the frame sizes are enumerated.</summary>
		public ePixelFormat pixelFormat;
		/// <summary>Frame size type the device supports.</summary>
		public eFrameSizeType type;

		sUnion m_union;
		/// <summary>Discrete frame size with the given index</summary>
		public CSize discreteFrameSize => m_union.discreteFrameSize;
		/// <summary>Stepwise frame size with the given index</summary>
		public sFrameSizeStepwise stepwise => m_union.stepwise;

		/// <summary>Reserved space for future use</summary>
		fixed uint reserved[ 2 ];
	}
}