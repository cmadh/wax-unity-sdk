using System;
using System.Collections.Generic;

namespace StrattonStudios.EosioUnity.Models
{

    [Serializable]
    public class GetBlockHeaderStateResponse
    {

        public Schedule active_schedule;

        public UInt32 bft_irreversible_blocknum;

        public UInt32 block_num;

        public string block_signing_key;

        public Merkle blockroot_merkle;

        public List<UInt32> confirm_count;

        public object confirmations;

        public UInt32 dpos_irreversible_blocknum;

        public UInt32 dpos_proposed_irreversible_blocknum;

        public SignedBlockHeader header;

        public string id;

        public Schedule pending_schedule;

        public ActivedProtocolFeatures activated_protocol_features;

        public List<List<string>> producer_to_last_produced;

        public List<List<string>> producer_to_last_implied_irb;
    }

}