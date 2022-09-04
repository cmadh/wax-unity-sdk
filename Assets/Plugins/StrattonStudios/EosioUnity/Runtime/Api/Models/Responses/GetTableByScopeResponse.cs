using System;
using System.Collections.Generic;

namespace StrattonStudios.EosioUnity.Models
{

    [Serializable]
    public class GetTableByScopeResponse
    {

        public List<TableByScopeResultRow> rows;

        public string more;
    }

}