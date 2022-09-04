using System.Collections;
using System.Collections.Generic;

using Newtonsoft.Json;

using StrattonStudios.EosioUnity.Signing;

using UnityEngine;

namespace StrattonStudios.EosioUnity
{

    /// <summary>
    /// Identity model.
    /// </summary>
    public class IdentityV2 : IESRRequest
    {

        public const string IDENTITY = "identity";
        public const string VARIANT_TYPE = IDENTITY;
        private PermissionLevel permissionLevel;

        public PermissionLevel PermissionLevel
        {
            get
            {
                return this.permissionLevel;
            }
        }

        public IdentityV2()
        {
            this.permissionLevel = new PermissionLevel();
        }

        public IdentityV2(PermissionLevel permissionLevel)
        {
            this.permissionLevel = permissionLevel;
        }

        public List<Action> GetRawActions()
        {
            return null;
        }

        public List<object> ToVariant()
        {
            var variant = new List<object>();
            variant.Add(VARIANT_TYPE);
            variant.Add(ToDictionary());
            return variant;
        }

        public Dictionary<string, object> ToDictionary()
        {
            var result = new Dictionary<string, object>();
            if (this.permissionLevel == null)
            {
                result.Add(PermissionLevel.PermissionPropertyName, null);
            }
            else
            {
                result.Add(PermissionLevel.PermissionPropertyName, this.permissionLevel.ToDictionary());
            }
            return result;
        }

        public string ToJSON()
        {
            return JsonConvert.SerializeObject(ToDictionary());
        }

    }

}