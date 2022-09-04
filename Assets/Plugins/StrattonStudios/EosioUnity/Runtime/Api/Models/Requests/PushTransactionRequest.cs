using System;

namespace StrattonStudios.EosioUnity.Models
{

    [Serializable]
    public class PushTransactionRequest
    {
        public string[] signatures;
        public UInt32 compression;
        public string packed_context_free_data;
        public string packed_trx;
        public SerializableTransaction transaction;
    }

}