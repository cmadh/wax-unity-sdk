using System;
using System.Collections.Generic;

namespace StrattonStudios.EosioUnity.Models
{

    [Serializable]
    public class GetProducersResponse
    {

        public List<object> rows;

        public double total_producer_vote_weight;

        public string more;
    }

}