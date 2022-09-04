using System;
using System.Collections.Generic;

namespace StrattonStudios.EosioUnity.Models
{

    [Serializable]
    public class GetScheduledTransactionsResponse
    {

        public List<ScheduledTransaction> transactions;

        public string more;
    }

}