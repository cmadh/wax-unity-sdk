using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace StrattonStudios.EosioUnity.Signing
{

    /// <summary>
    /// Signature model.
    /// </summary>
    public class Signature : IJsonModel
    {

        private const string SIGNER = "signer";
        private const string SIGNATURE = "signature";

        private string signer;
        private string signature;

        public Signature()
        {
        }

        public Signature(string signer, string signature)
        {
            this.signer = signer;
            this.signature = signature;
        }

        public Signature(JObject jsonObject)
        {
            Populate(jsonObject);
        }

        public Signature(IDictionary<string, object> dic)
        {
            Populate(dic);
        }

        public void Populate(JObject jsonObject)
        {
            this.signer = (string)jsonObject[SIGNER];
            this.signature = (string)jsonObject[SIGNATURE];
        }

        public void Populate(IDictionary<string, object> dic)
        {
            this.signer = (string)dic[SIGNER];
            this.signature = (string)dic[SIGNATURE];
        }

        public string GetSigner()
        {
            return this.signer;
        }

        public string GetSignature()
        {
            return this.signature;
        }

        public Dictionary<string, string> ToDictionary()
        {
            Dictionary<string, string> toEncode = new Dictionary<string, string>();
            toEncode.Add(SIGNER, this.signer);
            toEncode.Add(SIGNATURE, this.signature);
            return toEncode;
        }

        public string ToJSON()
        {
            return JsonConvert.SerializeObject(ToDictionary());
        }

    }

}