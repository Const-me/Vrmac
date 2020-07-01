using System;
using System.Collections.Generic;
using System.IO;

namespace VrmacVideo.Containers.MKV
{
	/// <summary>Settings describing the encryption algorithm used. If `ContentEncAlgo` != 5 this MUST be ignored.</summary>
	public sealed partial class ContentEncAESSettings
	{
		/// <summary>The AES cipher mode used in the encryption.</summary>
		public readonly eAESSettingsCipherMode aESSettingsCipherMode;

		internal ContentEncAESSettings( Stream stream )
		{
			ElementReader reader = new ElementReader( stream );
			while( !reader.EOF )
			{
				eElement id = reader.readElementId();
				switch( id )
				{
					case eElement.AESSettingsCipherMode:
						aESSettingsCipherMode = (eAESSettingsCipherMode)reader.readByte();
						break;
					default:
						reader.skipElement();
						break;
				}
			}
		}
	}
}