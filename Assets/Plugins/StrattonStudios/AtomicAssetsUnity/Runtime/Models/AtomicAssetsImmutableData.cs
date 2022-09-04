using System.Collections;
using System.Collections.Generic;

using Newtonsoft.Json;

using UnityEngine;

namespace StrattonStudios.AtomicAssetsUnity.Models
{

    public class AtomicAssetsImmutableData
    {

        [JsonProperty("img")]
        public string Image { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

    }

}