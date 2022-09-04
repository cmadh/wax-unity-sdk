using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using StrattonStudios.EosioUnity.Models;

namespace StrattonStudios.EosioUnity
{

    /// <summary>
    /// Permission level model.
    /// </summary>
    public class PermissionLevel : IJsonModel
    {

        public const string ActorPropertyName = "actor";
        public const string PermissionPropertyName = "permission";

        private AccountName actor;
        private PermissionName permission;

        public AccountName Actor
        {
            get
            {
                return this.actor;
            }
            set
            {
                this.actor = value;
            }
        }

        public PermissionName Permission
        {
            get
            {
                return this.permission;
            }
            set
            {
                this.permission = value;
            }
        }

        public PermissionLevel()
        {

        }

        public PermissionLevel(AccountName actor, PermissionName permission)
        {
            this.actor = actor;
            this.permission = permission;
        }

        public PermissionLevel(string actor, string permission)
        {
            this.actor = new AccountName(actor);
            this.permission = new PermissionName(permission);
        }

        public PermissionLevel(JObject obj)
        {
            if (obj.ContainsKey(PermissionPropertyName))
            {
                var value = obj[PermissionPropertyName];
                if (value.Type == JTokenType.Object)
                {
                    obj = value as JObject;
                }
                else if (value.Type == JTokenType.Null)
                {
                    return;
                }
            }

            this.actor = new AccountName((string)obj[ActorPropertyName]);
            this.permission = new PermissionName((string)obj[PermissionPropertyName]);
        }

        public PermissionLevel(Dictionary<string, object> dictionary)
        {
            if (dictionary.ContainsKey(PermissionPropertyName))
            {
                var value = dictionary[PermissionPropertyName];
                if (value is Dictionary<string, object>)
                {
                    dictionary = value as Dictionary<string, object>;
                }
                else if (value == null)
                {
                    return;
                }
            }

            this.actor = new AccountName((string)dictionary[ActorPropertyName]);
            this.permission = new PermissionName((string)dictionary[PermissionPropertyName]);
        }

        public PermissionLevel(Dictionary<string, string> dictionary)
        {
            this.actor = new AccountName(dictionary[ActorPropertyName]);
            this.permission = new PermissionName(dictionary[PermissionPropertyName]);
        }

        public Dictionary<string, string> ToDictionary()
        {
            if (this.actor == null || this.permission == null)
                return null;

            Dictionary<string, string> map = new Dictionary<string, string>();
            map.Add(ActorPropertyName, this.actor == null ? null : this.actor.Value);
            map.Add(PermissionPropertyName, this.permission == null ? null : this.permission.Value);
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

        public static implicit operator SerializablePermissionLevel(PermissionLevel auth)
        {
            var ser = new SerializablePermissionLevel();
            ser.permission = auth.Permission.Value;
            ser.actor = auth.Actor.Value;
            return ser;
        }

        public static implicit operator PermissionLevel(SerializablePermissionLevel ser)
        {
            return new PermissionLevel(ser.actor, ser.permission);
        }

    }

}