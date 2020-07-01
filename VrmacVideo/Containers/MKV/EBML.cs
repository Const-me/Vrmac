using System.IO;

namespace VrmacVideo.Containers.MKV
{
	public sealed class EBML
	{
		public readonly uint version = 1, readVersion = 1;
		public readonly string docType;
		public readonly uint docTypeVersion = 1;

		public EBML( Stream stream )
		{
			ElementReader reader = new ElementReader( stream );
			while( !reader.EOF )
			{
				eElement id = reader.stream.readElementId();
				switch( id )
				{
					case eElement.EBMLVersion:
						version = reader.readUint( 1 );
						break;
					case eElement.EBMLReadVersion:
						readVersion = reader.readUint( 1 );
						break;
					case eElement.DocType:
						docType = reader.readAscii();
						break;
					default:
						reader.skipElement();
						break;
				}
			}
		}
	}
}