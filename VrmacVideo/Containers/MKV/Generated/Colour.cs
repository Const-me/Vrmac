using System;
using System.Collections.Generic;
using System.IO;

namespace VrmacVideo.Containers.MKV
{
	/// <summary>Settings describing the colour format.</summary>
	public sealed partial class Colour
	{
		/// <summary>The Matrix Coefficients of the video used to derive luma and chroma values from red, green, and blue color primaries. For clarity, the value and meanings for MatrixCoefficients are adopted from Table 4 of ISO/IEC
		/// 23001-8:2016 or ITU-T H.273.</summary>
		public readonly eMatrixCoefficients matrixCoefficients = eMatrixCoefficients.Unspecified;
		/// <summary>Number of decoded bits per channel. A value of 0 indicates that the BitsPerChannel is unspecified.</summary>
		public readonly ulong bitsPerChannel = 0;
		/// <summary>The amount of pixels to remove in the Cr and Cb channels for every pixel not removed horizontally. Example: For video with 4:2:0 chroma subsampling, the ChromaSubsamplingHorz SHOULD be set to 1.</summary>
		public readonly ulong chromaSubsamplingHorz;
		/// <summary>The amount of pixels to remove in the Cr and Cb channels for every pixel not removed vertically. Example: For video with 4:2:0 chroma subsampling, the ChromaSubsamplingVert SHOULD be set to 1.</summary>
		public readonly ulong chromaSubsamplingVert;
		/// <summary>The amount of pixels to remove in the Cb channel for every pixel not removed horizontally. This is additive with ChromaSubsamplingHorz. Example: For video with 4:2:1 chroma subsampling, the ChromaSubsamplingHorz SHOULD
		/// be set to 1 and CbSubsamplingHorz SHOULD be set to 1.</summary>
		public readonly ulong cbSubsamplingHorz;
		/// <summary>The amount of pixels to remove in the Cb channel for every pixel not removed vertically. This is additive with ChromaSubsamplingVert.</summary>
		public readonly ulong cbSubsamplingVert;
		/// <summary>How chroma is subsampled horizontally.</summary>
		public readonly eChromaSitingHorz chromaSitingHorz = eChromaSitingHorz.Unspecified;
		/// <summary>How chroma is subsampled vertically.</summary>
		public readonly eChromaSitingVert chromaSitingVert = eChromaSitingVert.Unspecified;
		/// <summary>Clipping of the color ranges.</summary>
		public readonly eRange range = eRange.Unspecified;
		/// <summary>The transfer characteristics of the video. For clarity, the value and meanings for TransferCharacteristics are adopted from Table 3 of ISO/IEC 23091-4 or ITU-T H.273.</summary>
		public readonly eTransferCharacteristics transferCharacteristics = eTransferCharacteristics.Unspecified;
		/// <summary>The colour primaries of the video. For clarity, the value and meanings for Primaries are adopted from Table 2 of ISO/IEC 23091-4 or ITU-T H.273.</summary>
		public readonly ePrimaries primaries = ePrimaries.Unspecified;
		/// <summary>Maximum brightness of a single pixel (Maximum Content Light Level) in candelas per square meter (cd/m²).</summary>
		public readonly ulong maxCLL;
		/// <summary>Maximum brightness of a single full frame (Maximum Frame-Average Light Level) in candelas per square meter (cd/m²).</summary>
		public readonly ulong maxFALL;
		/// <summary>SMPTE 2086 mastering data.</summary>
		public readonly MasteringMetadata masteringMetadata;

		internal Colour( Stream stream )
		{
			ElementReader reader = new ElementReader( stream );
			while( !reader.EOF )
			{
				eElement id = reader.readElementId();
				switch( id )
				{
					case eElement.MatrixCoefficients:
						matrixCoefficients = (eMatrixCoefficients)reader.readByte( 2 );
						break;
					case eElement.BitsPerChannel:
						bitsPerChannel = reader.readUlong( 0 );
						break;
					case eElement.ChromaSubsamplingHorz:
						chromaSubsamplingHorz = reader.readUlong();
						break;
					case eElement.ChromaSubsamplingVert:
						chromaSubsamplingVert = reader.readUlong();
						break;
					case eElement.CbSubsamplingHorz:
						cbSubsamplingHorz = reader.readUlong();
						break;
					case eElement.CbSubsamplingVert:
						cbSubsamplingVert = reader.readUlong();
						break;
					case eElement.ChromaSitingHorz:
						chromaSitingHorz = (eChromaSitingHorz)reader.readByte( 0 );
						break;
					case eElement.ChromaSitingVert:
						chromaSitingVert = (eChromaSitingVert)reader.readByte( 0 );
						break;
					case eElement.Range:
						range = (eRange)reader.readByte( 0 );
						break;
					case eElement.TransferCharacteristics:
						transferCharacteristics = (eTransferCharacteristics)reader.readByte( 2 );
						break;
					case eElement.Primaries:
						primaries = (ePrimaries)reader.readByte( 2 );
						break;
					case eElement.MaxCLL:
						maxCLL = reader.readUlong();
						break;
					case eElement.MaxFALL:
						maxFALL = reader.readUlong();
						break;
					case eElement.MasteringMetadata:
						masteringMetadata = new MasteringMetadata( stream );
						break;
					default:
						reader.skipElement();
						break;
				}
			}
		}
	}
}