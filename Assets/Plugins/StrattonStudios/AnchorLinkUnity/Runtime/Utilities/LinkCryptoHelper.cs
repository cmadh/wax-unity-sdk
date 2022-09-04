using System;
using System.IO;
using System.Linq;

using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Crypto.Utilities;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;

namespace StrattonStudios.AnchorLinkUnity.Utilities
{

    public static class LinkCryptoHelper
    {

        public static byte[] DigestSHA512(byte[] message)
        {
            var digest = new Sha512Digest();
            digest.BlockUpdate(message, 0, message.Length);
            byte[] result = new byte[digest.GetDigestSize()];
            digest.DoFinal(result, 0);
            return result;
        }

        public static byte[] DigestSHA256(byte[] message)
        {
            var digest = new Sha256Digest();
            digest.BlockUpdate(message, 0, message.Length);
            byte[] result = new byte[digest.GetDigestSize()];
            digest.DoFinal(result, 0);
            return result;
        }

    }

}