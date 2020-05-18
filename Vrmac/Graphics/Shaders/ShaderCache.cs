using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Vrmac;

namespace Diligent.Graphics
{
	/// <summary>Implementation of the shaders cache</summary>
	public partial class ShaderCache: iShaderCache
	{
		[DataContract]
		class Payload
		{
			[DataMember]
			public string name;

			[DataMember]
			public byte[] binary;

			[DataMember]
			public int binaryFormat = 0;

			[DataMember]
			public uint shaderType;

			[DataMember]
			public bool combinedTextureSampler;

			[DataMember]
			public string combinedSamplerSuffix;
		}

		iShaderFactory factory;

		/// <summary>Full path to the cache file on disk, or null if disk caching wasn’t configured.</summary>
		public readonly string diskCachePath;
		ZipArchive zipArchive = null;
		readonly Dictionary<Guid, ZipArchiveEntry> zipEntries = new Dictionary<Guid, ZipArchiveEntry>();
		readonly Dictionary<Guid, Payload> ramEntries = new Dictionary<Guid, Payload>();

		/// <summary>Create the cache. Pass null to only cache them in RAM, or a path to ZIP file to cache them on disk.</summary>
		/// <remarks>If you pass a path, don't forget to call <see cref="flushToDisk"/> to have them written, it doesn't happen automatically.</remarks>
		public ShaderCache( string zip = null )
		{
			if( null == zip )
			{
				diskCachePath = null;
				return;
			}
			if( !Path.IsPathRooted( zip ) )
				zip = Path.Combine( Directory.GetCurrentDirectory(), zip );
			diskCachePath = zip;
			if( !File.Exists( zip ) )
				return;

			listDiskEntries();
		}

		/// <summary>If the argument is true, construct a reasonable default location for the on-disk shaders cache. If false, the cache will be RAM only.</summary>
		/// <remarks>If you pass true, don't forget to call <see cref="flushToDisk"/> to have them written, it doesn't happen automatically.</remarks>
		public ShaderCache( bool defaultPath )
		{
			if( !defaultPath )
			{
				diskCachePath = null;
				return;
			}
			diskCachePath = defaultCachePath();
			if( !File.Exists( diskCachePath ) )
				return;
			listDiskEntries();
		}

		/// <summary>%TEMP% on Windows, ~/.cache/ on Linux</summary>
		static string getTempPath()
		{
			switch( RuntimeEnvironment.operatingSystem )
			{
				case eOperatingSystem.Windows:
					return Path.GetTempPath();

				case eOperatingSystem.Linux:
					// https://www.linux.org/threads/temporary-files.10817/
					return Path.Combine( Environment.GetFolderPath( Environment.SpecialFolder.UserProfile ), ".cache" );
			}

			throw new NotImplementedException( "Unsupported platform, only Windows and Linux are supported so far" );
		}

		static string defaultCachePath()
		{
			string dir = getTempPath();
			// TODO: maybe find something more stable, this one changes every rebuilt.
			Guid guid = Assembly.GetEntryAssembly().ManifestModule.ModuleVersionId;
			string id = guid.print();
			string name = $"shaders-{ id }.zip";
			string res = Path.Combine( dir, "VrmacGraphics", name );
			// Console.WriteLine( "ShaderCache.defaultCachePath: {0}", res );
			return res;
		}

		Payload lookupInCache( Guid guid )
		{
			if( ramEntries.TryGetValue( guid, out var res ) )
				return res;

			var zipEntry = zipEntries.GetValueOrDefault( guid );
			if( null != zipEntry )
				return read( zipEntry.Open() );

			return null;
		}

		IShader loadShader( ref Guid guid, Payload p )
		{
			return factory.loadCachedShader( ref guid, p.binary, (uint)p.binary.Length, p.binaryFormat,
				p.name, (ShaderType)p.shaderType, p.combinedTextureSampler, p.combinedSamplerSuffix );
		}

		int iShaderCache.tryLoadShader( ref Guid guid, out IShader shader )
		{
			shader = null;
			try
			{
				Payload p = lookupInCache( guid );
				if( null == p )
					return 1;   // S_FALSE
				shader = loadShader( ref guid, p );
				return 0;   // S_OK
			}
			catch( Exception ex )
			{
				return ex.HResult;
			}
		}

