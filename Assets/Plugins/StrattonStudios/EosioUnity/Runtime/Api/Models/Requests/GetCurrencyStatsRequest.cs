using System;

namespace StrattonStudios.EosioUnity.Models
{

    [Serializable]
    public class GetCurrencyStatsRequest
    {
        public string code;
        public string symbol;
    }

}