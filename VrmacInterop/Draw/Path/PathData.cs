// TODO: comment them
#pragma warning disable CS1591  // CS1591: Missing XML comment for publicly visible type or member
using Diligent.Graphics;
using System.Numerics;
using System.Runtime.InteropServices;

namespace Vrmac.Draw
{
	/// <summary></summary>
	public enum eSegmentKind: byte
	{
		/// <summary>Line segment, 2 floats / point</summary>
		Line,
		/// <summary>Circular ark segment, 5 floats / point</summary>
		Arc,
		/// <summary>Cubic Bezier segment, 6 floats / point</summary>
		Bezier,
		/// <summary>Quadratic Bezier segment, 4 floats / point</summary>
		QuadraticBezier,
	}

	/// <summary>A continuous segment of a path, with the same kind and flags of the points.</summary>
	/// <remarks>E.g. any polygon only has a single segment, with kind "Line" and all the points.</remarks>
	[StructLayout( LayoutKind.Sequential )]
	public struct sPathSegment
	{
		/// <summary>How the floats are interpreted</summary>
		public eSegmentKind kind;
		/// <summary>For circular arcs <see cref="eArcFlags" />, otherwise zero.</summary>
		public byte flags;
		/// <summary>Count of points in the segment</summary>
		public int pointsCount;
	}

	[StructLayout( LayoutKind.Sequential )]
	public struct sPathFigure
	{
		public Vector2 startingPoint;
		public int segmentsCount;
		byte m_filled, m_closed;

		public bool isFilled
		{
			get
			{
				return 0 != m_filled;
			}
			set
			{
				m_filled = value ? (byte)1 : (byte)0;
			}
		}

		public bool isClosed
		{
			get
			{
				return 0 != m_closed;
			}
			set
			{
				m_closed = value ? (byte)1 : (byte)0;
			}
		}
	}

	[StructLayout( LayoutKind.Sequential )]
	public struct sArcData
	{
		public Vector2 endPoint;
		public Vector2 size;
		public float angleDegrees;
	}

	[StructLayout( LayoutKind.Sequential )]
	public struct sPathData
	{
		public int figuresCount;
		public eFillMode fillMode;
	}
}