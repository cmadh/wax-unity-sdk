using System;

namespace StrattonStudios.EosioUnity
{

    /// <summary>
    /// Provides configuration for the <see cref="EosioClient"/>
    /// </summary>
    [Serializable]
    public class EosioClientConfig
    {

        protected string httpEndpoint = "http://127.0.0.1:8888";
        protected ChainType chainType = ChainType.Mainnet;
        protected string chainId = Chain.WAX.GetId();
        protected double expireSeconds = 60;
        protected uint blocksBehind = 3;

        /// <summary>
        /// Gets or sets the URL of the server that provides a Chain API specified by <see cref="ChainId"/>.
        /// </summary>
        public string HttpEndpoint
        {
            get
            {
                return this.httpEndpoint;
            }
            set
            {
                this.httpEndpoint = value;
            }
        }

        /// <summary>
        /// Gets the hash or unique ID of the Chain to connect to which is WAX by default, you can use <see cref="Chain"/> class for convenience.
        /// </summary>
        public string ChainId
        {
            get
            {
                if (string.IsNullOrEmpty(this.chainId))
                {
                    this.chainId = Chain.WAX.GetId();
                }
                return this.chainId;
            }
        }

        /// <summary>
        /// Specifies the chain type to use, whether <see cref="
        /// ChainType.Mainnet"/> or <see cref="ChainType.Testnet"/>.
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
                        this.chainId = Chain.WAX.GetId();
                        break;
                    case ChainType.Testnet:
                        this.chainId = Chain.WAXTestnet.GetId();
                        break;
                }
            }
        }

        /// <summary>
        /// Gets or sets the number of seconds before the transaction will expire. The time is based on the Node's clock. 
        /// An unexpired transaction that may have had an error is a liability until the expiration is reached, this time should be brief.
        /// </summary>
        public double ExpireSeconds
        {
            get
            {
                return this.expireSeconds;
            }
            set
            {
                this.expireSeconds = value;
            }
        }

        /// <summary>
        /// Gets or sets how many blocks behind to use for TAPoS reference block
        /// </summary>
        public uint BlocksBehind
        {
            get
            {
                return this.blocksBehind;
            }
            set
            {
                this.blocksBehind = value;
            }
        }

        /// <summary>
        /// Gets or sets the signature implementation to handle available keys and signing transactions. Use the DefaultSignProvider with a privateKey to sign transactions inside the lib.
        /// </summary>
        public IEosioSignProvider SignProvider { get; set; }

    }

}