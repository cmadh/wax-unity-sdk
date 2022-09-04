using System.Collections;
using System.Collections.Generic;

using StrattonStudios.EosioUnity.Signing;

using Newtonsoft.Json;

using UnityEngine;

namespace StrattonStudios.EosioUnity
{

    /// <summary>
    /// Identity model.
    /// </summary>
    public class IdentityV3 : IESRRequest
    {

        public const string IDENTITY = "identity";
        public const string VARIANT_TYPE = IDENTITY;

        private string scope;
        private PermissionLevel permissionLevel;

        public string Scope
        {
            get
            {
                return this.scope;
            }
        }

        public PermissionLevel PermissionLevel
        {
            get
            {
                return this.permissionLevel;
            }
        }

        public IdentityV3()
        {
            this.scope = null;
            this.permissionLevel = new PermissionLevel();
        }

        public IdentityV3(string scope, PermissionLevel permissionLevel)
        {
            this.scope = scope;
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

        private Dictionary<string, object> ToDictionary()
        {
            var result = new Dictionary<string, object>();
            var permissionLevel = this.permissionLevel.ToDictionary();
            result.Add(PermissionLevel.PermissionPropertyName, permissionLevel);
            return result;
        }

        public string ToJSON()
        {
            return JsonConvert.SerializeObject(ToDictionary());
        }

    }

}