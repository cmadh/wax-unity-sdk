using System;
using System.Collections;
using System.Collections.Generic;

using StrattonStudios.AnchorLinkUnity.Utilities;

using Cryptography.ECDSA;

using Cysharp.Threading.Tasks;

using Newtonsoft.Json;

using StrattonStudios.EosioUnity;
using StrattonStudios.EosioUnity.Serialization;
using StrattonStudios.EosioUnity.Signing;
using StrattonStudios.EosioUnity.Utilities;

using UnityEngine;
using UnityEngine.Networking;

namespace StrattonStudios.AnchorLinkUnity
{

    public class LinkChannelSession : LinkSession, ILinkTransport
    {

        #region Fields

        protected readonly System.Timers.Timer timeoutTimer = new System.Timers.Timer();

        protected Func<SigningRequest, SealedMessage> encrypt;

        protected int timeout = 2 * 60 * 1000; // in ms
        protected string channelKey;
        protected string channelUrl;
        protected string channelName;

        protected LinkChannelSessionData sessionData;

        public ILinkStorage Storage => this.link.Transport.Storage;

        public string UserAgent => this.link.Transport.UserAgent;

        #endregion

        #region Constructors

        public LinkChannelSession(Link link, LinkChannelSessionData data, Dictionary<string, object> metadata)
        {
            this.type = "channel";
            this.link = link;

            // TODO: To be implemented for protocol version 3
            //this.chainId = data.chainId;
            this.auth = data.auth;
            this.publicKey = data.publicKey;
            this.identifier = data.identifier;
            var privateKey = data.requestKey;
            this.channelKey = data.channel.key;
            this.channelUrl = data.channel.url;
            this.channelName = data.channel.name;

            this.encrypt = (request) =>
            {
                return LinkUtils.SealMessage(request.Encode(true, false), privateKey, this.channelKey);
            };

            if (metadata == null)
            {
                metadata = new Dictionary<string, object>();
            }
            this.metadata = metadata;
            this.metadata["timeout"] = this.timeout;
            this.metadata["name"] = this.channelName;
            //this.metadata["request_key"] = privateKey;
            this.metadata["request_key"] = Secp256K1Manager.GetPublicKey(Cryptography.ECDSA.Base58.RemoveCheckSum(KeyUtility.ConvertPrivateKeyStringToBinary(privateKey)), true);

            // TODO: Might be unused
            //data.channel.name = this.channelName;
            //data.channel.key = this.channelKey;
            //data.channel.url = this.channelUrl;
            this.sessionData = data;
        }

        #endregion

        #region Public Methods

        public override SerializedLinkSession Serialize()
        {
            return new SerializedLinkSession()
            {
                type = "channel",
                data = JsonConvert.SerializeObject(this.sessionData),
                metadata = this.metadata
            };
        }

        public void OnFailure(SigningRequest request, Exception error)
        {
            this.link.Transport.OnFailure(request, error);
        }

        public void OnSuccess(SigningRequest request, TransactResult result)
        {
            this.link.Transport.OnSuccess(request, result);
        }

