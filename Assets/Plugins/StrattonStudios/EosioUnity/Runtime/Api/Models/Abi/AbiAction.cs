using System;

using Newtonsoft.Json;


namespace StrattonStudios.EosioUnity.Models
{

    [Serializable]
    public class AbiAction
    {

        [EosioAbiFieldType("name")]
        [JsonProperty("name")]
        public string Name;

        [JsonProperty("type")]
        public string type;

        [JsonProperty("ricardian_contract")]
        public string ricardian_contract;

    }

}