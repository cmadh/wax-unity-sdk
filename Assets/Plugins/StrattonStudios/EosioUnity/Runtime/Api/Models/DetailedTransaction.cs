using System;

namespace StrattonStudios.EosioUnity.Models
{

    [Serializable]
    public class DetailedTransaction
    {

        public TransactionReceipt receipt;

        public SerializableTransaction trx;
    }

}