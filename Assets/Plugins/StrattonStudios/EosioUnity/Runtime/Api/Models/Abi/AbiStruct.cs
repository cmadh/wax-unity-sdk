using System;
using System.Collections.Generic;

namespace StrattonStudios.EosioUnity.Models
{

    [Serializable]
    public class AbiStruct
    {

        public string name;

        public string @base;

        public List<AbiField> fields;
    }

}