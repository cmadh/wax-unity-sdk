using System.Collections;
using System.Collections.Generic;

using Newtonsoft.Json;

using UnityEngine;

namespace StrattonStudios.AtomicAssetsUnity.Models
{

    public class SchemaData
    {

        [JsonProperty("contract")]
        public string Contract { get; set; }

        [JsonProperty("schema_name")]
        public string SchemaName { get; set; }

        [JsonProperty("format")]
        public FormatData[] Format { get; set; }

        [JsonProperty("created_at_block")]
        public float CreatedAtBlock { get; set; }

        [JsonProperty("created_at_time")]
        public float CreatedAtTime { get; set; }

        [JsonProperty("collection")]
        public CollectionData Collection { get; set; }

    }

}