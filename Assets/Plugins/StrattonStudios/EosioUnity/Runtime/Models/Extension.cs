using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace StrattonStudios.EosioUnity
{

    /// <summary>
    /// Extension model.
    /// </summary>
    public class Extension : IJsonModel
    {

        private const string TYPE = "type";
        private const string DATA = "data";

        private short type;
        private string data;

        public short Type
        {
            get
            {
                return this.type;
            }
            set
            {
                this.type = value;
            }
        }

        public string Data
        {
            get
            {
                return this.data;
            }
            set
            {
                this.data = value;
            }
        }

        public Extension(short type, string data)
        {
            this.type = type;
            this.data = data;
        }

        public Extension(JObject obj)
        {
            this.type = (short)obj[TYPE];
            this.data = (string)obj[DATA];
        }

        public Extension(Dictionary<string, object> dictionary)
        {
            this.type = (short)dictionary[TYPE];
            this.data = (string)dictionary[DATA];
        }

        public Dictionary<short, string> ToDictionary()
        {
            Dictionary<short, string> extension = new Dictionary<short, string>();
            extension.Add(this.type, this.data);
            return extension;
        }

        public string ToJSON()
        {
            return JsonConvert.SerializeObject(ToDictionary());
        }

    }

}