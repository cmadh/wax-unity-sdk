using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace StrattonStudios.EosioUnity.Signing
{

    public class SigningRequestCommonArgs
    {

        protected Chain chainId = Chain.WAX;
        protected ChainType chainType = ChainType.Mainnet;

        /// <summary>
        /// Chain ID to use, can be set to `null` for a multi-chain request.
        /// </summary>
        /// <remarks>
        /// Defaults to WAX
        /// </remarks>
        public Chain ChainId
        {
            get
            {
                if (this.chainId == null)
                {
                    this.chainId = Chain.WAX;
                }
                return this.chainId;
            }
        }

        /// <summary>
        /// Chain IDs to constrain a multi-chain request to.
        /// </summary>
        /// <remarks>
        /// Only considered if <see cref="ChainId"/> is explicitly set to <c>null</c>.
        /// </remarks>
        //public List<Chain> chainIds;

        /// <summary>
        /// Optional metadata to pass along with the request.
        /// </summary>
        public List<InfoPair> info;

        /// <summary>
        /// Specifies the chain type to use, whether Mainnet or Testnet.
        /// </summary>
        public ChainType ChainType
        {
            get
            {
                return this.chainType;
            }
            set
            {
                this.chainType = value;
                switch (this.chainType)
                {
                    case ChainType.Mainnet:
                        this.chainId = Chain.WAX;
                        break;
                    case ChainType.Testnet:
                        this.chainId = Chain.WAXTestnet;
                        break;
                }
            }
        }

    }

}