using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Vrmac
{
	class EmbeddedResources: iStorageFolder, iStorageFolderManaged
	{
		/// <summary>Source assembly</summary>
		readonly Assembly assembly;
		/// <summary>Base part of the name, including the trailing dot.</summary>
		readonly string basePath;

		static readonly char[] pathSeparators;

		static EmbeddedResources()
		{
			if( Path.DirectorySeparatorChar == Path.AltDirectorySeparatorChar )
				pathSeparators = new char[ 1 ] { Path.DirectorySeparatorChar };
			else
				pathSeparators = new char[ 2 ] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar };
		}

		/// <summary>Split path into components, throws on invalid stuff like "../"</summary>
		static IEnumerable<string> splitIntoComponents( string path )
		{
			int prev = 0;

			while( true )
			{
				int next = path.IndexOfAny( pathSeparators, prev );
				if( 0 == next )
				{
					prev = 1;
					continue;
				}
				if( next == prev )
					throw new ArgumentException( "Malformed path, multiple slashes" );
				if( next < 0 )
				{
					yield return path.Substring( prev );
					yield break;
				}

				string component = path.Substring( prev, next - prev );
				if( component == "." || component == ".." )
					throw new ArgumentException( "Dots aren't supported, you must normalize the input path" );
				yield return component;
				prev = next + 1;
			}
		}

		// https://github.com/microsoft/msbuild/blob/master/src/Tasks/CreateManifestResourceName.cs
		// Have no idea WTF is Everett. Also that code is wrong, it escapes dots, real msbuild leaves them intact.
		// However, if the input is ".01", it will become "._01", something is broken in msbuild, the expected result would be either "_01" or "_.01"

		/// <summary>Is the character a valid first Everett identifier character?</summary>
		static bool IsValidEverettIdFirstChar( char c )
		{
			return char.IsLetter( c ) || CharUnicodeInfo.GetUnicodeCategory( c ) == UnicodeCategory.ConnectorPunctuation;
		}

		/// <summary>Is the character a valid Everett identifier character?</summary>
		static bool IsValidEverettIdChar( char c )
		{
			UnicodeCategory cat = CharUnicodeInfo.GetUnicodeCategory( c );

			return
				char.IsLetterOrDigit( c ) ||
				cat == UnicodeCategory.ConnectorPunctuation ||
				cat == UnicodeCategory.NonSpacingMark ||
				cat == UnicodeCategory.SpacingCombiningMark ||
				cat == UnicodeCategory.EnclosingMark;
		}

		/// <summary>Make that undocumented ID</summary>
		static string makeId( string input )
		{
			StringBuilder result = new StringBuilder( input.Length + 1 );
			bool hadFirst = false;
			foreach( char c in input )
			{
				if( c == '.' )
				{
					// Dots are valid, but they don't count as first character, it's the first non-dot character that has extra "_" inserted before that.
					result.Append( '.' );
					continue;
				}

				if( hadFirst )
				{
					if( IsValidEverettIdChar( c ) )
						result.Append( c );
					else
						result.Append( '_' );
					continue;
				}
				hadFirst = true;

				if( IsValidEverettIdFirstChar( c ) )
				{
					result.Append( c );
					continue;
				}

				result.Append( '_' );
				if( IsValidEverettIdChar( c ) )
					result.Append( c );
				else
					result.Append( '_' );
			}
			return result.ToString();
		}

		string makeFileId( string s )
		{
			// Apparently, the escaping only applies to directory names but not files..
			return s;
		}

		readonly StringBuilder stringBuilder = new StringBuilder();

		string buildBaseName( string bp )
		{
			if( string.IsNullOrEmpty( bp ) )
				return stringBuilder.ToString();
			foreach( string c in splitIntoComponents( bp ) )
			{
				stringBuilder.Append( makeId( c ) );
				stringBuilder.Append( '.' );
			}
			return stringBuilder.ToString();
		}

		public EmbeddedResources( Assembly ass, string basePath )
		{
			assembly = ass;
			this.basePath = buildBaseName( basePath );
		}

		string resourceName( string name )
		{
			stringBuilder.Clear();
			stringBuilder.Append( basePath );

			bool first = true;

			string[] components = splitIntoComponents( name ).ToArray();
			for( int i = 0; i < components.Length - 1; i++ )
			{
				string c = components[ i ];
				if( first )
					first = false;
				else
					stringBuilder.Append( '.' );

				stringBuilder.Append( makeId( c ) );
			}

			// The last component uses different escaping rules, e.g. "-" are OK without escaping
			if( first )
				first = false;
			else
				stringBuilder.Append( '.' );
			stringBuilder.Append( makeFileId( components.Last() ) );

			return stringBuilder.ToString();
		}

		void iStorageFolder.openRead( string name, out Stream stm )
		{
			try
			{
				string res = resourceName( name );
				Stream stream = assembly.GetManifestResourceStream( res );
				if( null == stream )
					throw new FileNotFoundException( $"Embedded resource \"{ res }\" wasn't found in the assembly \"{ assembly.FullName }\"" );

				if( name.endsWith( ".gz" ) )
					stm = new GZipStream( stream, CompressionMode.Decompress );
				else
					stm = stream;
			}
			catch( Exception ex )
			{
				Utils.NativeContext.cacheException( ex );
				throw;
			}
		}

		bool iStorageFolderManaged.fileExist( string name )
		{
			string res = resourceName( name );
			return null != assembly.GetManifestResourceInfo( res );
		}

		public override string ToString()
		{
			return $"the embedded resource folder \"{ basePath.TrimEnd( '.' ) }\"";
		}
	}
}