using System;
using System.Collections.Generic;
using System.IO;

namespace VrmacVideo.Containers.MKV
{
	/// <summary>Contains all possible strings to use for the chapter display.</summary>
	public sealed partial class ChapterDisplay
	{
		/// <summary>Contains the string to use as the chapter atom.</summary>
		public readonly string chapString;
		/// <summary>The languages corresponding to the string, in the <a href="https://www.loc.gov/standards/iso639-2/php/English_list.php">bibliographic ISO-639-2 form</a>. This Element MUST be ignored if the ChapLanguageIETF Element is
		/// used within the same ChapterDisplay Element.</summary>
		public readonly string[] chapLanguage;
		/// <summary>Specifies the language used in the ChapString according to <a href="https://tools.ietf.org/html/bcp47">BCP 47</a> and using the <a
		/// href="https://www.iana.com/assignments/language-subtag-registry/language-subtag-registry">IANA Language Subtag Registry</a>. If this Element is used, then any ChapLanguage Elements used in the same ChapterDisplay MUST
		/// be ignored.</summary>
		public readonly string[] chapLanguageIETF;
		/// <summary>The countries corresponding to the string, same 2 octets as in <a href="https://www.iana.org/domains/root/db">Internet domains</a>. This Element MUST be ignored if the ChapLanguageIETF Element is used within the same
		/// ChapterDisplay Element.</summary>
		public readonly string[] chapCountry;

		internal ChapterDisplay( Stream stream )
		{
			List<string> chapLanguagelist = null;
			List<string> chapLanguageIETFlist = null;
			List<string> chapCountrylist = null;
			ElementReader reader = new ElementReader( stream );
			while( !reader.EOF )
			{
				eElement id = reader.readElementId();
				switch( id )
				{
					case eElement.ChapString:
						chapString = reader.readUtf8();
						break;
					case eElement.ChapLanguage:
						if( null == chapLanguagelist ) chapLanguagelist = new List<string>();
						chapLanguagelist.Add( reader.readAscii() );
						break;
					case eElement.ChapLanguageIETF:
						if( null == chapLanguageIETFlist ) chapLanguageIETFlist = new List<string>();
						chapLanguageIETFlist.Add( reader.readAscii() );
						break;
					case eElement.ChapCountry:
						if( null == chapCountrylist ) chapCountrylist = new List<string>();
						chapCountrylist.Add( reader.readAscii() );
						break;
					default:
						reader.skipElement();
						break;
				}
			}
			if( chapLanguagelist != null ) chapLanguage = chapLanguagelist.ToArray();
			if( chapLanguageIETFlist != null ) chapLanguageIETF = chapLanguageIETFlist.ToArray();
			if( chapCountrylist != null ) chapCountry = chapCountrylist.ToArray();
		}
	}
}