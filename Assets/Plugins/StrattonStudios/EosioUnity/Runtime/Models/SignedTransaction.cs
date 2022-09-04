using System;
using System.Collections.Generic;
using System.Text;

namespace StrattonStudios.EosioUnity.Models
{

    public class SignedTransaction
    {

        public IEnumerable<string> Signatures { get; set; }

        public byte[] PackedTransaction { get; set; }

    }

}
