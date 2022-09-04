using System;
using System.Collections.Generic;

namespace StrattonStudios.EosioUnity.Models
{

    [Serializable]
    public class PackedTransaction
    {

        public string compression;

        public List<object> context_free_data;

        public string id;

        public string packed_context_free_data;

        public string packed_trx;

        public List<string> signatures;

        public SerializableTransaction transaction;
    }

}