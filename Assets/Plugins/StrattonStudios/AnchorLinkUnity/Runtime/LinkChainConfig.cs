using StrattonStudios.EosioUnity;

namespace StrattonStudios.AnchorLinkUnity
{

    /// <summary>
    /// Type describing a EOSIO chain.
    /// </summary>
    public class LinkChainConfig
    {

        private Chain chain = Chain.WAX;
        private ChainType chainType = ChainType.Mainnet;

        private string nodeUrl;

        /// <summary>
        /// The chains unique 32-byte id.
        /// </summary>
        public Chain Chain
        {
            get
            {
                if (this.chain == null)
                {
                    this.chain = Chain.WAX;
                }
                return this.chain;
            }
            //set
            //{
            //    this.chain = value;
            //}
        }

        /// <summary>
        /// Specifies the chain type, whether Mainnet or Testnet.
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
                        this.chain = Chain.WAX;
                        break;
                    case ChainType.Testnet:
                        this.chain = Chain.WAXTestnet;
                        break;
                }
            }
        }

        /// <summary>
        /// URL to EOSIO node to communicate with.
        /// </summary>
        public string NodeUrl
        {
            get
            {
                return this.nodeUrl;
            }
            set
            {
                this.nodeUrl = value;
            }
        }

    }

}