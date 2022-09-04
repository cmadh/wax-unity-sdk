using System;

namespace StrattonStudios.EosioUnity.Models
{

    [Serializable]
    public class PushTransactionResponse
    {

        public string transaction_id;

        public ProcessedTransaction processed;
    }

}