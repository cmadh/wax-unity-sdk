using System.Collections;
using System.Collections.Generic;

using Newtonsoft.Json;

using UnityEngine;

namespace StrattonStudios.AtomicAssetsUnity.Models
{

    public class StatsData
    {

        [JsonProperty("template_mint")]
        public int TemplateMint { get; set; }

    }

}