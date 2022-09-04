using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace StrattonStudios.EosioUnity
{

    /// <summary>
    /// Action data model.
    /// </summary>
    public class ActionData : IJsonModel
    {

        private Dictionary<string, object> data;
        private string packedData;

        public ActionData(string data)
        {
            this.packedData = data;
        }

        public ActionData(Dictionary<string, object> data)
        {
            this.data = data;
        }

        public ActionData(JObject data)
        {
            this.data = data.ToObject<Dictionary<string, object>>();
        }

        public bool IsPacked()
        {
            return this.packedData != null;
        }

        public void SetData(Dictionary<string, object> data)
        {
            this.data = data;
            this.packedData = null;
        }

        public void SetData(string packedData)
        {
            this.packedData = packedData;
            this.data = null;
        }

        public Dictionary<string, object> GetData()
        {
            return this.data;
        }

        public string GetPackedData()
        {
            return this.packedData;
        }

        public string ToJSON()
        {
            if (IsPacked())
                throw new EosioException("Cannot toJSON packed action data");

            return JsonConvert.SerializeObject(this.data);
        }

        public override string ToString()
        {
            try
            {
                return IsPacked() ? GetPackedData() : ToJSON();
            }
            catch (EosioException e)
            {
                return "Failed to toJSON - " + e.Message;
            }
        }

    }

}