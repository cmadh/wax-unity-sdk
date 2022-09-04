using System;

namespace StrattonStudios.EosioUnity.Models
{

    [Serializable]
    public class GetActionsRequest
    {
        public string account_name;
        public Int32 pos;
        public Int32 offset;
    }

}