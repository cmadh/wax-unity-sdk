using System;
using System.Collections.Generic;

using static UnityEditor.MaterialProperty;

namespace StrattonStudios.EosioUnity.Models
{

    [Serializable]
    public class GetTableRowsResponse : GetTableRowsResponse<object>
    {
    }

    [Serializable]
    public class GetTableRowsResponse<TRowType>
    {

        public List<TRowType> rows;

        public bool more;
    }

}