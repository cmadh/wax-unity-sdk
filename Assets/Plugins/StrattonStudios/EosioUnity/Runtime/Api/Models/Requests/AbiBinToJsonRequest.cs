using System;

namespace StrattonStudios.EosioUnity.Models
{

    [Serializable]
    public class AbiBinToJsonRequest
    {
        public string code;
        public string action;
        public string binargs;
    }

}