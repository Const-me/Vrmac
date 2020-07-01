using System;
using System.Collections.Generic;
using System.IO;

namespace VrmacVideo.Containers.MKV
{
	/// <summary>Settings describing the encryption used. This Element MUST be present if the value of `ContentEncodingType` is 1 (encryption) and MUST be ignored otherwise.</summary>
	public sealed partial class ContentEncryption
	{
		/// <summary>The encryption algorithm used. The value '0' means that the contents have not been encrypted but only signed.</summary>
		public readonly eContentEncAlgo contentEncAlgo = eContentEncAlgo.NotEncrypted;
		/// <summary>For public key algorithms this is the ID of the public key the the data was encrypted with.</summary>
		public readonly Blob contentEncKeyID;
		/// <summary>Settings describing the encryption algorithm used. If `ContentEncAlgo` != 5 this MUST be ignored.</summary>
		public readonly ContentEncAESSettings contentEncAESSettings;
		/// <summary>A cryptographic signature of the contents.</summary>
		public readonly Blob contentSignature;
		/// <summary>This is the ID of the private key the data was signed with.</summary>
		public readonly Blob contentSigKeyID;
		/// <summary>The algorithm used for the signature.</summary>
		public readonly eContentSigAlgo contentSigAlgo = eContentSigAlgo.NotSigned;
		/// <summary>The hash algorithm used for the signature.</summary>
		public readonly eContentSigHashAlgo contentSigHashAlgo = eContentSigHashAlgo.NotSigned;

		internal ContentEncryption( Stream stream )
		{
			ElementReader reader = new ElementReader( stream );
			while( !reader.EOF )
			{
				eElement id = reader.readElementId();
				switch( id )
				{
					case eElement.ContentEncAlgo:
						contentEncAlgo = (eContentEncAlgo)reader.readByte( 0 );
						break;
					case eElement.ContentEncKeyID:
						contentEncKeyID = Blob.read( reader );
						break;
					case eElement.ContentEncAESSettings:
						contentEncAESSettings = new ContentEncAESSettings( stream );
						break;
					case eElement.ContentSignature:
						contentSignature = Blob.read( reader );
						break;
					case eElement.ContentSigKeyID:
						contentSigKeyID = Blob.read( reader );
						break;
					case eElement.ContentSigAlgo:
						contentSigAlgo = (eContentSigAlgo)reader.readByte( 0 );
						break;
					case eElement.ContentSigHashAlgo:
						contentSigHashAlgo = (eContentSigHashAlgo)reader.readByte( 0 );
						break;
					default:
						reader.skipElement();
						break;
				}
			}
		}
	}
}