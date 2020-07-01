using System;
using System.Buffers.Binary;
using System.IO;

namespace VrmacVideo.Containers
{
	/// <summary>Class factory for media files</summary>
	static class OpenMedia
	{
		enum eContainerFormat: byte
		{
			Mpeg4,
			MKV,
		}

		static eContainerFormat detectFormat( Stream file )
		{
			if( !file.CanSeek )
				throw new ArgumentException( "The source stream doesn’t support random access" );

			// Read first 12 bytes of the file
			Span<byte> span = stackalloc byte[ 12 ];
			file.read( span );
			file.Seek( 0, SeekOrigin.Begin );

			// Test for MKV
			if( BinaryPrimitives.ReadUInt32BigEndian( span ) == (uint)MKV.eElement.EBML )
				return eContainerFormat.MKV;

			// Test for Mpeg4
			MP4.eFileType ft = (MP4.eFileType)BitConverter.ToUInt32( span.Slice( 8 ) );
			switch( ft )
			{
				case MP4.eFileType.isom:
				case MP4.eFileType.iso2:
					return eContainerFormat.Mpeg4;
			}

			// None of the above
			throw new NotSupportedException( "The media file is neither Mpeg4 nor MKV." );
		}

		/// <summary>Class factory for media files</summary>
		public static iMediaFile openMedia( Stream file )
		{
			switch( detectFormat( file ) )
			{
				case eContainerFormat.Mpeg4:
					return new MP4.Mp4File( file );
				case eContainerFormat.MKV:
					return new MKV.MkvMediaFile( file );
			}
			throw new ApplicationException();
		}
	}
}