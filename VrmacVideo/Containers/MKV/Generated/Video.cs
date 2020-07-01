using System;
using System.Collections.Generic;
using System.IO;

namespace VrmacVideo.Containers.MKV
{
	/// <summary>Video settings.</summary>
	public sealed partial class Video
	{
		/// <summary>A flag to declare if the video is known to be progressive or interlaced and if applicable to declare details about the interlacement.</summary>
		public readonly eFlagInterlaced flagInterlaced = eFlagInterlaced.Undetermined;
		/// <summary>Declare the field ordering of the video. If FlagInterlaced is not set to 1, this Element MUST be ignored.</summary>
		public readonly eFieldOrder fieldOrder = eFieldOrder.Undetermined;
		/// <summary>Alpha Video Mode. Presence of this Element indicates that the BlockAdditional Element could contain Alpha data.</summary>
		public readonly ulong alphaMode = 0;
		/// <summary>Width of the encoded video frames in pixels.</summary>
		public readonly uint pixelWidth;
		/// <summary>Height of the encoded video frames in pixels.</summary>
		public readonly uint pixelHeight;
		/// <summary>The number of video pixels to remove at the bottom of the image.</summary>
		public readonly uint pixelCropBottom = 0;
		/// <summary>The number of video pixels to remove at the top of the image.</summary>
		public readonly uint pixelCropTop = 0;
		/// <summary>The number of video pixels to remove on the left of the image.</summary>
		public readonly uint pixelCropLeft = 0;
		/// <summary>The number of video pixels to remove on the right of the image.</summary>
		public readonly uint pixelCropRight = 0;
		/// <summary>Width of the video frames to display. Applies to the video frame after cropping (PixelCrop* Elements).</summary>
		public readonly uint? displayWidth;
		/// <summary>Height of the video frames to display. Applies to the video frame after cropping (PixelCrop* Elements).</summary>
		public readonly uint? displayHeight;
		/// <summary>How DisplayWidth &amp; DisplayHeight are interpreted.</summary>
		public readonly eDisplayUnit displayUnit = eDisplayUnit.Pixels;
		/// <summary>Specify the possible modifications to the aspect ratio.</summary>
		public readonly eAspectRatioType aspectRatioType = eAspectRatioType.FreeResizing;
		/// <summary>Specify the pixel format used for the Track's data as a FourCC. This value is similar in scope to the biCompression value of AVI's BITMAPINFOHEADER.</summary>
		public readonly uint? colourSpace;
		/// <summary>Gamma Value.</summary>
		public readonly double? gammaValue;
		/// <summary>Number of frames per second. <strong>Informational</strong> only.</summary>
		public readonly double? frameRate;
		/// <summary>Settings describing the colour format.</summary>
		public readonly Colour colour;
		/// <summary>Describes the video projection details. Used to render spherical and VR videos.</summary>
		public readonly Projection projection;

		internal Video( Stream stream )
		{
			ElementReader reader = new ElementReader( stream );
			while( !reader.EOF )
			{
				eElement id = reader.readElementId();
				switch( id )
				{
					case eElement.FlagInterlaced:
						flagInterlaced = (eFlagInterlaced)reader.readByte( 0 );
						break;
					case eElement.FieldOrder:
						fieldOrder = (eFieldOrder)reader.readByte( 2 );
						break;
					case eElement.AlphaMode:
						alphaMode = reader.readUlong( 0 );
						break;
					case eElement.PixelWidth:
						pixelWidth = reader.readUint();
						break;
					case eElement.PixelHeight:
						pixelHeight = reader.readUint();
						break;
					case eElement.PixelCropBottom:
						pixelCropBottom = reader.readUint( 0 );
						break;
					case eElement.PixelCropTop:
						pixelCropTop = reader.readUint( 0 );
						break;
					case eElement.PixelCropLeft:
						pixelCropLeft = reader.readUint( 0 );
						break;
					case eElement.PixelCropRight:
						pixelCropRight = reader.readUint( 0 );
						break;
					case eElement.DisplayWidth:
						displayWidth = reader.readUint();
						break;
					case eElement.DisplayHeight:
						displayHeight = reader.readUint();
						break;
					case eElement.DisplayUnit:
						displayUnit = (eDisplayUnit)reader.readByte( 0 );
						break;
					case eElement.AspectRatioType:
						aspectRatioType = (eAspectRatioType)reader.readByte( 0 );
						break;
					case eElement.ColourSpace:
						colourSpace = reader.readColorSpace();
						break;
					case eElement.GammaValue:
						gammaValue = reader.readFloat();
						break;
					case eElement.FrameRate:
						frameRate = reader.readFloat();
						break;
					case eElement.Colour:
						colour = new Colour( stream );
						break;
					case eElement.Projection:
						projection = new Projection( stream );
						break;
					default:
						reader.skipElement();
						break;
				}
			}
		}
	}
}