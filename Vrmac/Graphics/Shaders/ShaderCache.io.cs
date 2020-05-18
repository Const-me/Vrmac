using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;

namespace Diligent.Graphics
{
	public partial class ShaderCache
	{
		// BTW, this kind of functionality is exceptionally hard to implement in C++, yet almost trivially simple in C#.
		static readonly DataContractSerializer serializer = new DataContractSerializer( typeof( Payload ) );

		/// <summary>Performance optimization, pre-shared XML dictionary. MS binary XML is awesome.</summary>
		class XmlDict: IXmlDictionary
		{
			readonly XmlDictionaryString[] array;
			readonly Dictionary<string, XmlDictionaryString> dict;

			public XmlDict()
			{
				string[] entries = new string[]
				{
					"ShaderCache.Payload",
					"http://schemas.datacontract.org/2004/07/Diligent.Graphics",
					"http://www.w3.org/2001/XMLSchema-instance",
					"name",
					"binary",
					"binaryFormat",
					"shaderType",
					"combinedTextureSampler",
					"combinedSamplerSuffix",
				};

				array = new XmlDictionaryString[ entries.Length ];
				dict = new Dictionary<string, XmlDictionaryString>( entries.Length );
				for( int i = 0; i < entries.Length; i++ )
				{
					XmlDictionaryString s = new XmlDictionaryString( this, entries[ i ], i );
					array[ i ] = s;
					dict.Add( entries[ i ], s );
				}
			}

			bool IXmlDictionary.TryLookup( int key, out XmlDictionaryString result )
			{
				if( key >= 0 && key < array.Length )
				{
					result = array[ key ];
					return true;
				}
				result = null;
				return false;
			}

			bool IXmlDictionary.TryLookup( string value, out XmlDictionaryString result )
			{
				result = dict.GetValueOrDefault( value );
				return null != result;
			}

			bool IXmlDictionary.TryLookup( XmlDictionaryString value, out XmlDictionaryString result )
			{
				if( value.Dictionary == this )
				{
					result = value;
					return true;
				}
				result = dict.GetValueOrDefault( value.Value );
				return null != result;
			}
		}

		static readonly IXmlDictionary xmlDictionary = new XmlDict();

		static Payload read( Stream stm )
		{
			using( XmlDictionaryReader reader = XmlDictionaryReader.CreateBinaryReader( stm, xmlDictionary, XmlDictionaryReaderQuotas.Max ) )
				return (Payload)serializer.ReadObject( reader );
		}

		static void write( Stream stm, Payload payload )
		{
			using( XmlDictionaryWriter writer = XmlDictionaryWriter.CreateBinaryWriter( stm, xmlDictionary ) )
				serializer.WriteObject( writer, payload );
		}
	}
}