using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using StrattonStudios.EosioUnity.Signing;

namespace StrattonStudios.EosioUnity
{

    /// <summary>
    /// Actions model.
    /// </summary>
    public class Actions : IESRRequest
    {

        public const string VARIANT_TYPE = "action[]";
        private List<Action> actions = new List<Action>();

        public Actions(JArray actions)
        {
            foreach (JToken el in actions)
            {
                if (!(el is JObject))
                {
                    throw new EosioException(VARIANT_TYPE + " should be an array of objects");
                }

                this.actions.Add(new Action((JObject)el));
            }
        }

        public Actions(List<object> actions)
        {
            foreach (var item in actions)
            {
                if (!(item is Dictionary<string, object>))
                {
                    throw new EosioException(VARIANT_TYPE + " should be an array of objects");
                }

                var dictionary = (Dictionary<string, object>)item;
                this.actions.Add(new Action(dictionary));
            }
        }

        public Actions()
        {

        }

        public Actions(List<Action> actions)
        {
            this.actions = actions;
        }

        public void AddAction(Action action)
        {
            this.actions.Add(action);
        }

        public List<Action> GetActions()
        {
            return this.actions;
        }

        public List<Action> GetRawActions()
        {
            return GetActions();
        }

        public List<object> ToVariant()
        {
            List<object> variant = new List<object>();
            variant.Add(VARIANT_TYPE);
            List<Dictionary<string, object>> actionMaps = new List<Dictionary<string, object>>();
            foreach (Action action in GetActions())
            {
                actionMaps.Add(action.ToDictionary());
            }

            variant.Add(actionMaps);
            return variant;
        }

        public string ToJSON()
        {
            return JsonConvert.SerializeObject(ToVariant());
        }

    }

}