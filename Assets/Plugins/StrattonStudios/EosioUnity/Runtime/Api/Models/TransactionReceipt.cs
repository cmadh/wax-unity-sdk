using System;

namespace StrattonStudios.EosioUnity.Models
{

    [Serializable]
    public class TransactionReceipt
    {

        public string status;

        public UInt32? cpu_usage_us;

        public UInt32? net_usage_words;

        public object trx;
    }

}