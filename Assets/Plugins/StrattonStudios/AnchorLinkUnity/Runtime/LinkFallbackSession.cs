using System;
using System.Collections;
using System.Collections.Generic;

using Cysharp.Threading.Tasks;

using StrattonStudios.EosioUnity;
using StrattonStudios.EosioUnity.Signing;

using Newtonsoft.Json;

using UnityEngine;

namespace StrattonStudios.AnchorLinkUnity
{

    public class LinkFallbackSession : LinkSession, ILinkTransport
    {

        protected LinkFallbackSessionData sessionData;

        public ILinkStorage Storage => this.link.Transport.Storage;

        public string UserAgent => this.link.Transport.UserAgent;

        public LinkFallbackSession(Link link, LinkFallbackSessionData data, Dictionary<string, object> metadata)
        {
            this.type = "fallback";
            this.link = link;
            //this.chainId = data.chainId;
            this.auth = data.auth;
            this.publicKey = data.publicKey;

            if (metadata == null)
            {
                metadata = new Dictionary<string, object>();
            }

            this.metadata = metadata;
            this.identifier = data.identifier;
            this.sessionData = data;
        }

        public override SerializedLinkSession Serialize()
        {
            return new SerializedLinkSession()
            {
                type = "fallback",
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

        public void OnRequest(SigningRequest request, LinkCancelHandler cancel)
        {
            this.link.Transport.OnSessionRequest(this, request, cancel);
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

        public void OnSessionRequest(LinkSession session, SigningRequest request, LinkCancelHandler cancel)
        {
            this.link.Transport.OnSessionRequest(session, request, cancel);
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
            //options.chain = Chain.FromChainId(this.link.ActiveChain.ChainId.GetChainId());
            var response = await this.link.Transact(args, options, this);
            return response;
        }

    }

    public class LinkFallbackSessionData : LinkSessionData
    {

    }

}