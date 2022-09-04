using System.Collections;
using System.Collections.Generic;

using Newtonsoft.Json;

using UnityEngine;

namespace StrattonStudios.AtomicAssetsUnity.Models
{

    public class CollectionData
    {

        [JsonProperty("contract")]
        public string Contract { get; set; }

        [JsonProperty("collection_name")]
        public string CollectionName { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("img")]
        public string Image { get; set; }

        [JsonProperty("author")]
        public string Author { get; set; }

        [JsonProperty("allow_notify")]
        public bool AllowNotify { get; set; }

        [JsonProperty("authorized_accounts")]
        public string[] AuthorizedAccounts { get; set; }

        [JsonProperty("notify_accounts")]
        public string[] NotifyAccounts { get; set; }

        [JsonProperty("market_fee")]
        public float MarketFee { get; set; }

        [JsonProperty("data")]
        public object Data { get; set; }

        [JsonProperty("assets")]
        public string Assets { get; set; }

        [JsonProperty("created_at_block")]
        public float CreatedAtBlock { get; set; }

        [JsonProperty("created_at_time")]
        public float CreatedAtTime { get; set; }

    }

}