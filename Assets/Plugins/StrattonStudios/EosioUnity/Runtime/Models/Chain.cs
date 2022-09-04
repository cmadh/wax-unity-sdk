using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace StrattonStudios.EosioUnity
{

    /// <summary>
    /// Chain model.
    /// </summary>
    public class Chain
    {

        public static readonly Chain UNKNOWN = new Chain("UNKNOWN", 0, null);
        public static readonly Chain EOS = new Chain("EOS", 1, "aca376f206b8fc25a6ed44dbdc66547c36c6c33e3a119ffbeaef943642f0e906");
        public static readonly Chain TELOS = new Chain("TELOS", 2, "4667b205c6838ef70ff7988f6e8257e8be0e1284a2f59699054a018f743b1d11");
        public static readonly Chain JUNGLE = new Chain("JUNGLE", 3, "e70aaab8997e1dfce58fbfac80cbbb8fecec7b99cf982a9444273cbc64c41473");
        public static readonly Chain KYLIN = new Chain("KYLIN", 4, "5fff1dae8dc8e2fc4d5b23b2c7665c97f9e9d8edf2b6485a86ba311c25639191");
        public static readonly Chain WORBLI = new Chain("WORBLI", 5, "73647cde120091e0a4b85bced2f3cfdb3041e266cbbe95cee59b73235a1b3b6f");
        public static readonly Chain BOS = new Chain("BOS", 6, "d5a3d18fbb3c084e3b1f3fa98c21014b5f3db536cc15d08f9f6479517c6a3d86");
        public static readonly Chain MEETONE = new Chain("MEETONE", 7, "cfe6486a83bad4962f232d48003b1824ab5665c36778141034d75e57b956e422");
        public static readonly Chain INSIGHTS = new Chain("INSIGHTS", 8, "b042025541e25a472bffde2d62edd457b7e70cee943412b1ea0f044f88591664");
        public static readonly Chain BEOS = new Chain("BEOS", 9, "b912d19a6abd2b1b05611ae5be473355d64d95aeff0c09bedc8c166cd6468fe4");

        public static readonly Chain WAX = new Chain("WAX", 10, "1064487b3cd1a897ce03ae5b6a865651747e2e152090f99c1d19d44e01aea5a4");
        public static readonly Chain WAXTestnet = new Chain("WAXTestnet", 0, "f16b1833c747c43682f4386fca9cbb327929334a762755ebec17f6f23c9b8a12");

        public static readonly Chain PROTON = new Chain("PROTON", 11, "384da888112027f0321850a169f737c33e53b388aad48b5adace4bab97f437e0");
        public static readonly Chain FIO = new Chain("FIO", 12, "21dcae42c0182200e93f954a074011f9048a7624c6fe81d3c9541a614a88bd1c");

        public static IEnumerable<Chain> Values
        {
            get
            {
                yield return UNKNOWN;
                yield return EOS;
                yield return TELOS;
                yield return JUNGLE;
                yield return KYLIN;
                yield return WORBLI;
                yield return BOS;
                yield return MEETONE;
                yield return INSIGHTS;
                yield return BEOS;

                yield return WAX;
                yield return WAXTestnet;

                yield return PROTON;
                yield return FIO;
            }
        }

        private readonly string name;
        private readonly string chainId;
        private readonly int chainAlias;
        private readonly bool testnet = false;

        public string Name
        {
            get
            {
                return this.name;
            }
        }

        private Chain(string name, int alias, string chainId, bool testnet = false)
        {
            this.name = name;
            this.chainAlias = alias;
            this.chainId = chainId;
            this.testnet = testnet;
        }

        public static Chain FromChainId(string chainId)
        {
            if (chainId == null)
            {
                return UNKNOWN;
            }

            chainId = chainId.ToLower();

            foreach (Chain id in Values)
            {
                if (chainId.Equals(id.GetId()))
                {
                    return id;
                }
            }

            return UNKNOWN;
        }

        public static Chain FromChainAlias(int alias)
        {
            foreach (Chain id in Values)
            {
                if (alias == id.GetAlias())
                {
                    return id;
                }
            }

            return UNKNOWN;
        }

        public int GetAlias()
        {
            return this.chainAlias;
        }

        public string GetId()
        {
            return this.chainId;
        }

        public ChainId ToChainId()
        {
            return new ChainId(this);
        }

    }

}