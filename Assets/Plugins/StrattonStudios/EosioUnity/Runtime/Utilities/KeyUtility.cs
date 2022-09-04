using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Cryptography.ECDSA;

namespace StrattonStudios.EosioUnity.Utilities
{

    /// <summary>
    /// Public, Private and Signature keys utilities.
    /// </summary>
    public static class KeyUtility
    {

        /// <summary>
        /// KeyPair with a private and public key
        /// </summary>
        public class KeyPair
        {
            public string PrivateKey { get; set; }
            public string PublicKey { get; set; }
        }

        #region Constants

        /// <summary>
        /// The size of a public key in binary format.
        /// </summary>
        public const int PublicKeySizeInBytes = 33;

        /// <summary>
        /// The size of a private key in binary format.
        /// </summary>
        public const int PrivateKeySizeInBytes = 32;

        /// <summary>
        /// The size of the signature key in binary format.
        /// </summary>
        public const int SignatureKeySizeInBytes = 64;

        //
        // R1
        //

        public const string PublicR1Prefix = "PUB_R1_";
        public const string PrivateR1Prefix = "PVT_R1_";
        public const string SignatureR1Prefix = "SIG_R1_";

        //
        // K1
        //

        public const string PublicK1Prefix = "PUB_K1_";
        public const string PrivateK1Prefix = "PVT_K1_";
        public const string SignatureK1Prefix = "SIG_K1_";

        #endregion

        #region Public Methods

        /// <summary>
        /// Generate a new key pair based on the key type
        /// </summary>
        /// <param name="keyType">The key type</param>
        /// <returns>Returns the key pair</returns>
        public static KeyPair GenerateKeyPair(KeyType? keyType = KeyType.Sha256x2)
        {
            if (!keyType.HasValue)
            {
                throw new ArgumentNullException(nameof(keyType));
            }
            var key = Secp256K1Manager.GenerateRandomKey();
            var pubKey = Secp256K1Manager.GetPublicKey(key, true);

            string privPrefix = null;
            string pubPrefix;
            switch (keyType)
            {
                default:
                case KeyType.Sha256x2:
                    pubPrefix = "EOS";
                    break;
                case KeyType.K1:
                    privPrefix = "PVT_K1_";
                    pubPrefix = "PUB_K1_";
                    break;
                case KeyType.R1:
                    privPrefix = "PVT_R1_";
                    pubPrefix = "PUB_R1_";
                    break;
            }
            return new KeyPair()
            {
                PrivateKey = ConvertBinaryKeyToString(key, keyType, privPrefix),
                PublicKey = ConvertBinaryKeyToString(pubKey, keyType != KeyType.Sha256x2 ? keyType : null, pubPrefix)
            };
        }

        /// <summary>
        /// Converts the Public key from binary format to string format.
        /// </summary>
        /// <param name="keyBytes">The public key bytes</param>
        /// <param name="keyType">The optional key type. (sha256x2, R1, K1)</param>
        /// <param name="prefix">The optional prefix to public key</param>
        /// <returns>Returns the public key in string format</returns>
        public static string ConvertPublicKeyBinaryToString(byte[] keyBytes, KeyType? keyType = null, string prefix = "EOS")
        {
            if (keyType.HasValue)
            {
                switch (keyType)
                {
                    case KeyType.K1:
                        prefix = PublicK1Prefix;
                        break;
                    case KeyType.R1:
                        prefix = PublicR1Prefix;
                        break;
                }
            }
            return ConvertBinaryKeyToString(keyBytes, keyType, prefix);
        }

        /// <summary>
        /// Converts the Private key from binary format to string format.
        /// </summary>
        /// <param name="keyBytes">The private key bytes</param>
        /// <param name="keyType">The optional key type. (sha256x2, R1, K1)</param>
        /// <param name="prefix">The optional prefix to public key</param>
        /// <returns>Returns the private key in string format</returns>
        public static string ConvertPrivateKeyBinaryToString(byte[] keyBytes, KeyType keyType = KeyType.R1, string prefix = PrivateR1Prefix)
        {
            switch (keyType)
            {
                case KeyType.K1:
                    prefix = PrivateK1Prefix;
                    break;
                case KeyType.R1:
                    prefix = PrivateR1Prefix;
                    break;
            }
            return ConvertBinaryKeyToString(keyBytes, keyType, prefix);
        }

        /// <summary>
        /// Converts the Signature from binary format to string format.
        /// </summary>
        /// <param name="keyBytes">The signature bytes</param>
        /// <param name="keyType">The optional key type. (sha256x2, R1, K1)</param>
        /// <param name="prefix">The optional prefix to public key</param>
        /// <returns>Returns the signature in string format</returns>
        public static string ConvertSignatureBinaryToString(byte[] signBytes, KeyType keyType = KeyType.K1, string prefix = SignatureK1Prefix)
        {
            switch (keyType)
            {
                case KeyType.K1:
                    prefix = SignatureK1Prefix;
                    break;
                case KeyType.R1:
                    prefix = SignatureR1Prefix;
                    break;
            }
            return ConvertBinaryKeyToString(signBytes, keyType, prefix);
        }

