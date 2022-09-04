using System;

namespace StrattonStudios.EosioUnity.Models
{

    [Serializable]
    public class TableByScopeResultRow
    {

        public string code;

        public string scope;

        public string table;

        public string payer;

        public UInt32? count;
    }

}