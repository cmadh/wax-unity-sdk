using System.Collections;
using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using StrattonStudios.EosioUnity.Utilities;

using UnityEngine;

namespace StrattonStudios.EosioUnity.Signing
{

    /// <summary>
    /// Info pair model.
    /// </summary>
    public class InfoPair : IJsonModel
    {

        private const string KEY = "key";
        private const string VALUE = "value";

        private string key;
        private string hexValue;

        public static InfoPair Create(string key, byte[] value)
        {
            //return new InfoPair(key, SerializationHelper.ObjectToHexString(value));
            return new InfoPair(key, HexUtility.ToHexString(value));
        }

        public InfoPair(string key, string value)
        {
            this.key = key;
            this.hexValue = value;
        }

        public string GetKey()
        {
            return this.key;
        }

        public void SetHexValue(string hexValue)
        {
            this.hexValue = hexValue;
        }

        public string GetHexValue()
        {
            return this.hexValue;
        }

        public string GetStringValue()
        {
            return System.Text.Encoding.UTF8.GetString(GetBytesValue());
        }

        public byte[] GetBytesValue()
        {
            // Could use: SerializationHelper.HexStringToByteArray(this.gHexValue);
            return HexUtility.FromHexString(this.hexValue);
            //return EncodingHelper.StringToByteArray(this.hexValue);
        }

        public static InfoPair FromDeserializedJsonObject(JObject obj)
        {
            string key = (string)obj[KEY];
            JToken value = obj[VALUE];
            if (value.Type == JTokenType.String)
                return new InfoPair(key, (string)value);

            throw new EosioException("InfoPair value should always be a hex string when deserializing");
        }

        public static InfoPair FromDeserialized(IDictionary<string, object> obj)
        {
            string key = (string)obj[KEY];
            var value = obj[VALUE];
            if (value is byte[])
            {
                return new InfoPair(key, HexUtility.ToHexString((byte[])value));
            }
            if (value is string)
            {
                return new InfoPair(key, (string)value);
            }

            throw new EosioException("InfoPair value should always be a hex string when deserializing");
        }

        public static List<InfoPair> ListFromDeserializedJsonArray(JArray pairs)
        {
            List<InfoPair> infoPairs = new List<InfoPair>();

            foreach (JToken el in pairs)
            {
                if (!(el is JObject))
                    throw new EosioException("Info pairs must be objects");

                infoPairs.Add(InfoPair.FromDeserializedJsonObject((JObject)el));
            }

            return infoPairs;
        }

        public static List<InfoPair> ListFromDeserialized(List<object> pairs)
        {
            List<InfoPair> infoPairs = new List<InfoPair>();

            foreach (var el in pairs)
            {
                if (!(el is IDictionary))
                    throw new EosioException("Info pairs must be objects");

                infoPairs.Add(FromDeserialized((IDictionary<string, object>)el));
            }

            return infoPairs;
        }

        public Dictionary<string, object> ToDictionary()
        {
            Dictionary<string, object> pair = new Dictionary<string, object>();
            pair.Add(KEY, this.key);
            pair.Add(VALUE, GetBytesValue());
            return pair;
        }

        public string ToJSON()
        {
            return JsonConvert.SerializeObject(ToDictionary());
        }

    }

}