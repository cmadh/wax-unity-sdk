using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace StrattonStudios.EosioUnity
{

    /// <summary>
    /// Chain ID model.
    /// </summary>
    public class ChainId : IJsonModel
    {

        public const string CHAIN_ID = "chain_id";
        public const string CHAIN_IDS = "chain_ids";

        private const string ALIAS_LABEL = "chain_alias";

        private string chainId;
        private int chainAlias;
        private string chainName;

        public ChainId(Chain chain)
        {
            this.chainId = chain.GetId();
            this.chainAlias = chain.GetAlias();
            this.chainName = chain.Name.ToLower();
        }

        public ChainId(string chainId, int chainAlias, string chainName)
        {
            this.chainId = chainId;
            this.chainAlias = chainAlias;
            this.chainName = chainName;
        }

        public string GetChainId()
        {
            return this.chainId;
        }

        public int GetChainAlias()
        {
            return this.chainAlias;
        }

        public string GetChainName()
        {
            return this.chainName;
        }

        public static ChainId FromVariant(JArray variant)
        {
            if (ALIAS_LABEL.Equals((string)variant[0]))
            {
                int alias = (int)variant[1];
                Chain chain = Chain.FromChainAlias(alias);
                if (chain == Chain.UNKNOWN)
                    throw new EosioException("Cannont create ChainId from variant, chain alias unknown");

                return new ChainId(chain);
            }
            else
            {
                string id = (string)variant[0];
                Chain chain = Chain.FromChainId(id);
                if (chain == Chain.UNKNOWN)
                    return new ChainId(id, Chain.UNKNOWN.GetAlias(), Chain.UNKNOWN.Name);

                return new ChainId(chain);
            }
        }

        public static ChainId FromVariant(List<object> variant)
        {
            if (ALIAS_LABEL.Equals((string)variant[0]))
            {
                int alias = (byte)variant[1];
                Chain chain = Chain.FromChainAlias(alias);
                if (chain == Chain.UNKNOWN)
                    throw new EosioException("Cannont create ChainId from variant, chain alias unknown");

                return new ChainId(chain);
            }
            else
            {
                string id = (string)variant[0];
                Chain chain = Chain.FromChainId(id);
                if (chain == Chain.UNKNOWN)
                    return new ChainId(id, Chain.UNKNOWN.GetAlias(), Chain.UNKNOWN.Name);

                return new ChainId(chain);
            }
        }

        public static ChainId FromVariant(KeyValuePair<string, object> variant)
        {
            if (ALIAS_LABEL.Equals(variant.Key))
            {
                int alias = (byte)variant.Value;
                Chain chain = Chain.FromChainAlias(alias);
                if (chain == Chain.UNKNOWN)
                    throw new EosioException("Cannont create ChainId from variant, chain alias unknown");

                return new ChainId(chain);
            }
            else
            {
                string id = (string)variant.Value;
                Chain chain = Chain.FromChainId(id);
                if (chain == Chain.UNKNOWN)
                    return new ChainId(id, Chain.UNKNOWN.GetAlias(), Chain.UNKNOWN.Name);

                return new ChainId(chain);
            }
        }

        public List<object> ToVariant()
        {
            //if (this.chainAlias != Chain.UNKNOWN.GetAlias())
            //{
            //    List<object> variant = new List<object>();
            //    variant.Add(ALIAS_LABEL);
            //    variant.Add(this.chainAlias);
            //    return variant;
            //}
            //else
            //{
            List<object> variant = new List<object>();
            variant.Add(CHAIN_ID);
            variant.Add(this.chainId);
            return variant;
            //}
        }

        public string ToJSON()
        {
            return JsonConvert.SerializeObject(ToVariant());
        }

    }

}