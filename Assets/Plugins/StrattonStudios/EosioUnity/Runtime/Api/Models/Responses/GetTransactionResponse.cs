using System;
using System.Collections.Generic;

namespace StrattonStudios.EosioUnity.Models
{

    [Serializable]
    public class GetTransactionResponse
    {

        public string id;

        public DetailedTransaction trx;

        public DateTime block_time;

        public UInt32 block_num;

        public UInt32 last_irreversible_block;

        public List<ActionTrace> traces;
    }

}