using ComLight;
using System;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Vrmac.Input.X11
{
	/// <summary>Describes a KeySym that has a Unicode character.</summary>
	/// <remarks>Not all of them have; the embedded resource only contains KeySym values which have a character.</remarks>
	[StructLayout( LayoutKind.Sequential )]
	public struct sKeySymMap
	{
		/// <summary>KeySym value from X11</summary>
		public uint keySym;
		/// <summary>Unicode character of that symbol.</summary>
		public ushort unicodeChar;
		ushort unused;
		internal void read( BinaryReader reader )
		{
			keySym = reader.ReadUInt32();
			unicodeChar = reader.ReadUInt16();
		}
	}

	/// <summary>X11 developers failed to support Unicode. Doing manually.</summary>
	/// <remarks>Funfact: the data in the embedded .gzip comes from parsing X11/keysymdef.h header file. However, the data ain't exposed through any API.</remarks>
	[ComInterface( "147e3e10-7ecc-40f1-ba36-03afa562d01c", eMarshalDirection.ToManaged )]
	public interface iLinuxEngine: IDisposable
	{
		/// <summary>Upload the mapping from KeySym to Unicode characters.</summary>
		void uploadKeySymMap( [In, MarshalAs( UnmanagedType.LPArray )] sKeySymMap[] data, int count );
	}

	static class KeySym
	{
		const string resourceName = @"Vrmac.Input.X11.KeySym.gz";

		static bool uploaded = false;
		static readonly object syncRoot = new object();

		public static void uploadKeySymMap( this iGraphicsEngine engine )
		{
			lock( syncRoot )
			{
				if( uploaded )
					return;
				uploaded = true;

				// Deserialize into an array
				Assembly ass = Assembly.GetExecutingAssembly();
				using( var x11 = ComLightCast.cast<iLinuxEngine>( engine ) )
				using( var stm = ass.GetManifestResourceStream( resourceName ) )
				using( var unzip = new GZipStream( stm, CompressionMode.Decompress ) )
				using( var reader = new BinaryReader( unzip ) )
				{
					int count = reader.ReadInt32();
					sKeySymMap[] map = new sKeySymMap[ count ];
					for( int i = 0; i < count; i++ )
						map[ i ].read( reader );
					// Upload the data to native memory.
					// X11 window class uses the data to resolve key symbols
					// https://www.oreilly.com/library/view/xlib-reference-manual/9780937175262/16_appendix-h.html
					// into characters, to be passed to iKeyboardHandler.keyEvent method.
					x11.uploadKeySymMap( map, count );
				}
			}
		}
	}
}