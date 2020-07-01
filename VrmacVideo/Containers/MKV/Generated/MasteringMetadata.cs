using System;
using System.Collections.Generic;
using System.IO;

namespace VrmacVideo.Containers.MKV
{
	/// <summary>SMPTE 2086 mastering data.</summary>
	public sealed partial class MasteringMetadata
	{
		/// <summary>Red X chromaticity coordinate as defined by CIE 1931.</summary>
		public readonly double? primaryRChromaticityX;
		/// <summary>Red Y chromaticity coordinate as defined by CIE 1931.</summary>
		public readonly double? primaryRChromaticityY;
		/// <summary>Green X chromaticity coordinate as defined by CIE 1931.</summary>
		public readonly double? primaryGChromaticityX;
		/// <summary>Green Y chromaticity coordinate as defined by CIE 1931.</summary>
		public readonly double? primaryGChromaticityY;
		/// <summary>Blue X chromaticity coordinate as defined by CIE 1931.</summary>
		public readonly double? primaryBChromaticityX;
		/// <summary>Blue Y chromaticity coordinate as defined by CIE 1931.</summary>
		public readonly double? primaryBChromaticityY;
		/// <summary>White X chromaticity coordinate as defined by CIE 1931.</summary>
		public readonly double? whitePointChromaticityX;
		/// <summary>White Y chromaticity coordinate as defined by CIE 1931.</summary>
		public readonly double? whitePointChromaticityY;
		/// <summary>Maximum luminance. Represented in candelas per square meter (cd/m²).</summary>
		public readonly double? luminanceMax;
		/// <summary>Minimum luminance. Represented in candelas per square meter (cd/m²).</summary>
		public readonly double? luminanceMin;

		internal MasteringMetadata( Stream stream )
		{
			ElementReader reader = new ElementReader( stream );
			while( !reader.EOF )
			{
				eElement id = reader.readElementId();
				switch( id )
				{
					case eElement.PrimaryRChromaticityX:
						primaryRChromaticityX = reader.readFloat();
						break;
					case eElement.PrimaryRChromaticityY:
						primaryRChromaticityY = reader.readFloat();
						break;
					case eElement.PrimaryGChromaticityX:
						primaryGChromaticityX = reader.readFloat();
						break;
					case eElement.PrimaryGChromaticityY:
						primaryGChromaticityY = reader.readFloat();
						break;
					case eElement.PrimaryBChromaticityX:
						primaryBChromaticityX = reader.readFloat();
						break;
					case eElement.PrimaryBChromaticityY:
						primaryBChromaticityY = reader.readFloat();
						break;
					case eElement.WhitePointChromaticityX:
						whitePointChromaticityX = reader.readFloat();
						break;
					case eElement.WhitePointChromaticityY:
						whitePointChromaticityY = reader.readFloat();
						break;
					case eElement.LuminanceMax:
						luminanceMax = reader.readFloat();
						break;
					case eElement.LuminanceMin:
						luminanceMin = reader.readFloat();
						break;
					default:
						reader.skipElement();
						break;
				}
			}
		}
	}
}