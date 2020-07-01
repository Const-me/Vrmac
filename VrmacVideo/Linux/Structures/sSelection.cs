#pragma warning disable CS0649
using System;
using Vrmac;

namespace VrmacVideo.Linux
{
	/// <summary>Selection targets</summary>
	/// <seealso href="https://www.kernel.org/doc/html/latest/media/uapi/v4l/v4l2-selection-targets.html" />
	enum eSelectionTarget: uint
	{
		/// <summary>Crop rectangle. Defines the cropped area.</summary>
		Crop = 0,
		CropDefault = 1,
		CropBounds = 2,
		NativeSize = 3,
		Compose = 0x100,
		ComposeDefault = 0x101,
		ComposeBounds = 0x102,
		ComposePadded = 0x103,
	}

	/// <summary>Selection flags</summary>
	/// <seealso href="https://www.kernel.org/doc/html/latest/media/uapi/v4l/v4l2-selection-flags.html#v4l2-selection-flags" />
	[Flags]
	enum eSelectionFlags: uint
	{
		/// <summary>Suggest the driver it should choose greater or equal rectangle (in size) than was requested.
		/// Albeit the driver may choose a lesser size, it will only do so due to hardware limitations.
		/// Without this flag (and <see cref="LesserOrEqual" />) the behavior is to choose the closest possible rectangle.</summary>
		GreaterOrEqual = 1,

		/// <summary>Suggest the driver it should choose lesser or equal rectangle (in size) than was requested. Albeit the driver may choose a greater size, it will only do so due to hardware limitations.</summary>
		LesserOrEqual = 2,

		/// <summary>The configuration must not be propagated to any further processing steps. If this flag is not given, the configuration is propagated inside the subdevice to all further processing steps.</summary>
		KeepConfig = 4,
	}

	struct sRect
	{
		public int left, top, width, height;

		public static implicit operator sRect( CRect rc )
		{
			CSize size = rc.size;
			return new sRect()
			{
				left = rc.left,
				top = rc.top,
				width = size.cx,
				height = size.cy
			};
		}

		public static implicit operator CRect( sRect wr )
		{
			return new CRect( new CPoint( wr.left, wr.top ), new CSize( wr.width, wr.height ) );
		}
	}

	/// <seealso href="https://www.kernel.org/doc/html/latest/media/uapi/v4l/vidioc-g-selection.html#c.v4l2_selection" />
	unsafe struct sSelection
	{
		/// <summary>Type of the buffer</summary>
		public eBufferType type;

		/// <summary>Used to select between cropping and composing rectangles</summary>
		public eSelectionTarget target;

		/// <summary>Flags controlling the selection rectangle adjustments</summary>
		public eSelectionFlags flags;

		sRect m_rect;
		/// <summary>The selection rectangle.</summary>
		public CRect rect
		{
			get => m_rect;
			set => m_rect = value;
		}

		fixed uint reserved[ 9 ];

		public override string ToString() => $"type { type }, target { target }, flags { flags }, rect { rect }";
	}
}