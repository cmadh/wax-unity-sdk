using System.Collections;
using System.Collections.Generic;

using StrattonStudios.EosioUnity.Signing;

using UnityEngine;

namespace StrattonStudios.EosioUnity.Samples
{

    /// <summary>
    /// An example of decoding Signing Request from an encoded URL.
    /// </summary>
    public class DecodeESRSample : MonoBehaviour
    {

        [Header("EOS Config")]
        public string endpoint = "https://wax.greymass.com";
        //public string chainId = "1064487b3cd1a897ce03ae5b6a865651747e2e152090f99c1d19d44e01aea5a4";
        public int expireSeconds = 60;

        public string encodedUrl = "esr://gmPgYmBY1mTC_MoglIGBIVzX5uxZxoLNB5-lzdl4CijAsOKtkZEyXECFefKznLxjL5gZwIAj3DECRDNlKAJJAA";

        protected EosioClient eos;
        protected IAbiProvider abiProvider;
        protected IZlibProvider zlibProvider;

        private void Start()
        {
            var config = new EosioClientConfig();
            //config.ChainId = this.chainId;
            config.ExpireSeconds = this.expireSeconds;
            config.HttpEndpoint = this.endpoint;
            this.eos = new EosioClient(config);

            this.abiProvider = new DefaultAbiProvider(this.eos);
            this.zlibProvider = DefaultZlibProvider.Instance;
        }

        public void Decode()
        {
            var options = new SigningRequestEncodingOptions();
            options.zlib = this.zlibProvider;
            options.abiProvider = this.abiProvider;
            var request = SigningRequest.From(this.encodedUrl, options);

            Debug.Log(request.ToDataJSON());
        }

        public void SetEncodedUrl(string url)
        {
            this.encodedUrl = url;
        }

    }

}