using System;
using System.IO;
using System.IO.Compression;
using System.Reflection;

namespace Vrmac
{
	interface iStorageFolderManaged
	{
		bool fileExist( string name );
	}

	/// <summary>Utility class which implements a helpers to expose shaders and assets to native code.</summary>
	public static class StorageFolder
	{
		class ReadFolder: iStorageFolder, iStorageFolderManaged
		{
			readonly string root;
			public ReadFolder( string root )
			{
				this.root = root;
			}
			void iStorageFolder.openRead( string name, out Stream stm )
			{
				stm = File.OpenRead( Path.Combine( root, name ) );
			}
			bool iStorageFolderManaged.fileExist( string name )
			{
				return File.Exists( Path.Combine( root, name ) );
			}
			public override string ToString()
			{
				return $"the folder \"{ root }\"";
			}
		}

		/// <summary>Implement <see cref="iStorageFolder" /> on top of a directory somewhere in file system.</summary>
		public static iStorageFolder directory( string path )
		{
			if( !Path.IsPathRooted( path ) )
				path = Path.Combine( Directory.GetCurrentDirectory(), path );
			if( !Directory.Exists( path ) )
				throw new FileNotFoundException( "Assets directory doesn't exist", path );
			return new ReadFolder( path );
		}

		class ZipFolder: iStorageFolder, ComLight.iComDisposable, iStorageFolderManaged
		{
			readonly ZipArchive archive;

			public ZipFolder( ZipArchive archive )
			{
				this.archive = archive;
			}

			void ComLight.iComDisposable.lastNativeReferenceReleased()
			{
				archive?.Dispose();
			}

			void iStorageFolder.openRead( string name, out Stream stm )
			{
				ZipArchiveEntry e = archive.GetEntry( name );
				if( null != e )
					stm = e.Open();
				else
					throw new FileNotFoundException( $"ZIp entry {name} was not found in the archive" );
			}
			bool iStorageFolderManaged.fileExist( string name )
			{
				return null != archive.GetEntry( name );
			}
			public override string ToString()
			{
				return "a ZIP archive";
			}
		}

		/// <summary>Implement <see cref="iStorageFolder" /> on top of a ZIP archive.</summary>
		public static iStorageFolder zip( Stream zipFile, bool leaveOpen = false )
		{
			if( !zipFile.CanRead || !zipFile.CanSeek )
			{
				throw new ArgumentException( "The source stream of StorageFolder.zip API must be readable, and support random access." );
			}
			ZipArchive archive = new ZipArchive( zipFile, ZipArchiveMode.Read, leaveOpen );
			return new ZipFolder( archive );
		}

		/// <summary>Create a folder that can read embedded resources of the specified assembly.</summary>
		/// <param name="ass">Assembly to load resources from.</param>
		/// <param name="relativeLocation">Location within the assembly, including default namespace of the assembly.</param>
		public static iStorageFolder embeddedResources( Assembly ass, string relativeLocation )
		{
			return new EmbeddedResources( ass, relativeLocation );
		}
	}
}