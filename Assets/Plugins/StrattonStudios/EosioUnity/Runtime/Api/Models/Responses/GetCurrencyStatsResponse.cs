using System;
using System.Collections.Generic;

namespace StrattonStudios.EosioUnity.Models
{

    [Serializable]
    public class GetCurrencyStatsResponse
    {

        public Dictionary<string, CurrencyStat> stats;
    }

}