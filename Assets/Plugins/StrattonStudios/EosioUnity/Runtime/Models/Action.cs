using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using StrattonStudios.EosioUnity.Signing;
using StrattonStudios.EosioUnity.Utilities;

namespace StrattonStudios.EosioUnity
{

    /// <summary>
    /// Action model.
    /// </summary>
    public class Action : IESRRequest
    {

        public const string VARIANT_TYPE = "action";

        private const string ACCOUNT = "account";
        private const string NAME = "name";
        private const string AUTHORIZATION = "authorization";
        private const string DATA = "data";

        private AccountName account;
        private ActionName name;
        private List<PermissionLevel> authorization = new List<PermissionLevel>();
        private ActionData data;

        public AccountName Account
        {
            get
            {
                return this.account;
            }
            set
            {
                this.account = value;
            }
        }

        public ActionName Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
            }
        }

        public List<PermissionLevel> Authorization
        {
            get
            {
                return this.authorization;
            }
            set
            {
                this.authorization = value;
            }
        }

        public ActionData Data
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

        public Action()
        {
        }

        public Action(JObject obj)
        {
            this.account = new AccountName((string)obj[ACCOUNT]);
            this.name = new ActionName((string)obj[NAME]);
            this.authorization = GetPermissionsFromJsonArray((JArray)obj[AUTHORIZATION]);
            if (obj[DATA].Type == JTokenType.Object)
            {
                this.data = new ActionData((JObject)obj[DATA]);
            }
            else
            {
                this.data = new ActionData((string)obj[DATA]);
            }
        }

        public Action(Dictionary<string, object> dictionary)
        {
            this.account = new AccountName((string)dictionary[ACCOUNT]);
            this.name = new ActionName((string)dictionary[NAME]);
            this.authorization = GetPermissionsFromArray((List<object>)dictionary[AUTHORIZATION]);
            if (dictionary[DATA] is Dictionary<string, object>)
            {
                this.data = new ActionData((Dictionary<string, object>)dictionary[DATA]);
            }
            else
            {
                if (dictionary[DATA] is byte[])
                {
                    this.data = new ActionData(HexUtility.ToHexString((byte[])dictionary[DATA]));
                }
                else
                {
                    this.data = new ActionData((string)dictionary[DATA]);
                }
            }
        }

        private List<PermissionLevel> GetPermissionsFromJsonArray(JArray array)
        {
            List<PermissionLevel> permissionLevels = new List<PermissionLevel>();
            foreach (JToken el in array)
            {
                if (!(el is JObject))
                {
                    throw new EosioException("Permission was not an object");
                }

                permissionLevels.Add(new PermissionLevel((JObject)el));
            }
            return permissionLevels;
        }

        private List<PermissionLevel> GetPermissionsFromArray(List<object> array)
        {
            List<PermissionLevel> permissionLevels = new List<PermissionLevel>();
            foreach (var item in array)
            {
                if (!(item is Dictionary<string, string> || item is Dictionary<string, object> || item is PermissionLevel))
                {
                    throw new EosioException("Permission was not an object");
                }
                if (item is PermissionLevel)
                {
                    permissionLevels.Add(item as PermissionLevel);
                }
                else if (item is Dictionary<string, string>)
                {
                    var dictionary = item as Dictionary<string, string>;
                    permissionLevels.Add(new PermissionLevel(dictionary));
                }
                else
                {
                    var dictionary = item as Dictionary<string, object>;
                    permissionLevels.Add(new PermissionLevel(dictionary));
                }
            }
            return permissionLevels;
        }

        public bool IsIdentity()
        {
            return this.account != null && "".Equals(this.account.Value) &&
                    this.name != null && IdentityV2.IDENTITY.Equals(this.name.Value);
        }

        public string GetAuthorizationJSON()
        {
            List<Dictionary<string, string>> toEncode = new List<Dictionary<string, string>>();
            foreach (PermissionLevel level in this.authorization)
            {
                toEncode.Add(level.ToDictionary());
            }

            return JsonConvert.SerializeObject(toEncode);
        }

        public void AddAuthorization(PermissionLevel authorization)
        {
            this.authorization.Add(authorization);
        }

        public Dictionary<string, object> ToDictionary()
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            result.Add(ACCOUNT, this.account.Value);
            result.Add(NAME, this.name.Value);
            List<Dictionary<string, string>> auths = new List<Dictionary<string, string>>();
            foreach (PermissionLevel permissionLevel in this.authorization)
            {
                auths.Add(permissionLevel.ToDictionary());
            }

            result.Add(AUTHORIZATION, auths);
            object dataToEncode;
            if (this.data.IsPacked())
            {
                dataToEncode = this.data.GetPackedData();
            }
            else
            {
                dataToEncode = this.data.GetData();
            }
            result.Add(DATA, dataToEncode);
            return result;
        }

        public List<Action> GetRawActions()
        {
            return new List<Action>() { this };
        }

        public List<object> ToVariant()
        {
            List<object> variant = new List<object>();
            variant.Add(VARIANT_TYPE);
            variant.Add(ToDictionary());
            return variant;
        }

        public string ToJSON()
        {
            return JsonConvert.SerializeObject(ToDictionary());
        }

    }

}