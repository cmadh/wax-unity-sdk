using System;

namespace StrattonStudios.EosioUnity.Models
{

    [Serializable]
    public class GetTableByScopeRequest
    {
        public string code;
        public string table;
        public string lower_bound;
        public string upper_bound;
        public Int32 limit = 10;
        public bool reverse;
    }

}