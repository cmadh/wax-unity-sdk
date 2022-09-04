using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace StrattonStudios.EosioUnity.Signing
{

    /// <summary>
    /// Sealed message model.
    /// </summary>
    public class SealedMessage : IJsonModel
    {

        public const string FROM = "from";
        public const string NONCE = "nonce";
        public const string CIPHERTEXT = "ciphertext";
        public const string CHECKSUM = "checksum";

        public string from;
        public ulong nonce;
        public byte[] cipherText;
        public uint checksum;

        public SealedMessage()
        {

        }

        public SealedMessage(JObject jsonObject)
        {
            this.from = (string)jsonObject[FROM];
            this.nonce = (ulong)jsonObject[NONCE];
            this.cipherText = jsonObject[CIPHERTEXT].ToObject<byte[]>();
            this.checksum = (uint)jsonObject[CHECKSUM];
        }

        public Dictionary<string, object> ToDictionary()
        {
            var map = new Dictionary<string, object>();
            map.Add(FROM, this.from);
            map.Add(NONCE, this.nonce);
            map.Add(CIPHERTEXT, this.cipherText);
            map.Add(CHECKSUM, this.checksum);
            return map;
        }

        public string ToJSON()
        {
            return JsonConvert.SerializeObject(ToDictionary());
        }

    }

}