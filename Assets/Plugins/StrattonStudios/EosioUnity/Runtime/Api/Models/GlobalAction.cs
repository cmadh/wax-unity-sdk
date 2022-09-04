using System;

namespace StrattonStudios.EosioUnity.Models
{

    [Serializable]
    public class GlobalAction
    {

        public UInt64? global_action_seq;

        public UInt64? account_action_seq;

        public UInt32? block_num;

        public DateTime? block_time;

        public ActionTrace action_trace;
    }

}