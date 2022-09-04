using System;

namespace StrattonStudios.EosioUnity.Models
{

    [Serializable]
    public class AbiJsonToBinRequest
    {
        public string code;
        public string action;
        public object args;
    }

}