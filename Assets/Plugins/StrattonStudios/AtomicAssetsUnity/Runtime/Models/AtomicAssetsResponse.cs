using System.Collections;
using System.Collections.Generic;

using Newtonsoft.Json;

using UnityEngine;

namespace StrattonStudios.AtomicAssetsUnity.Models
{

    public class AtomicAssetsResponse<TData>
    {

        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("data")]
        public TData Data { get; set; }

        [JsonProperty("query_time")]
        public long QueryTime { get; set; }

    }

}