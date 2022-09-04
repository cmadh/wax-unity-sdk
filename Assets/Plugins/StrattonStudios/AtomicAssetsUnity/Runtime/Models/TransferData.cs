using Newtonsoft.Json;

namespace StrattonStudios.AtomicAssetsUnity.Models
{

    public class TransferData
    {

        [JsonProperty("contract")]
        public string Contract { get; set; }

        [JsonProperty("transfer_id")]
        public string TransferId { get; set; }

        [JsonProperty("sender_name")]
        public string SenderName { get; set; }

        [JsonProperty("recipient_name")]
        public string RecipientName { get; set; }

        [JsonProperty("memo")]
        public string Memo { get; set; }

        [JsonProperty("assets")]
        public AssetData[] Assets { get; set; }

        [JsonProperty("created_at_block")]
        public float CreatedAtBlock { get; set; }

        [JsonProperty("created_at_time")]
        public float CreatedAtTime { get; set; }

    }

}