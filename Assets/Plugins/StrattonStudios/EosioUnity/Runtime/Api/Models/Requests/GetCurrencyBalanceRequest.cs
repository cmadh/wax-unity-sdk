using System;

namespace StrattonStudios.EosioUnity.Models
{

    [Serializable]
    public class GetCurrencyBalanceRequest
    {
        public string code;
        public string account;
        public string symbol;
    }

}