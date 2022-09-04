using System.Collections;
using System.Collections.Generic;

using Newtonsoft.Json;

using UnityEngine;

namespace StrattonStudios.AtomicAssetsUnity.Models
{

    public class BackedTokensData
    {

        [JsonProperty("token_contract")]
        public string TokenContract { get; set; }

        [JsonProperty("token_symbol")]
        public string TokenSymbol { get; set; }

        [JsonProperty("token_precision")]
        public int TokenPrecision { get; set; }

        [JsonProperty("amount")]
        public string Amount { get; set; }

    }

}