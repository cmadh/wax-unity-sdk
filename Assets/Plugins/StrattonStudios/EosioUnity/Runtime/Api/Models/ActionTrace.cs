using System;
using System.Collections.Generic;

namespace StrattonStudios.EosioUnity.Models
{

    [Serializable]
    public class ActionTrace
    {

        public Receipt receipt;

        public SerializableAction act;

        public UInt32? elapsed;

        public UInt32? cpu_usage;

        public string console;

        public UInt32? total_cpu_usage;

        public string trx_id;

        public List<ActionTrace> inline_traces;
    }

}