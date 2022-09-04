using Newtonsoft.Json;

namespace StrattonStudios.AtomicAssetsUnity.Models
{

    public class AccountData
    {

        [JsonProperty("account")]
        public string Account { get; set; }

        [JsonProperty("collections")]
        public CollectionsDataWrapper[] Collections { get; set; }

        [JsonProperty("templates")]
        public TemplateData[] Templates { get; set; }

        [JsonProperty("schemas")]
        public SchemaData[] Schemas { get; set; }

        [JsonProperty("assets")]
        public string Assets { get; set; }

        public class CollectionsDataWrapper
        {
            [JsonProperty("collection")]
            public CollectionData Collection { get; set; }

            [JsonProperty("assets")]
            public string Assets { get; set; }
        }

    }

}