using System.Collections;
using System.Collections.Generic;

using Newtonsoft.Json;

using UnityEngine;

namespace StrattonStudios.AtomicAssetsUnity.Models
{

    public class AssetData
    {

        [JsonProperty("contract")]
        public string Contract { get; set; }

        [JsonProperty("asset_id")]
        public string AssetId { get; set; }

        [JsonProperty("owner")]
        public string Owner { get; set; }

        [JsonProperty("is_transferable")]
        public bool Transferable { get; set; }

        [JsonProperty("is_burnable")]
        public bool Burnable { get; set; }

        [JsonProperty("collection")]
        public CollectionData Collection { get; set; }

        [JsonProperty("schema")]
        public SchemaData Schema { get; set; }

        [JsonProperty("template")]
        public TemplateData Template { get; set; }

        [JsonProperty("mutable_data")]
        public object MutableData { get; set; }

        [JsonProperty("immutable_data")]
        public object ImmutableData { get; set; }

        [JsonProperty("template_mint")]
        public string TemplateMint { get; set; }

        [JsonProperty("backed_tokens")]
        public BackedTokensData[] BackedTokens { get; set; }

        [JsonProperty("updated_at_block")]
        public string UpdatedAtBlock { get; set; }

        [JsonProperty("updated_at_time")]
        public string UpdatedAtTime { get; set; }

        [JsonProperty("transferred_at_block")]
        public string TransferredAtBlock { get; set; }

        [JsonProperty("transferred_at_time")]
        public string TransferredAtTime { get; set; }

        [JsonProperty("minted_at_block")]
        public string MintedAtBlock { get; set; }

        [JsonProperty("minted_at_time")]
        public string MintedAtTime { get; set; }


        [JsonProperty("name")]
        public string Name { get; set; }

    }

}