using System;
using System.IO;

namespace VrmacVideo
{
#if false
	sealed class DbgSaveFrames
	{
		const string dest = "/tmp/frames";

		public DbgSaveFrames()
		{
			if( !Directory.Exists( dest ) )
			{
				Directory.CreateDirectory( dest );
				return;
			}
			DirectoryInfo di = new DirectoryInfo( dest );
			foreach( FileInfo file in di.GetFiles() )
				file.Delete();
		}

		int nextFrame = 1;

		public void parameters( ReadOnlySpan<byte> src )
		{
			string path = Path.Combine( dest, "parameters.bin" );
			using( var f = File.Create( path ) )
				f.Write( src );
		}

		public void frame( ReadOnlySpan<byte> src )
		{
			string name = string.Format( "frame-{0:D4}.bin", nextFrame++ );
			string path = Path.Combine( dest, name );
			using( var f = File.Create( path ) )
				f.Write( src );
		}
	}
#endif
}