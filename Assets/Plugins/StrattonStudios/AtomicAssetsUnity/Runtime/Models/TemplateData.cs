using System.Collections;
using System.Collections.Generic;

using Newtonsoft.Json;

using UnityEngine;

namespace StrattonStudios.AtomicAssetsUnity.Models
{

    public class TemplateData
    {

        [JsonProperty("contract")]
        public string Contract { get; set; }

        [JsonProperty("template_id")]
        public string TemplateId { get; set; }

        [JsonProperty("max_supply")]
        public string MaxSupply { get; set; }

        [JsonProperty("is_transferable")]
        public bool Transferable { get; set; }

        [JsonProperty("is_burnable")]
        public bool Burnable { get; set; }

        [JsonProperty("issued_supply")]
        public string IssuedSupply { get; set; }

        [JsonProperty("created_at_block")]
        public float CreatedAtBlock { get; set; }

        [JsonProperty("created_at_time")]
        public float CreatedAtTime { get; set; }

        [JsonProperty("immutable_data")]
        public AtomicAssetsImmutableData ImmutableData { get; set; }

        [JsonProperty("assets")]
        public string Assets { get; set; }

        [JsonProperty("collection")]
        public CollectionData Collection { get; set; }

        [JsonProperty("scheme")]
        public SchemaData Schema { get; set; }

    }

}