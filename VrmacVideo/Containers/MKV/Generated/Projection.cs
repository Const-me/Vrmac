using System;
using System.Collections.Generic;
using System.IO;

namespace VrmacVideo.Containers.MKV
{
	/// <summary>Describes the video projection details. Used to render spherical and VR videos.</summary>
	public sealed partial class Projection
	{
		/// <summary>Describes the projection used for this video track.</summary>
		public readonly eProjectionType projectionType = eProjectionType.Rectangular;
		/// <summary>Private data that only applies to a specific projection.<br/>Semantics<br/>If ProjectionType equals 0 (Rectangular), 			then this element must not be present.<br/>If ProjectionType equals 1 (Equirectangular), then this
		/// element must be present and contain the same binary data that would be stored inside 			an ISOBMFF Equirectangular Projection Box ('equi').<br/>If ProjectionType equals 2 (Cubemap), then this element must be present and
		/// contain the same binary data that would be stored 			inside an ISOBMFF Cubemap Projection Box ('cbmp').<br/>If ProjectionType equals 3 (Mesh), then this element must be present and contain the same binary data that
		/// would be stored inside 			an ISOBMFF Mesh Projection Box ('mshp').<br/>Note: ISOBMFF box size and fourcc fields are not included in the binary data, but the FullBox version and flag fields are. This is to avoid
		/// redundant framing information while preserving versioning and semantics between the two container formats.</summary>
		public readonly Blob projectionPrivate;
		/// <summary>Specifies a yaw rotation to the projection.<br/>Semantics<br/>Value represents a clockwise rotation, in degrees, around the up vector. This rotation must be applied before any ProjectionPosePitch or ProjectionPoseRoll
		/// rotations. The value of this field should be in the -180 to 180 degree range.</summary>
		public readonly double projectionPoseYaw = 0;
		/// <summary>Specifies a pitch rotation to the projection.<br/>Semantics<br/>Value represents a counter-clockwise rotation, in degrees, around the right vector. This rotation must be applied after the ProjectionPoseYaw rotation and
		/// before the ProjectionPoseRoll rotation. The value of this field should be in the -90 to 90 degree range.</summary>
		public readonly double projectionPosePitch = 0;
		/// <summary>Specifies a roll rotation to the projection.<br/>Semantics<br/>Value represents a counter-clockwise rotation, in degrees, around the forward vector. This rotation must be applied after the ProjectionPoseYaw and
		/// ProjectionPosePitch rotations. The value of this field should be in the -180 to 180 degree range.</summary>
		public readonly double projectionPoseRoll = 0;

		internal Projection( Stream stream )
		{
			ElementReader reader = new ElementReader( stream );
			while( !reader.EOF )
			{
				eElement id = reader.readElementId();
				switch( id )
				{
					case eElement.ProjectionType:
						projectionType = (eProjectionType)reader.readByte( 0 );
						break;
					case eElement.ProjectionPrivate:
						projectionPrivate = Blob.read( reader );
						break;
					case eElement.ProjectionPoseYaw:
						projectionPoseYaw = reader.readFloat( 0 );
						break;
					case eElement.ProjectionPosePitch:
						projectionPosePitch = reader.readFloat( 0 );
						break;
					case eElement.ProjectionPoseRoll:
						projectionPoseRoll = reader.readFloat( 0 );
						break;
					default:
						reader.skipElement();
						break;
				}
			}
		}
	}
}