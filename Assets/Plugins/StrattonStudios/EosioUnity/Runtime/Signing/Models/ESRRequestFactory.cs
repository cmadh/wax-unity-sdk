using System.Collections;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using UnityEngine;

namespace StrattonStudios.EosioUnity.Signing
{


    /// <summary>
    /// EOSIO Signing Request factory.
    /// </summary>
    public class ESRRequestFactory
    {

        public static IESRRequest FromVariant(JArray variant)
        {
            string variantType = (string)variant[0];
            switch (variantType)
            {
                case Action.VARIANT_TYPE:
                    return new Action((JObject)variant[1]);
                case Actions.VARIANT_TYPE:
                    return new Actions((JArray)variant[1]);
                case Transaction.VARIANT_TYPE:
                    return new Transaction((JObject)variant[1]);
                case IdentityV2.VARIANT_TYPE:
                    return new IdentityV2(new PermissionLevel((JObject)variant[1]));
                default:
                    throw new EosioException("Unknown request variant type: " + variantType);
            }
        }

        public static IESRRequest FromVariant(List<object> variant)
        {
            string variantType = (string)variant[0];
            switch (variantType)
            {
                case Action.VARIANT_TYPE:
                    return new Action((Dictionary<string, object>)variant[1]);
                case Actions.VARIANT_TYPE:
                    return new Actions((List<object>)variant[1]);
                case Transaction.VARIANT_TYPE:
                    return new Transaction((Dictionary<string, object>)variant[1]);
                case IdentityV2.VARIANT_TYPE:
                    return new IdentityV2(new PermissionLevel((Dictionary<string, object>)variant[1]));
                default:
                    throw new EosioException("Unknown request variant type: " + variantType);
            }
        }

    }

}