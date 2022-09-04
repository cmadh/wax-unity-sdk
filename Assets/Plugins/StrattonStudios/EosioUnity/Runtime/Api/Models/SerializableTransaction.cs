using System;
using System.Collections.Generic;

namespace StrattonStudios.EosioUnity.Models
{

    [Serializable]
    public class SerializableTransaction
    {

        public DateTime expiration;

        public UInt16 ref_block_num;

        public UInt32 ref_block_prefix;

        public UInt32 max_net_usage_words;

        public byte max_cpu_usage_ms;

        public UInt32 delay_sec;

        public List<SerializableAction> context_free_actions = new List<SerializableAction>();

        public List<SerializableAction> actions = new List<SerializableAction>();

        public List<SerializableExtension> transaction_extensions = new List<SerializableExtension>();
    }

}