using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Text;

namespace Vrmac.Input.Linux
{
	/// <summary>Uses an embedded dataset to resolve device IDs into strings</summary>
#if DEBUG
	public static class UsbDevices
#else
	static class UsbDevices
#endif
	{
		// This class caches stuff, need a lock to protect from multi-threaded access
		static readonly object syncRoot = new object();

		// Offset from the start of the resource to the first GZipped blob
		static int? payloadOffset = null;

		// Key = USB vendor ID, value = offset of the compressed blob with the names for that vendor. Null value means "we have already searched for that vendor and it's not on the list."
		static readonly Dictionary<ushort, int?> cachedVendors = new Dictionary<ushort, int?>();

		// Key = combined vendor + device ID, value is the name of the device. Null value means "we have already searched, and the ID was not found."
		static readonly Dictionary<uint, string> cachedDevices = new Dictionary<uint, string>();

		const string resource = "Vrmac.Input.Linux.usb-devices.bin";

		static int? findVendorBlobOffset( Stream resource, ushort vendor )
		{
			int? result;
			if( cachedVendors.TryGetValue( vendor, out result ) )
				return result;
			result = null;

			using( var reader = new BinaryReader( resource, Encoding.UTF8, true ) )
			{
				int size = reader.ReadUInt16();

				for( int i = 0; i < size; i++ )
				{
					ushort readVendor = reader.ReadUInt16();
					int begin = reader.ReadInt32();
					if( readVendor == vendor )
					{
						result = begin;
						if( payloadOffset.HasValue )
							break;
					}
					// Keys in the blob are sorted. If what we have read is greater than what we look for, it means the data won't be found anywhere.
					// However, if we don't yet know the payloadOffset, we have to read the complete index to find out.
					if( readVendor > vendor && payloadOffset.HasValue )
						break;
				}
				if( !payloadOffset.HasValue )
					payloadOffset = (int)resource.Position;
			}

			cachedVendors.Add( vendor, result );
			return result;
		}

		static string findDeviceName( Stream resource, int blobOffset, ushort deviceId )
		{
			resource.Seek( payloadOffset.Value + blobOffset, SeekOrigin.Begin );
			using( var unzip = new GZipStream( resource, CompressionMode.Decompress ) )
			using( var reader = new BinaryReader( unzip ) )
			{
				int count = reader.ReadUInt16();
				for( int i = 0; i < count; i++ )
				{
					ushort dev = reader.ReadUInt16();
					if( dev == deviceId )
						return reader.ReadString();
					if( dev > deviceId )
					{
						// Keys in the blob are sorted. If what we have read is greater than what we look for, it means the data won't be found.
						return null;
					}
					reader.ReadString();
				}
				return null;
			}
		}

		/// <summary>Given USB vendor and device IDs, query embedded database for the name.</summary>
		public static string lookup( ushort vendor, ushort device )
		{
			lock( syncRoot )
			{
				uint key = ( (uint)vendor ) << 16 | device;
				if( cachedDevices.TryGetValue( key, out string val ) )
					return val;

				var ass = Assembly.GetExecutingAssembly();
				using( var stm = ass.GetManifestResourceStream( resource ) )
				{
					int? offset = findVendorBlobOffset( stm, vendor );
					if( !offset.HasValue )
						return null;

					string name = findDeviceName( stm, offset.Value, device );
					cachedDevices.Add( key, name );
					return name;
				}
			}
		}
	}
}