        public async void OnRequest(SigningRequest request, LinkCancelHandler cancel)
        {
            var info = new LinkInfo()
            {
                expiration = System.DateTime.Now.AddMilliseconds(this.timeout)
            };

            this.link.Transport.OnSessionRequest(this, request, cancel);

            this.timeoutTimer.Interval = this.timeout; // This is in ms
            this.timeoutTimer.Elapsed += (source, e) =>
            {
                cancel(new SessionException("Wallet did not respond in time", LinkErrorCode.E_TIMEOUT, this));
                this.timeoutTimer.Stop();
            };
            this.timeoutTimer.AutoReset = false;
            this.timeoutTimer.Start();

            var serializer = new AbiTypeWriter(null);
            request.SetInfoKey("link", serializer.SerializeStructData("link_info", info, LinkAbiData.Types));

            bool payloadSent = false;
            var payload = serializer.SerializeStructData("sealed_message", this.encrypt(request).ToDictionary(), LinkAbiData.Types);
            try
            {
                payloadSent = this.link.Transport.SendSessionPayload(payload, this);
            }
            catch (System.Exception e)
            {
                Debug.LogWarning("Unexpected error when transport tried to send session payload");
                Debug.LogException(e);
            }
            if (!payloadSent)
            {
                return;
            }

            var postData = new List<IMultipartFormSection>()
            {
                new MultipartFormDataSection(payload)
            };
            var postRequest = UnityWebRequest.Post(this.channelUrl, postData);
            postRequest.SetRequestHeader("X-Buoy-Soft-Wait", "10");
            await postRequest.SendWebRequest();

            // Stop the timer
            this.timeoutTimer.Stop();

#if UNITY_2020_1_OR_NEWER
            if (postRequest.result == UnityWebRequest.Result.Success)
#else
            if (!postRequest.isHttpError && !postRequest.isNetworkError)
#endif
            {
                if (Mathf.Floor(postRequest.responseCode / 100) != 2)
                {
                    if (postRequest.responseCode == 202)
                    {
                        Debug.LogWarning("Missing delivery ABI from session channel");
                    }
                    cancel(new SessionException("Unable to push message", LinkErrorCode.E_DELIVERY, this));
                }
                else
                {
                    // Request delivered
                }
            }
            else
            {
                cancel(new SessionException($"Unable to reach link service: ({postRequest.error})", LinkErrorCode.E_DELIVERY, this));
            }
        }

        public void OnSessionRequest(LinkSession session, SigningRequest request, LinkCancelHandler cancel)
        {
            this.link.Transport.OnSessionRequest(session, request, cancel);
        }

        public void AddLinkInfo(SigningRequest request)
        {
            var createInfo = new LinkCreate(this.identifier, (string)this.metadata["request_key"]);
            var serializer = new AbiTypeWriter(null);
            request.SetInfoKey("link", serializer.SerializeStructData("link_create", createInfo, LinkAbiData.Types));
        }

        public UniTask<SigningRequest> Prepare(SigningRequest request, LinkSession session)
        {
            return this.link.Transport.Prepare(request, this);
        }

        public void ShowLoading()
        {
            this.link.Transport.ShowLoading();
        }

        public bool RecoverError(Exception error, SigningRequest request)
        {
            return this.link.Transport.RecoverError(error, request);
        }

        public bool SendSessionPayload(byte[] payload, LinkSession session)
        {
            // TODO: Check if should throw the exception instead
            return false;
            throw new System.NotImplementedException();
        }

        public override object MakeSignatureProvider()
        {
            return this.link.MakeSignatureProvider(new string[] { this.publicKey }, this);
        }

        public override async UniTask<TransactResult> Transact(TransactArgs args, TransactOptions options)
        {
            // TODO: To be implemented for the protocol version 3
            //options.chain = Chain.FromChainId(this.chainId.GetChainId());
            var response = await this.link.Transact(args, options, this);

            // Update session if callback payload contains new channel info
            if (response.payload.ContainsKey("link_ch") && response.payload.ContainsKey("link_key") && response.payload.ContainsKey("link_name"))
            {
                try
                {
                    LinkUtils.SessionMetadata(response.payload, response.resolved.SigningRequest, this.metadata);

                    this.channelUrl = (string)response.payload["link_ch"];
                    this.channelName = (string)response.payload["link_name"];
                    this.channelKey = (string)response.payload["link_key"];

                    this.metadata["name"] = this.channelName;
                }
                catch (System.Exception e)
                {
                    Debug.LogWarning("Unable to recover link session");
                    Debug.LogException(e);
                }
            }
            return response;
        }

        #endregion

    }

    public class ChannelInfo
    {

        /// <summary>
        /// Public key requests are encrypted to.
        /// </summary>
        public string key;

        /// <summary>
        /// The wallet given channel name, usually the device name.
        /// </summary>
        public string name;

        /// <summary>
        /// The channel push URL.
        /// </summary>
        public string url;

    }

    public class LinkChannelSessionData : LinkSessionData
    {

        /// <summary>
        /// The wallet channel URL.
        /// </summary>
        public ChannelInfo channel;

        /// <summary>
        /// The private request key.
        /// </summary>
        public string requestKey;

    }

}