using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Reflection;

namespace Vrmac.Input.Linux
{
	/// <summary>Uses an embedded dataset to resolve USB vendor IDs into something human-readable</summary>
#if DEBUG
	public static class UsbVendors
#else
	static class UsbVendors
#endif
	{
		// Only caching searched entries as opposed to the complete dictionary to save memory. User only needs 2-3 values from that DB, for the devices they plugged in.
		static readonly object syncRoot = new object();
		static readonly Dictionary<ushort, string> cachedVendors = new Dictionary<ushort, string>();

		static string findVendorName( ushort vendor )
		{
			var ass = Assembly.GetExecutingAssembly();
			using( var stm = ass.GetManifestResourceStream( "Vrmac.Input.Linux.usb-vendors.gz" ) )
			using( var unzip = new GZipStream( stm, CompressionMode.Decompress ) )
			using( var reader = new BinaryReader( unzip ) )
			{
				int size = reader.ReadUInt16();
				for( int i = 0; i < size; i++ )
				{
					ushort key = reader.ReadUInt16();
					if( key == vendor )
						return reader.ReadString();
					if( key > vendor )
						return null;
					reader.ReadString();
				}
				return null;
			}
		}

		/// <summary>Query the embedded database for USB vendor name</summary>
		public static string print( ushort code )
		{
			lock( syncRoot )
			{
				string name;
				if( !cachedVendors.TryGetValue( code, out name ) )
				{
					name = findVendorName( code );
					cachedVendors.Add( code, name );
				}
				return name ?? "Unknown vendor";
			}
		}
	}
}