using System.Collections;
using System.Collections.Generic;

using Newtonsoft.Json;

using UnityEngine;

namespace StrattonStudios.AtomicAssetsUnity.Models
{

    public class OfferData
    {

        [JsonProperty("contract")]
        public string Contract { get; set; }

        [JsonProperty("offer_id")]
        public string OfferId { get; set; }

        [JsonProperty("sender_name")]
        public string SenderName { get; set; }

        [JsonProperty("recipient_name")]
        public string RecipientName { get; set; }

        [JsonProperty("memo")]
        public string Memo { get; set; }

        [JsonProperty("state")]
        public int State { get; set; }

        [JsonProperty("is_sender_contract")]
        public bool IsSenderContract { get; set; }

        [JsonProperty("is_recipient_contract")]
        public bool IsRecipientContract { get; set; }

        [JsonProperty("data")]
        public object Data { get; set; }

        [JsonProperty("created_at_block")]
        public float CreatedAtBlock { get; set; }

        [JsonProperty("created_at_time")]
        public float CreatedAtTime { get; set; }

        [JsonProperty("updated_at_block")]
        public float UpdatedAtBlock { get; set; }

        [JsonProperty("updated_at_time")]
        public float UpdatedAtTime { get; set; }

        [JsonProperty("sender_assets")]
        public AssetData[] SenderAssets { get; set; }

        [JsonProperty("recipient_assets")]
        public AssetData[] RecipientAssets { get; set; }

    }

}