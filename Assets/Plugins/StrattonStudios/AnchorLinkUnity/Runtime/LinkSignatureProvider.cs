using System.Collections;
using System.Collections.Generic;

using Cysharp.Threading.Tasks;

using StrattonStudios.EosioUnity;
using StrattonStudios.EosioUnity.Serialization;
using StrattonStudios.EosioUnity.Signing;

using UnityEngine;

namespace StrattonStudios.AnchorLinkUnity
{

    public class LinkSignatureProvider
    {

        protected Link link;
        protected ILinkCallbackService callbackService;
        //protected LinkChain linkChain;
        protected ILinkTransport transport;

        protected List<string> availableKeys = new List<string>();

        public LinkSignatureProvider(Link link, ILinkCallbackService callbackService, ILinkTransport transport, string[] availableKeys)
        {
            this.link = link;
            this.callbackService = callbackService;
            //this.linkChain = linkChain;
            this.transport = transport;
            this.availableKeys = new List<string>(availableKeys);
        }

        public UniTask<string[]> GetAvailableKeys()
        {
            return UniTask.FromResult(this.availableKeys.ToArray());
        }

        public async UniTask<TransactResult> Sign(string chainId, IEnumerable<string> requiredKeys, byte[] signBytes, IEnumerable<string> abiNames = null)
        {
            var c = this.link.ActiveChain;
            var options = new SigningRequestEncodingOptions();
            options.abiProvider = c;
            options.zlib = DefaultZlibProvider.Instance;
            var request = SigningRequest.FromTransaction(Chain.FromChainId(chainId).ToChainId(), signBytes, options);
            var callback = this.callbackService.Create();
            request.SetCallback(callback.Url);
            request.GetRequestFlag().SetBackground(true).SetBroadcast(false);

            var t = this.transport ?? this.link.Transport;
            request = await t.Prepare(request, null);

            var result = await this.link.SendRequest(request, callback, t, false);

            var serializer = new AbiTypeWriter(this.link.Client);
            var serializedTransaction = await serializer.SerializeTransaction(result.transaction, null);
            result.serializedTransaction = serializedTransaction;
            return result;
        }

    }

}