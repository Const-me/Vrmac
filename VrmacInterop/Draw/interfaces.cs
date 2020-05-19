using System;
using System.Numerics;

namespace Vrmac.Draw
{
	/// <summary>Represents a geometry resource</summary>
	public interface iGeometry: IDisposable
	{
		/// <summary>Get geometry bounding box, after the specified transform is applied</summary>
		Rect getBounds( ref Matrix3x2 transform );
	}

	/// <summary>Defines an object that paints an area.</summary>
	public interface iBrush: IDisposable
	{ }

	/// <summary>Describes the caps, miter limit, line join, and dash information for a stroke.</summary>
	public interface iStrokeStyle: IDisposable
	{
		/// <summary>Describes the stroke that outlines a shape.</summary>
		sStrokeStyle strokeStyle { get; }
	}

	/// <summary>Encapsulates a device- and transform-dependent representation of a filled or stroked geometry.</summary>
	public interface iGeometryRealization: IDisposable
	{ }
}