        /// <summary>
        /// Converts the key from binary format to string format.
        /// </summary>
        /// <param name="key">The key byte array</param>
        /// <param name="keyType">The key type. (sha256x2, R1, K1)</param>
        /// <param name="prefix">The optional prefix to apply on the string format</param>
        /// <returns>Returns the key in string format</returns>
        public static string ConvertBinaryKeyToString(byte[] key, KeyType? keyType, string prefix = null)
        {
            byte[] digest;

            if (keyType.HasValue)
            {
                switch (keyType)
                {
                    default:
                    case KeyType.Sha256x2:
                        digest = Sha256Manager.GetHash(Sha256Manager.GetHash(ArrayUtility.Combine(new List<byte[]>() {
                            new byte[] { 128 },
                            key
                        })));

                        return (prefix ?? "") + Base58.Encode(ArrayUtility.Combine(new List<byte[]>() {
                            new byte[] { 128 },
                            key,
                            digest.Take(4).ToArray()
                        }));
                    case KeyType.K1:
                    case KeyType.R1:
                        digest = Ripemd160Manager.GetHash(ArrayUtility.Combine(new List<byte[]>() {
                            key,
                            Encoding.UTF8.GetBytes(keyType.ToString())
                        }));
                        break;
                }
            }
            else
            {
                digest = Ripemd160Manager.GetHash(key);
            }

            return (prefix ?? "") + Base58.Encode(ArrayUtility.Combine(new List<byte[]>() {
                key,
                digest.Take(4).ToArray()
            }));
        }

        /// <summary>
        /// Converts the Public key to binary format.
        /// </summary>
        /// <param name="publicKey">The public key</param>
        /// <param name="prefix">Optional prefix on key</param>
        /// <param name="removeChecksum">Whether or not to remove the checksum when converting to binary format</param>
        /// <returns>Returns the public key in binary format</returns>
        public static byte[] ConvertPublicKeyStringToBinary(string publicKey, string prefix = "EOS", bool removeChecksum = false)
        {
            if (publicKey.StartsWith(PublicR1Prefix))
            {
                return ConvertStringKeyToBinary(publicKey.Substring(7), PublicKeySizeInBytes, KeyType.R1, removeChecksum);
            }
            else if (publicKey.StartsWith(PublicK1Prefix))
            {
                return ConvertStringKeyToBinary(publicKey.Substring(7), PublicKeySizeInBytes, KeyType.K1, removeChecksum);
            }
            else if (publicKey.StartsWith(prefix))
            {
                return ConvertStringKeyToBinary(publicKey.Substring(prefix.Length), PublicKeySizeInBytes, removeChecksum: removeChecksum);
            }
            else
            {
                throw new Exception("The public key's format cannot be recognized.");
            }
        }


        /// <summary>
        /// Converts the Private key to binary format.
        /// </summary>
        /// <param name="privateKey">The private key</param>
        /// <param name="removeChecksum">Whether or not to remove the checksum when converting to binary format</param>
        /// <returns>Returns the private key in binary format</returns>
        public static byte[] ConvertPrivateKeyStringToBinary(string privateKey, bool removeChecksum = false)
        {
            if (privateKey.StartsWith(PrivateR1Prefix))
            {
                return ConvertStringKeyToBinary(privateKey.Substring(7), PrivateKeySizeInBytes, KeyType.R1, removeChecksum);
            }
            else if (privateKey.StartsWith(PrivateK1Prefix))
            {
                return ConvertStringKeyToBinary(privateKey.Substring(7), PrivateKeySizeInBytes, KeyType.K1, removeChecksum);
            }
            else
            {
                return ConvertStringKeyToBinary(privateKey, PrivateKeySizeInBytes, KeyType.Sha256x2, removeChecksum);
            }
        }

        /// <summary>
        /// Converts the Signature key to binary format.
        /// </summary>
        /// <param name="signature">The encoded signature</param>
        /// <returns>Returns the signature in binary format</returns>
        public static byte[] ConvertSignatureStringToBinary(string signature)
        {
            if (signature.StartsWith(SignatureK1Prefix))
            {
                return ConvertStringKeyToBinary(signature.Substring(7), SignatureKeySizeInBytes, KeyType.K1);
            }
            if (signature.StartsWith(SignatureR1Prefix))
            {
                return ConvertStringKeyToBinary(signature.Substring(7), SignatureKeySizeInBytes, KeyType.R1);
            }
            else
            {
                throw new Exception("The signature format cannot be recognized.");
            }
        }

        /// <summary>
        /// Converts Public/Private/Signature key to binary format.
        /// </summary>
        /// <param name="key">The key</param>
        /// <param name="size">The key size</param>
        /// <param name="keyType">Optional key type. (sha256x2, R1, K1)</param>
        /// <param name="removeChecksum">Specifies whether or not to remove the checksum from the key when converting</param>
        /// <returns>Returns the key in binary format</returns>
        public static byte[] ConvertStringKeyToBinary(string key, int size, KeyType? keyType = null, bool removeChecksum = false)
        {
            var keyBytes = Base58.Decode(key);
            byte[] digest;
            int skipSize = 0;

            if (keyType.HasValue)
            {
                switch (keyType)
                {
                    default:
                    case KeyType.Sha256x2:
                        // Skip the version
                        skipSize = 1;
                        digest = Sha256Manager.GetHash(Sha256Manager.GetHash(keyBytes.Take(size + skipSize).ToArray()));
                        break;
                    case KeyType.R1:
                    case KeyType.K1:
                        // TODO: Confirm working with both K1 and R1
                        digest = Ripemd160Manager.GetHash(ArrayUtility.Combine(new List<byte[]>() {
                            keyBytes.Take(size + skipSize).ToArray(),
                            Encoding.UTF8.GetBytes(keyType.ToString())
                        }));
                        break;
                }
            }
            else
            {
                digest = Ripemd160Manager.GetHash(keyBytes.Take(size).ToArray());
            }
            if (!keyBytes.Skip(size + skipSize).SequenceEqual(digest.Take(4)))
            {
                throw new Exception("The key's checksum doesn't match.");
            }
            if (removeChecksum)
            {
                return Base58.RemoveCheckSum(keyBytes);
            }
            return keyBytes;
        }

        #endregion

    }

}