		int iShaderCache.saveShader( ref Guid guid, byte[] binary, uint length, int binaryFormat, string name, ShaderType shaderType, bool combinedTextureSampler, string combinedSamplerSuffix )
		{
			if( ramEntries.ContainsKey( guid ) || zipEntries.ContainsKey( guid ) )
				return 1;

			Payload p = new Payload();
			p.binary = binary;
			p.binaryFormat = binaryFormat;
			p.name = name;
			p.shaderType = (uint)shaderType;
			p.combinedTextureSampler = combinedTextureSampler;
			p.combinedSamplerSuffix = combinedSamplerSuffix;
			ramEntries[ guid ] = p;
			return 0;
		}

		/// <summary>Drop all RAM entries of the cache.</summary>
		public void dropRamEntries()
		{
			ramEntries.Clear();
		}

		void listDiskEntries()
		{
			try
			{
				zipArchive = new ZipArchive( File.OpenRead( diskCachePath ), ZipArchiveMode.Read, false );

				foreach( var e in zipArchive.Entries )
				{
					string name = Path.GetFileNameWithoutExtension( e.Name );
					if( !Guid.TryParseExact( name, "D", out Guid guid ) )
						continue;
					zipEntries.Add( guid, e );
				}
			}
			catch( Exception )
			{
				// TODO: log somewhere
				zipArchive?.Dispose();
				zipArchive = null;
				try
				{
					File.Delete( diskCachePath );
				}
				catch( Exception ) { }
			}
		}

		/// <summary>Flush RAM entries to disk.</summary>
		public int flushToDisk( CompressionLevel compression = CompressionLevel.Optimal )
		{
			if( ramEntries.Count <= 0 )
				return 0;

			string tmpPath = diskCachePath + ".tmp";
			ZipArchive newArchive;
			bool appendingEntries = File.Exists( diskCachePath );

			if( appendingEntries )
			{
				File.Copy( diskCachePath, tmpPath );
				newArchive = new ZipArchive( File.Open( tmpPath, FileMode.Open, FileAccess.ReadWrite, FileShare.None ), ZipArchiveMode.Update, false );
			}
			else
			{
				string dir = Path.GetDirectoryName( tmpPath );
				if( !Directory.Exists( dir ) )
					Directory.CreateDirectory( dir );
				newArchive = new ZipArchive( File.Create( tmpPath ), ZipArchiveMode.Create, false );
			}

			int wrote = 0;
			using( newArchive )
			{
				Guid[] keys = ramEntries.Keys.ToArray();
				foreach( Guid guid in keys )
				{
					string name = guid.ToString( "D" ) + ".bin";
					ZipArchiveEntry e;
					if( appendingEntries )
					{
						e = newArchive.GetEntry( name );
						if( null != e )
							continue;
					}

					e = newArchive.CreateEntry( name, compression );
					write( e.Open(), ramEntries[ guid ] );
					wrote++;
				}
			}

			zipArchive?.Dispose();
			zipArchive = null;

			// TODO: implement better solution here.
			// All OSes support atomic move with overwrite, yet it's only exposed starting from .NET Core 3.0, as FileSystem.Rename from Microsoft.VisualBasic.Core.dll
			// For now, introducing race condition, if the process will crash or debugger stops between File.Delete and File.Move lines.
			// It's not a huge deal however, the probability is very low, and the worst case is shaders will be dropped, the app will just recompile them next time.
			if( File.Exists( diskCachePath ) )
				File.Delete( diskCachePath );
			File.Move( tmpPath, diskCachePath );

			zipEntries.Clear();
			listDiskEntries();

			return wrote;
		}

		/// <summary>If shader with the provided GUID found on the cache, upload it to GPU and return.</summary>
		/// <returns>Return null if it wasn't found</returns>
		public IShader loadCachedShader( Guid guid )
		{
			Payload payload = lookupInCache( guid );
			if( null == payload )
				return null;
			return loadShader( ref guid, payload );
		}

		void iShaderCache.setFactory( iShaderFactory factory )
		{
			this.factory = factory;
		}
	}
}