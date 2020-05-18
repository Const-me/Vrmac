using Diligent.Graphics;
using System;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using Vrmac.Utils.Cursor.Load;

namespace Vrmac.Utils
{
	/// <summary>Load default cursors from embedded resources</summary>
	public static class BuiltinCursors
	{
		static Stream openResource( string name )
		{
			string resource = $"Vrmac.Utils.Cursor.Assets.{ name }.gz";
			var ass = Assembly.GetExecutingAssembly();
			return ass.GetManifestResourceStream( resource );
		}

		static int findBestCursor( CursorFile file, int size )
		{
			return file.images.minIndex( ii => Math.Abs( ii.size.cx - size ) );
		}

		static CursorTexture loadStatic( this IRenderDevice renderDevice, string resource, int idealSize )
		{
			using( var stm = openResource( resource ) )
			using( var unzip = new GZipStream( stm, CompressionMode.Decompress ) )
			using( var file = new CursorFile( unzip ) )
			{
				int index = findBestCursor( file, idealSize );
				return file.load( renderDevice, index );
			}
		}

		static CursorTexture loadAnimated( this IRenderDevice device, string resource, int idealSize )
		{
			using( var stm = openResource( resource ) )
			{
				AniFile file;
				using( var unzip = new GZipStream( stm, CompressionMode.Decompress, true ) )
					file = new AniFile( unzip );

				int index = file.formats.minIndex( ii => Math.Abs( ii.size.cx - idealSize ) );

				stm.rewind();
				using( var unzip = new GZipStream( stm, CompressionMode.Decompress ) )
					return file.load( device, unzip, "Busy cursor", index );
			}
		}

		/// <summary>Load a cursor from embedded resource, decompress, decode, and upload to VRAM.</summary>
		public static CursorTexture loadCursor( this IRenderDevice renderDevice, eCursor cursor, int idealSize = 32 )
		{
			switch( cursor )
			{
				case eCursor.None:
				default:
					return null;
				case eCursor.Arrow:
					return renderDevice.loadStatic( "arrow", idealSize );
				case eCursor.Beam:
					return renderDevice.loadStatic( "beam", idealSize );
				case eCursor.Hand:
					return renderDevice.loadStatic( "hand", idealSize );
				case eCursor.Working:
					return renderDevice.loadAnimated( "working", idealSize );
				case eCursor.Busy:
					return renderDevice.loadAnimated( "busy", idealSize );
			}
		}
	}
}