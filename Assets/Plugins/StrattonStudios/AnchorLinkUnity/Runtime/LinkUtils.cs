using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json.Linq;

using Org.BouncyCastle.Asn1.Nist;
using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;

using StrattonStudios.EosioUnity.Serialization;
using StrattonStudios.EosioUnity.Signing;
using StrattonStudios.EosioUnity.Utilities;

namespace StrattonStudios.AnchorLinkUnity.Utilities
{

    public class LinkUtils
    {

        public static SealedMessage SealMessage(string message, string privateKey, string publicKey, ulong? nonce = null)
        {
            if (nonce == null || !nonce.HasValue)
            {
                nonce = RandomExtensions.RandomULong();
            }

            var pair = GenerateKeyPair();

            byte[] publicKeyBlob = KeyUtility.ConvertPublicKeyStringToBinary(publicKey, removeChecksum: true);
            byte[] privateKeyBlob = KeyUtility.ConvertPrivateKeyStringToBinary(privateKey, true);

            var curve = SecNamedCurves.GetByName("secp256k1");
            var domain = new ECDomainParameters(curve.Curve, curve.G, curve.N, curve.H);

            var publicPoint = curve.Curve.DecodePoint(publicKeyBlob);

            var publicKeyParams = new ECPublicKeyParameters(publicPoint, domain);
            var privateKeyParams = new ECPrivateKeyParameters(new BigInteger(1, privateKeyBlob), domain);

            var keyAgree = AgreementUtilities.GetBasicAgreement("ECDH");
            keyAgree.Init(privateKeyParams);
            var sharedSecret = LinkCryptoHelper.DigestSHA512(keyAgree.CalculateAgreement(publicKeyParams).ToByteArray());

            var serializer = new AbiTypeWriter(null);
            var key = serializer.SerializeType("uint64", nonce.Value, LinkAbiData.Types).Concat(sharedSecret).ToArray();

            var cipherText = CryptoUtility.AesEncrypt(key, message);
            var checksum = System.BitConverter.ToUInt32(LinkCryptoHelper.DigestSHA256(key), 0);

            var data = new SealedMessage()
            {
                from = publicKey,
                nonce = nonce.Value,
                cipherText = cipherText,
                checksum = checksum
            };
            return data;
        }

        private static AsymmetricCipherKeyPair GenerateKeyPair()
        {
            var curve = ECNamedCurveTable.GetByName("secp256k1");
            var domainParams = new ECDomainParameters(curve.Curve, curve.G, curve.N, curve.H, curve.GetSeed());

            var secureRandom = new SecureRandom();
            var keyParams = new ECKeyGenerationParameters(domainParams, secureRandom);

            var generator = new ECKeyPairGenerator("ECDSA");
            generator.Init(keyParams);
            var keyPair = generator.GenerateKeyPair();

            return keyPair;
        }

        public static Dictionary<string, object> SessionMetadata(JObject payload, SigningRequest request, Dictionary<string, object> metadata = null)
        {
            if (metadata == null)
            {
                metadata = new Dictionary<string, object>();
            }
            // TODO: Add `sameDevice` meta

            if (payload.ContainsKey("link_meta"))
            {
                try
                {
                    var linkMeta = (JObject)payload["link_meta"];
                    foreach (var item in linkMeta)
                    {
                        metadata[SnakeCaseToCamelCase(item.Key)] = linkMeta[item.Key];
                    }
                }
                catch (System.Exception e)
                {
                    UnityEngine.Debug.LogWarning("Unable to parse link metadata");
                    UnityEngine.Debug.LogException(e);
                    UnityEngine.Debug.LogWarning(payload["link_meta"].ToString());
                }
            }
            return metadata;
        }

        public static string SnakeCaseToCamelCase(string name)
        {
            return string.Join(string.Empty, name.Split('_').Select((v) =>
            {
                return (v.Length > 0 ? char.ToUpperInvariant(v[0]) : '_') + v.Substring(1);
            }));
        }

    }

}