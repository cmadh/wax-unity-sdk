using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

using Cysharp.Threading.Tasks;

using Org.BouncyCastle.Utilities;

using UnityEngine;

namespace StrattonStudios.AnchorLinkUnity
{

    /// <summary>
    /// A highly configurable secure storage wrapper that uses symmetric (AES by default) encryption.
    /// </summary>
    public class LinkSecureStorage : ILinkStorage
    {

        protected string password = RandomString(12);
        protected int saltSize = 12;
        protected int iterations = 10000;

        protected string algorithmName = "aes";

        protected PaddingMode paddingMode = PaddingMode.PKCS7;
        protected CipherMode cipherMode = CipherMode.CBC;

        protected byte[] iv;
        protected byte[] key;

        protected SymmetricAlgorithm algorithm;
        protected ILinkStorage storage;

        /// <summary>
        /// The symmetric algorithm.
        /// </summary>
        public virtual SymmetricAlgorithm Algorithm
        {
            get
            {
                if (this.algorithm == null)
                {
                    if (string.IsNullOrEmpty(this.algorithmName))
                    {
                        this.algorithmName = "aes";
                    }

                    this.algorithm = SymmetricAlgorithm.Create(this.algorithmName);
                    this.algorithm.Mode = this.cipherMode;
                    this.algorithm.Padding = this.paddingMode;
                }
                return this.algorithm;
            }
        }

        public LinkSecureStorage(ILinkStorage storage, string password, int saltSize = 12, int iterations = 10000, string algorithmName = "aes", PaddingMode paddingMode = PaddingMode.PKCS7, CipherMode cipherMode = CipherMode.CBC)
        {
            this.storage = storage;
            this.password = password;
            this.saltSize = saltSize;
            this.iterations = iterations;
            this.algorithmName = algorithmName;
            this.paddingMode = paddingMode;
            this.cipherMode = cipherMode;
        }

        public UniTask Write(string key, string data)
        {
            try
            {
                var bytes = Encoding.UTF8.GetBytes(data);
                string encryptedData;
                using (var stream = new MemoryStream())
                {
                    using (var cryptoStream = GetCryptoStream(stream, CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(bytes);
                    }
                    encryptedData = System.Convert.ToBase64String(stream.ToArray());
                }
                this.storage.Write(key, encryptedData);
                return UniTask.CompletedTask;
            }
            catch (System.Exception e)
            {
                Debug.LogError("Failed to encrypt the data, saving the data without encryption instead");
                Debug.LogException(e);

                // Backwards compatibility
                return this.storage.Write(key, data);
            }
        }

        public async UniTask<string> Read(string key)
        {
            try
            {
                var savedData = await this.storage.Read(key);
                var encryptedBytes = System.Convert.FromBase64String(savedData);
                var dataBytes = new byte[encryptedBytes.Length];
                using (var stream = new MemoryStream(encryptedBytes))
                {
                    using (var cryptoStream = GetCryptoStream(stream, CryptoStreamMode.Read))
                    {
                        cryptoStream.Read(dataBytes, 0, dataBytes.Length);
                    }
                }
                return Encoding.UTF8.GetString(dataBytes);
            }
            catch (System.Exception e)
            {
                //Debug.LogError("Failed to decrypt the data, loading the data without encryption instead");
                //Debug.LogException(e);

                // Backwards compatibility
                return await this.storage.Read(key);
            }
        }

        public UniTask Remove(string key)
        {
            return this.storage.Remove(key);
        }

        /// <summary>
        /// Prepares the encryption Key and IV.
        /// </summary>
        /// <returns>Returns the salt calculated during the creation of Key and IV</returns>
        public virtual byte[] PrepareEncryptPassword()
        {
            using (var rfc = new Rfc2898DeriveBytes(this.password, this.saltSize, this.iterations))
            {
                this.key = rfc.GetBytes(Algorithm.KeySize / 8);
                this.iv = rfc.GetBytes(Algorithm.BlockSize / 8);
                return rfc.Salt;
            }
        }

        /// <summary>
        /// Prepares the decryption Key and IV.
        /// </summary>
        /// <param name="salt">The salt</param>
        public virtual void PrepareDecryptPassword(byte[] salt)
        {
            using (var rfc = new Rfc2898DeriveBytes(this.password, salt, this.iterations))
            {
                this.key = rfc.GetBytes(Algorithm.KeySize / 8);
                this.iv = rfc.GetBytes(Algorithm.BlockSize / 8);
            }
        }

        /// <summary>
        /// Gets a <see cref="CryptoStream"/> for the <paramref name="stream"/> using the specified <paramref name="mode"/> with symmetric encryption algorithm.
        /// </summary>
        /// <param name="stream">The stream</param>
        /// <param name="mode">The read or write mode</param>
        /// <returns>Returns a <see cref="CryptoStream"/> for the given <paramref name="stream"/></returns>
        public virtual CryptoStream GetCryptoStream(Stream stream, CryptoStreamMode mode)
        {
            this.algorithm = null;
            CryptoStream cryptoStream;
            if (mode == CryptoStreamMode.Read)
            {
                var salt = new byte[this.saltSize];
                stream.Read(salt, 0, salt.Length);
                PrepareDecryptPassword(salt);
                this.algorithm.Key = this.key;
                this.algorithm.IV = this.iv;
                cryptoStream = new CryptoStream(stream, Algorithm.CreateDecryptor(this.key, this.iv), CryptoStreamMode.Read);
            }
            else
            {
                var salt = PrepareEncryptPassword();
                stream.Write(salt, 0, salt.Length);
                this.algorithm.Key = this.key;
                this.algorithm.IV = this.iv;
                cryptoStream = new CryptoStream(stream, Algorithm.CreateEncryptor(this.key, this.iv), CryptoStreamMode.Write);
            }
            return cryptoStream;
        }

        /// <summary>
        /// Generates a random string with the given length
        /// </summary>
        /// <param name="length">The length of the string</param>
        /// <returns>Returns a randomly generated string</returns>
        public static string RandomString(int length)
        {
            var random = new System.Random();
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            StringBuilder res = new StringBuilder();
            while (0 < length--)
            {
                res.Append(valid[random.Next(valid.Length)]);
            }
            return res.ToString();
        }

    }

}