using System;
using System.Collections.Generic;

namespace StrattonStudios.EosioUnity.Models
{

    [Serializable]
    public class GetBlockResponse
    {

        public DateTime timestamp;

        public string producer;

        public UInt32 confirmed;

        public string previous;

        public string transaction_mroot;

        public string action_mroot;

        public UInt32 schedule_version;

        public Schedule new_producers;

        public List<SerializableExtension> block_extensions;

        public List<SerializableExtension> header_extensions;

        public string producer_signature;

        public List<TransactionReceipt> transactions;

        public string id;

        public UInt32 block_num;

        public UInt32 ref_block_prefix;
    }

}