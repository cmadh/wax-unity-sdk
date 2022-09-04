using System;
using System.Collections.Generic;

namespace StrattonStudios.EosioUnity.Models
{

    [Serializable]
    public class GetActionsResponse
    {

        public List<GlobalAction> actions;

        public UInt32 last_irreversible_block;

        public bool time_limit_exceeded_error;
    }

}