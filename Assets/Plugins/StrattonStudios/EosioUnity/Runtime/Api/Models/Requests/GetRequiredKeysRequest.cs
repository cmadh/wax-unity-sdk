using System;
using System.Collections.Generic;

namespace StrattonStudios.EosioUnity.Models
{

    [Serializable]
    public class GetRequiredKeysRequest
    {
        public Transaction transaction;
        public List<string> available_keys;
    }

}