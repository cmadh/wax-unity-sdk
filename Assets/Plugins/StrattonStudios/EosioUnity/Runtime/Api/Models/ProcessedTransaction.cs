using System;
using System.Collections.Generic;

namespace StrattonStudios.EosioUnity.Models
{

    [Serializable]
    public class ProcessedTransaction
    {

        public string id;

        public TransactionReceipt receipt;

        public UInt32 elapsed;

        public UInt32 net_usage;

        public bool scheduled;

        public string except;

        public List<ActionTrace> action_traces;
    }

}