using System.Collections;
using System.Collections.Generic;

using StrattonStudios.EosioUnity;

using UnityEngine;

namespace StrattonStudios.AnchorLinkUnity
{

    /// <summary>
    /// Available options when creating a new <see cref="Link"/> instance.
    /// </summary>
    public class LinkOptions
    {

        public const string DefaultWaxNodeUrl = "https://wax.greymass.com";

        public const string DefaultServiceUrl = "https://cb.anchor.link";
        public const bool DefaultVerifyProofs = false;
        public const ChainType DefaultChainType = ChainType.Mainnet;
        //public const bool DefaultEncodeChainIds = true;

        public static ISocketHandler DefaultSocketHandler;

        private ILinkTransport transport;

        //private List<LinkChainConfig> chains = new List<LinkChainConfig>();

        private LinkChainConfig chain = new LinkChainConfig()
        {
            //Chain = EosUnity.Chain.WAX,
            ChainType = DefaultChainType,
            NodeUrl = DefaultWaxNodeUrl
        };

        private ILinkCallbackService service;

        private ILinkStorage storage;

        private bool verifyProofs = DefaultVerifyProofs;

        private ISocketHandler socketHandler;

        //private bool encodeChainIds = DefaultEncodeChainIds;

        /// <summary>
        /// Link transport responsible for presenting signing requests to user.
        /// </summary>
        public ILinkTransport Transport
        {
            get
            {
                return this.transport;
            }
            set
            {
                this.transport = value;
            }
        }

        /// <summary>
        /// Gets or sets the chain configurations to support.
        /// </summary>
        /// <example>
        /// For example for a link that can login and transact on EOS and WAX:
        /// <code>
        /// LinkOptions options = new LinkOptions(); ;
        /// options.Chains = new List<LinkChainConfig>()
        /// {
        ///     new LinkChainConfig()
        ///     {
        ///         Chain = Chain.EOS,
        ///         NodeUrl = "https://eos.greymass.com"
        ///     },
        ///     new LinkChainConfig()
        ///     {
        ///         Chain = Chain.WAX,
        ///         NodeUrl = "https://wax.greymass.com"
        ///     }
        /// };
        /// </code>
        /// </example>
        //public List<LinkChainConfig> Chains
        //{
        //    get
        //    {
        //        return this.chains;
        //    }
        //    set
        //    {
        //        this.chains = value;
        //    }
        //}

        /// <summary>
        /// Gets or sets the chain configuration to support.
        /// </summary>
        /// <example>
        /// For example for a link that can login and transact on EOS and WAX:
        /// <code>
        /// LinkOptions options = new LinkOptions(); ;
        /// options.Chain = new LinkChainConfig()
        ///     {
        ///         Chain = Chain.EOS,
        ///         NodeUrl = "https://eos.greymass.com"
        ///     };
        /// </code>
        /// </example>
        public LinkChainConfig Chain
        {
            get
            {
                return this.chain;
            }
            set
            {
                this.chain = value;
            }
        }

        /// <summary>
        /// Gets or sets the callback forwarder service.
        /// Defaults to https://cb.anchor.link
        /// </summary>
        /// <remarks>
        /// See [buoy-nodejs](https://github.com/greymass/buoy-nodejs) and (buoy-golang)[https://github.com/greymass/buoy-golang] for reference implementations.
        /// </remarks>
        public ILinkCallbackService Service
        {
            get
            {
                if (this.service == null)
                {
                    this.service = new BuoyCallbackService(DefaultServiceUrl, SocketHandler);
                }
                return this.service;
            }
            set
            {
                this.service = value;
            }
        }

        /// <summary>
        /// Gets or sets the optional storage adapter that will be used to persist sessions. explicitly set this to `null` to force no storage.
        /// </summary>
        public ILinkStorage Storage
        {
            get
            {
                return this.storage;
            }
            set
            {
                this.storage = value;
            }
        }

        /// <summary>
        /// Whether to verify identity proofs submitted by the signer, if this is disabled the 
        /// <see cref="Link.Login">Login</see> and <see cref="Link.Identify">Identify</see> methods will not return an account object.
        /// </summary>
        public bool VerifyProofs
        {
            get
            {
                return this.verifyProofs;
            }
            set
            {
                this.verifyProofs = value;
            }
        }

        /// <summary>
        /// Whether to encode the chain ids with the identity request that establishes a session.
        /// Only applicable when using multiple chain configurations, can be set to false to
        /// decrease QR code sizes when supporting many chains.
        /// </summary>
        //public bool EncodeChainIds
        //{
        //    get
        //    {
        //        return this.encodeChainIds;
        //    }
        //    set
        //    {
        //        this.encodeChainIds = value;
        //    }
        //}

        public ISocketHandler SocketHandler
        {
            get
            {
                if (this.socketHandler == null)
                {
                    this.socketHandler = DefaultSocketHandler;
                }
                return this.socketHandler;
            }
            set
            {
                this.socketHandler = value;
            }
        }

        public void SetService(string url)
        {
            this.service = new BuoyCallbackService(url, SocketHandler);
        }

    }

}