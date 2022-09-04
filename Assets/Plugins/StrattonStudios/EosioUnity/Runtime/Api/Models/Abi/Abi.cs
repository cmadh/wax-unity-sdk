using System;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace StrattonStudios.EosioUnity.Models
{

    [Serializable]
    public class Abi
    {

        [JsonProperty("version")]
        public string Version;

        [JsonProperty("types")]
        public List<AbiType> Types;

        [JsonProperty("structs")]
        public List<AbiStruct> Structs;

        [JsonProperty("actions")]
        public List<AbiAction> Actions;

        [JsonProperty("tables")]
        public List<AbiTable> Tables;

        [JsonProperty("ricardian_clauses")]
        public List<AbiRicardianClause> RicardianClauses;

        [JsonProperty("error_messages")]
        public List<string> ErrorMessages;

        [JsonProperty("abi_extensions")]
        public List<SerializableExtension> AbiExtensions;

        [JsonProperty("variants")]
        public List<Variant> Variants;
    }

}