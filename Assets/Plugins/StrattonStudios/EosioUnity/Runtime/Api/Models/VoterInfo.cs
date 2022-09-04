using System;
using System.Collections.Generic;

namespace StrattonStudios.EosioUnity.Models
{

    [Serializable]
    public class VoterInfo
    {

        public bool? is_proxy;

        public double? last_vote_weight;

        public string owner;

        public List<string> producers;

        public double? proxied_vote_weight;

        public string proxy;

        public UInt64? staked;
    }

}