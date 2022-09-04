using System;
using System.Collections.Generic;

namespace StrattonStudios.EosioUnity.Models
{

    [Serializable]
    public class Receipt
    {

        public string receiver;

        public string act_digest;

        public UInt64? global_sequence;

        public UInt64? recv_sequence;

        public List<List<object>> auth_sequence;

        public UInt64? code_sequence;

        public UInt64? abi_sequence;
    }

}