using System;
using System.Collections.Generic;

namespace StrattonStudios.EosioUnity.Models
{

    [Serializable]
    public class GetAccountResponse
    {

        public string account_name;

        public UInt32 head_block_num;

        public DateTime head_block_time;

        public bool privileged;

        public DateTime last_code_update;

        public DateTime created;

        public Int64 ram_quota;

        public Int64 net_weight;

        public Int64 cpu_weight;

        public Resource net_limit;

        public Resource cpu_limit;

        public UInt64 ram_usage;

        public List<Permission> permissions;

        public RefundRequest refund_request;

        public SelfDelegatedBandwidth self_delegated_bandwidth;

        public TotalResources total_resources;

        public VoterInfo voter_info;
    }

}