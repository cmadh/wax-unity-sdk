using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace StrattonStudios.EosioUnity
{

    /// <summary>
    /// Transaction context model.
    /// </summary>
    public class TransactionContext : IJsonModel
    {

        private const string TIMESTAMP = "timestamp";
        private const string EXPIRE_SECONDS = "expire_seconds";
        private const string BLOCK_NUM = "block_num";

        private uint? timestamp;
        private uint expireSeconds = 60;
        private ushort? blockNum;
        private ushort? refBlockNum;
        private uint? refBlockPrefix;
        private string expiration;

        private Chain chainId;

        public uint? Timestamp
        {
            get
            {
                return this.timestamp;
            }
            set
            {
                this.timestamp = value;
            }
        }

        public uint ExpireSeconds
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

        public ushort? BlockNum
        {
            get
            {
                return this.blockNum;
            }
            set
            {
                this.blockNum = value;
            }
        }

        public ushort? RefBlockNum
        {
            get
            {
                return this.refBlockNum;
            }
            set
            {
                this.refBlockNum = value;
            }
        }

        public uint? RefBlockPrefix
        {
            get
            {
                return this.refBlockPrefix;
            }
            set
            {
                this.refBlockPrefix = value;
            }
        }

        public string Expiration
        {
            get
            {
                return this.expiration;
            }
            set
            {
                this.expiration = value;
            }
        }

        public Chain ChainId
        {
            get
            {
                return this.chainId;
            }
            set
            {
                this.chainId = value;
            }
        }

        public TransactionContext()
        {
        }

        public void SetChain(string chainId)
        {
            this.chainId = Chain.FromChainId(chainId);
        }

        public string ToJSON()
        {
            Dictionary<string, object> toEncode = new Dictionary<string, object>();
            toEncode.Add(TIMESTAMP, Timestamp);
            toEncode.Add(EXPIRE_SECONDS, ExpireSeconds);
            toEncode.Add(BLOCK_NUM, BlockNum);
            toEncode.Add(Transaction.REF_BLOCK_NUM, RefBlockNum);
            toEncode.Add(Transaction.REF_BLOCK_PREFIX, RefBlockPrefix);
            toEncode.Add(Transaction.EXPIRATION, Expiration);

            return JsonConvert.SerializeObject(toEncode);
        }

    }

}