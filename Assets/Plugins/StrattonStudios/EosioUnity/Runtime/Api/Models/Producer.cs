using System;

namespace StrattonStudios.EosioUnity.Models
{

    [Serializable]
    public class Producer
    {
        [EosioAbiFieldType("name")]
        public string owner;
        [EosioAbiFieldType("float64")]
        public double total_votes;
        [EosioAbiFieldType("public_key")]
        public string producer_key;

        public bool is_active;

        public string url;

        public UInt32 unpaid_blocks;

        public UInt64 last_claim_time;

        public UInt16 location;
    }

}