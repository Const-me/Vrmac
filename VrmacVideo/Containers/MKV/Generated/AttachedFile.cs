using System;
using System.Collections.Generic;
using System.IO;

namespace VrmacVideo.Containers.MKV
{
	/// <summary>An attached file.</summary>
	public sealed partial class AttachedFile
	{
		/// <summary>A human-friendly name for the attached file.</summary>
		public readonly string fileDescription;
		/// <summary>Filename of the attached file.</summary>
		public readonly string fileName;
		/// <summary>MIME type of the file.</summary>
		public readonly string fileMimeType;
		/// <summary>The data of the file.</summary>
		public readonly Blob fileData;
		/// <summary>Unique ID representing the file, as random as possible.</summary>
		public readonly ulong fileUID;
		/// <summary>A binary value that a track/codec can refer to when the attachment is needed.</summary>
		public readonly Blob fileReferral;
		/// <summary><a href="http://developer.divx.com/docs/divx_plus_hd/format_features/World_Fonts">DivX font extension</a></summary>
		public readonly ulong fileUsedStartTime;
		/// <summary><a href="http://developer.divx.com/docs/divx_plus_hd/format_features/World_Fonts">DivX font extension</a></summary>
		public readonly ulong fileUsedEndTime;

		internal AttachedFile( Stream stream )
		{
			ElementReader reader = new ElementReader( stream );
			while( !reader.EOF )
			{
				eElement id = reader.readElementId();
				switch( id )
				{
					case eElement.FileDescription:
						fileDescription = reader.readUtf8();
						break;
					case eElement.FileName:
						fileName = reader.readUtf8();
						break;
					case eElement.FileMimeType:
						fileMimeType = reader.readAscii();
						break;
					case eElement.FileData:
						fileData = Blob.read( reader );
						break;
					case eElement.FileUID:
						fileUID = reader.readUlong();
						break;
					case eElement.FileReferral:
						fileReferral = Blob.read( reader );
						break;
					case eElement.FileUsedStartTime:
						fileUsedStartTime = reader.readUlong();
						break;
					case eElement.FileUsedEndTime:
						fileUsedEndTime = reader.readUlong();
						break;
					default:
						reader.skipElement();
						break;
				}
			}
		}
	}
}