using System;

namespace StrattonStudios.EosioUnity.Models
{

    [Serializable]
    public class GetInfoResponse
    {

        public string server_version;

        public string chain_id;

        public UInt32 head_block_num;

        public UInt32 last_irreversible_block_num;

        public string last_irreversible_block_id;

        public string head_block_id;

        public DateTime head_block_time;

        public string head_block_producer;

        public string virtual_block_cpu_limit;

        public string virtual_block_net_limit;

        public string block_cpu_limit;

        public string block_net_limit;
    }

}