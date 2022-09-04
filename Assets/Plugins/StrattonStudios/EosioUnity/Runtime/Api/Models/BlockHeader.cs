using System;

namespace StrattonStudios.EosioUnity.Models
{

    [Serializable]
    public class BlockHeader
    {

        public DateTime timestamp;

        public string producer;

        public UInt32 confirmed;

        public string previous;

        public string transaction_mroot;

        public string action_mroot;

        public UInt32 schedule_version;

        public object new_producers;

        public object header_extensions;
    }

}