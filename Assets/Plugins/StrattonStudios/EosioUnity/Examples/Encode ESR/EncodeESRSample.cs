using System.Collections;
using System.Collections.Generic;

using StrattonStudios.EosioUnity.Signing;

using UnityEngine;
using UnityEngine.UI;

using ZXing;
using ZXing.QrCode;

namespace StrattonStudios.EosioUnity.Samples
{

    /// <summary>
    /// An example of creating and encoding a SigningRequest as QR code.
    /// </summary>
    public class EncodeESRSample : MonoBehaviour
    {

        [Header("References")]
        [SerializeField]
        protected RawImage qrImage;

        [Header("EOS Config")]
        [SerializeField]
        protected string endpoint = "https://wax.greymass.com";
        //[SerializeField]
        //protected string chainId = "1064487b3cd1a897ce03ae5b6a865651747e2e152090f99c1d19d44e01aea5a4";
        [SerializeField]
        protected int expireSeconds = 60;

        [Header("Action Details")]
        public string from = "teststrasatr";
        public string to = "stratton.gm";
        public string quantity = "0.00001000 WAX";
        public string memo = "h!";

        protected EosioClient eos;

        protected Texture2D qrTexture;

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

            this.qrTexture = new Texture2D(256, 256);
            this.qrImage.texture = this.qrTexture;
        }

        /// <summary>
        /// Creates and encoding the signing request and displays a QR code for it.
        /// </summary>
        public async void Encode()
        {
            var args = new SigningRequestCreateArgs();
            args.action = new Action()
            {
                Account = new AccountName("eosio.token"),
                Authorization = new List<PermissionLevel>()
                {
                    new PermissionLevel() {
                        Actor = new AccountName(this.from),
                        Permission= new PermissionName("active")
                    }
                },
                Name = new ActionName("transfer"),
                Data = new ActionData(new Dictionary<string, object>()
                {
                    { "from", this.from },
                    { "to", this.to },
                    { "quantity", this.quantity },
                    { "memo", this.memo }
                })
            };
            //args.chainId = Chain.FromChainId(this.chainId);

            var options = new SigningRequestEncodingOptions();
            options.zlib = this.zlibProvider;
            options.abiProvider = this.abiProvider;

            var request = await SigningRequest.Create(args, options);

            // The encoded signing request URL
            var url = request.Encode();
            Debug.Log(url);

            var color32 = EncodeQR(url, this.qrTexture.width, this.qrTexture.height);
            this.qrTexture.SetPixels32(color32);
            this.qrTexture.Apply();
        }

        private static Color32[] EncodeQR(string textForEncoding, int width, int height)
        {
            var writer = new BarcodeWriter
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new QrCodeEncodingOptions
                {
                    Height = height,
                    Width = width
                }
            };
            return writer.Write(textForEncoding);
        }

    }

}