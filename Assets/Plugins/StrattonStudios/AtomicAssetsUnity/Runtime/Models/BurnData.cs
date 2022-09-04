using Newtonsoft.Json;

namespace StrattonStudios.AtomicAssetsUnity.Models
{

    public class BurnData
    {

        [JsonProperty("account")]
        public string Account { get; set; }

        [JsonProperty("collections")]
        public CollectionData[] Collections { get; set; }

        [JsonProperty("templates")]
        public TemplateData[] Templates { get; set; }

        [JsonProperty("assets")]
        public string Assets { get; set; }

    }

}