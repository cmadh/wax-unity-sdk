using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace StrattonStudios.EosioUnity.Signing
{

    /// <summary>
    /// Link create model.
    /// </summary>
    public class LinkCreate : IJsonModel
    {

        public const string SESSION_NAME = "session_name";
        public const string REQUEST_KEY = "request_key";

        private string sessionName;
        private string requestKey;

        public LinkCreate(string sessionName, string requestKey)
        {
            this.sessionName = sessionName;
            this.requestKey = requestKey;
        }

        public LinkCreate(JObject jsonObject)
        {
            this.sessionName = (string)jsonObject[SESSION_NAME];
            this.requestKey = (string)jsonObject[REQUEST_KEY];
        }

        public string GetSessionName()
        {
            return this.sessionName;
        }

        public string GetRequestKey()
        {
            return this.requestKey;
        }

        public Dictionary<string, string> ToDictionary()
        {
            if (string.IsNullOrEmpty(this.sessionName) || string.IsNullOrEmpty(this.requestKey))
                return null;

            Dictionary<string, string> map = new Dictionary<string, string>();
            map.Add(SESSION_NAME, string.IsNullOrEmpty(this.sessionName) ? null : this.sessionName);
            map.Add(REQUEST_KEY, string.IsNullOrEmpty(this.requestKey) ? null : this.requestKey);
            return map;
        }

        public string ToJSON()
        {
            return JsonConvert.SerializeObject(ToDictionary());
        }

        public override string ToString()
        {
            return ToJSON();
        }

    }

}