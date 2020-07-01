using System;
using System.Globalization;
using VrmacVideo.Containers.MP4.Structures;

namespace VrmacVideo.Containers.MP4
{
	public struct MediaInfo
	{
		public readonly DateTime creationTime, modificationTime;
		public readonly TimeSpan duration;
		/// <summary>Time-scale of the media</summary>
		public readonly uint timeScale;
		public readonly CultureInfo culture;
		public readonly MediaHandler mediaHandler;
		public readonly MediaInformation mediaInformation;

		internal MediaInfo( Mp4Reader reader )
		{
			creationTime = modificationTime = default;
			duration = default;
			culture = null;
			mediaHandler = default;
			mediaInformation = null;
			timeScale = 0;

			foreach( eBoxType boxType in reader.readChildren() )
			{
				switch( boxType )
				{
					case eBoxType.mdhd:
						readInfoHeader( reader, out creationTime, out modificationTime, out duration, out culture, out timeScale );
						break;
					case eBoxType.hdlr:
						mediaHandler = new MediaHandler( reader );
						break;
					case eBoxType.minf:
						switch( mediaHandler.mediaHandler )
						{
							case eMediaHandler.vide:
							case eMediaHandler.auxv:
								mediaInformation = new VideoInformation( reader );
								break;
							case eMediaHandler.soun:
								mediaInformation = new AudioInformation( reader );
								break;
						}
						break;
				}
			}
		}

		static void readInfoHeader( Mp4Reader reader, out DateTime creationTime, out DateTime modificationTime, out TimeSpan duration, out CultureInfo culture, out uint timeScale )
		{
			uint ver = reader.readUInt();
			switch( ver & 0xFF )
			{
				case 0:
					var v0 = reader.readStructure<MediaInfoV0>();
					creationTime = v0.creationTime;
					modificationTime = v0.modificationTime;
					duration = v0.duration;
					culture = v0.culture;
					timeScale = v0.timeScale;
					return;
				case 1:
					var v1 = reader.readStructure<MediaInfoV1>();
					creationTime = v1.creationTime;
					modificationTime = v1.modificationTime;
					duration = v1.duration;
					culture = v1.culture;
					timeScale = v1.timeScale;
					return;
			}
			throw new ApplicationException( "Unsupported format version" );
		}
	}
}