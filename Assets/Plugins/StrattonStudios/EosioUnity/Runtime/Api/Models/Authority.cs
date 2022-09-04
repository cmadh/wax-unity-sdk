using System;
using System.Collections.Generic;

namespace StrattonStudios.EosioUnity.Models
{

    [Serializable]
    public class Authority
    {

        public UInt32 threshold;

        public List<AuthorityKey> keys;

        public List<AuthorityAccount> accounts;

        public List<AuthorityWait> waits;
    }

}