using System.Collections;
using System.Collections.Generic;

using Newtonsoft.Json;

using UnityEngine;

namespace StrattonStudios.EosioUnity
{

    /// <summary>
    /// Name base class.
    /// </summary>
    public class Name : IJsonModel
    {

        private string value;

        public string Value
        {
            get
            {
                return this.value;
            }
            set
            {
                this.value = value;
            }
        }

        public Name(string value)
        {
            this.value = value;
        }

        public override string ToString()
        {
            return this.value;
        }

        public string ToJSON()
        {
            return JsonConvert.SerializeObject(this.value);
        }

    }

}