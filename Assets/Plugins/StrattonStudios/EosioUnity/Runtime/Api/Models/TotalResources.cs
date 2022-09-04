using System;

namespace StrattonStudios.EosioUnity.Models
{

    [Serializable]
    public class TotalResources
    {

        public string cpu_weight;

        public string net_weight;

        public string owner;

        public UInt64? ram_bytes;
    }

}