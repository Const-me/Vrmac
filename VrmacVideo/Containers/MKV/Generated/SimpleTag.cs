using System;
using System.Collections.Generic;
using System.IO;

namespace VrmacVideo.Containers.MKV
{
	/// <summary>Contains general information about the target.</summary>
	public sealed partial class SimpleTag
	{
		/// <summary>The name of the Tag that is going to be stored.</summary>
		public readonly string tagName;
		/// <summary>Specifies the language of the tag specified, in the <a href="https://www.matroska.org/technical/basics.html#language-codes">Matroska languages form</a>. This Element MUST be ignored if the TagLanguageIETF Element is
		/// used within the same SimpleTag Element.</summary>
		public readonly string tagLanguage = "und";
		/// <summary>Specifies the language used in the TagString according to <a href="https://tools.ietf.org/html/bcp47">BCP 47</a> and using the <a
		/// href="https://www.iana.com/assignments/language-subtag-registry/language-subtag-registry">IANA Language Subtag Registry</a>. If this Element is used, then any TagLanguage Elements used in the same SimpleTag MUST be
		/// ignored.</summary>
		public readonly string tagLanguageIETF;
		/// <summary>A boolean value to indicate if this is the default/original language to use for the given tag.</summary>
		public readonly byte tagDefault = 1;
		/// <summary>The value of the Tag.</summary>
		public readonly string tagString;
		/// <summary>The values of the Tag if it is binary. Note that this cannot be used in the same SimpleTag as TagString.</summary>
		public readonly Blob tagBinary;

		internal SimpleTag( Stream stream )
		{
			ElementReader reader = new ElementReader( stream );
			while( !reader.EOF )
			{
				eElement id = reader.readElementId();
				switch( id )
				{
					case eElement.TagName:
						tagName = reader.readUtf8();
						break;
					case eElement.TagLanguage:
						tagLanguage = reader.readAscii();
						break;
					case eElement.TagLanguageIETF:
						tagLanguageIETF = reader.readAscii();
						break;
					case eElement.TagDefault:
						tagDefault = (byte)reader.readUint( 1 );
						break;
					case eElement.TagString:
						tagString = reader.readUtf8();
						break;
					case eElement.TagBinary:
						tagBinary = Blob.read( reader );
						break;
					default:
						reader.skipElement();
						break;
				}
			}
		}
	}
}