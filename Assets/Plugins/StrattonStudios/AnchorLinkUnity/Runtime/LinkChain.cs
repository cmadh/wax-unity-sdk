using System.Collections;
using System.Collections.Generic;

using Cysharp.Threading.Tasks;

using StrattonStudios.EosioUnity;
using StrattonStudios.EosioUnity.Models;
using StrattonStudios.EosioUnity.Signing;

using UnityEngine;

namespace StrattonStudios.AnchorLinkUnity
{

    /// <summary>
    /// Class representing a EOSIO chain.
    /// </summary>
    public class LinkChain : IAbiProvider
    {

        /// <summary>
        /// EOSIO ChainID for which requests are valid.
        /// </summary>
        private ChainId chainId;

        /// <summary>
        /// API client instance used to communicate with the chain.
        /// </summary>
        private EosioClient client;

        private ChainType chainType;

        private Dictionary<string, Abi> abiCache = new Dictionary<string, Abi>();
        private Dictionary<string, UniTask<GetAbiResponse>> pendingAbis = new Dictionary<string, UniTask<GetAbiResponse>>();

        public ChainId ChainId
        {
            get
            {
                return this.chainId;
            }
        }

        public EosioClient Client
        {
            get
            {
                return this.client;
            }
        }

        public ChainType ChainType
        {
            get
            {
                return this.chainType;
            }
            set
            {
                this.chainType = value;
            }
        }

        public LinkChain(ChainId chainId, EosioClient client, ChainType chainType)
        {
            this.chainId = chainId;
            this.client = client;
            this.chainType = chainType;
        }

        /// <summary>
        /// Fetch the ABI for given account, cached.
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public async UniTask<Abi> GetAbi(string account)
        {
            if (this.abiCache.ContainsKey(account))
            {
                return this.abiCache[account];
            }
            if (!this.pendingAbis.ContainsKey(account))
            {
                var getAbi = this.client.ChainApi.GetAbi(new GetAbiRequest() { account_name = account });
                this.pendingAbis.Add(account, getAbi);
            }
            var result = await this.pendingAbis[account];
            this.pendingAbis.Remove(account);
            this.abiCache[account] = result.abi;
            return result.abi;
        }

    }

}