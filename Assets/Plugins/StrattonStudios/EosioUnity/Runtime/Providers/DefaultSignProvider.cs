using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Cryptography.ECDSA;

using Cysharp.Threading.Tasks;

using StrattonStudios.EosioUnity.Models;
using StrattonStudios.EosioUnity.Utilities;

namespace StrattonStudios.EosioUnity
{

    /// <summary>
    /// Signature provider default implementation that stores private keys in memory
    /// </summary>
    public class DefaultSignProvider : IEosioSignProvider
    {

        private readonly byte[] KeyTypeBytes = Encoding.UTF8.GetBytes("K1");
        private readonly Dictionary<string, byte[]> Keys = new Dictionary<string, byte[]>();

        #region Constructors

        /// <summary>
        /// Create provider with single private key
        /// </summary>
        /// <param name="privateKey">The private key</param>
        public DefaultSignProvider(string privateKey)
        {

            // TODO: Check if working properly
            var privKeyBytes = KeyUtility.ConvertPrivateKeyStringToBinary(privateKey, true);
            var pubKey = KeyUtility.ConvertPublicKeyBinaryToString(Secp256K1Manager.GetPublicKey(privKeyBytes, true));
            this.Keys.Add(pubKey, privKeyBytes);
        }

        /// <summary>
        /// Create provider with list of private keys
        /// </summary>
        /// <param name="privateKeys">The private keys</param>
        public DefaultSignProvider(List<string> privateKeys)
        {
            if (privateKeys == null || privateKeys.Count == 0)
                throw new ArgumentNullException("privateKeys");

            foreach (var key in privateKeys)
            {
                var privKeyBytes = KeyUtility.ConvertPrivateKeyStringToBinary(key, true);
                var pubKey = KeyUtility.ConvertPublicKeyBinaryToString(Secp256K1Manager.GetPublicKey(privKeyBytes, true));
                this.Keys.Add(pubKey, privKeyBytes);
            }
        }

        /// <summary>
        /// Create provider with dictionary of encoded key pairs
        /// </summary>
        /// <param name="encodedKeys">The encoded keys mapping as public key / private key</param>
        public DefaultSignProvider(Dictionary<string, string> encodedKeys)
        {
            if (encodedKeys == null || encodedKeys.Count == 0)
                throw new ArgumentNullException("encodedKeys");

            foreach (var keyPair in encodedKeys)
            {
                var privKeyBytes = KeyUtility.ConvertPrivateKeyStringToBinary(keyPair.Value, true);
                this.Keys.Add(keyPair.Key, privKeyBytes);
            }
        }

        /// <summary>
        /// Create provider with dictionary of key pair with private key as byte array
        /// </summary>
        /// <param name="keys">The keys as public key (string) / private key (bytes)</param>
        public DefaultSignProvider(Dictionary<string, byte[]> keys)
        {
            if (keys == null || keys.Count == 0)
                throw new ArgumentNullException("keys");

            this.Keys = keys;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Get available public keys from signature provider
        /// </summary>
        /// <returns>Returns the list of public keys</returns>
        public UniTask<IEnumerable<string>> GetAvailableKeys()
        {
            return UniTask.FromResult(this.Keys.Keys.AsEnumerable());
        }

        /// <summary>
        /// Sign bytes using the signature provider
        /// </summary>
        /// <param name="chainId">The EOSIO Chain ID</param>
        /// <param name="requiredKeys">The required public keys for signing this bytes</param>
        /// <param name="signBytes">The binary to sign</param>
        /// <param name="abiNames">The ABI contract names to get ABI information from</param>
        /// <returns>Returns the list of signatures per required keys</returns>
        public UniTask<IEnumerable<string>> Sign(string chainId, IEnumerable<string> requiredKeys, byte[] signBytes, IEnumerable<string> abiNames = null)
        {
            if (requiredKeys == null)
                return UniTask.FromResult(new List<string>().AsEnumerable());

            var availableAndReqKeys = requiredKeys.Intersect(this.Keys.Keys);

            var data = new List<byte[]>()
            {
                Hex.HexToBytes(chainId),
                signBytes,
                new byte[32]
            };

            var hash = Sha256Manager.GetHash(ArrayUtility.Combine(data));

            return UniTask.FromResult(availableAndReqKeys.Select(key =>
            {
                var sign = Secp256K1Manager.SignCompressedCompact(hash, this.Keys[key]);
                var check = new List<byte[]>() { sign, this.KeyTypeBytes };
                var checksum = Ripemd160Manager.GetHash(ArrayUtility.Combine(check)).Take(4).ToArray();
                var signAndChecksum = new List<byte[]>() { sign, checksum };

                return KeyUtility.SignatureK1Prefix + Base58.Encode(ArrayUtility.Combine(signAndChecksum));
            }));

        }

        #endregion

    }

}