using System;
using System.Collections.Generic;


namespace StrattonStudios.EosioUnity.Models
{

    [Serializable]
    public class AbiTable
    {
        [EosioAbiFieldType("name")]
        public string name;

        public string index_type;

        public List<string> key_names;

        public List<string> key_types;

        public string type;
    }

}