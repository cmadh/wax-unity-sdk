using System.Collections;
using System.Collections.Generic;

using StrattonStudios.EosioUnity.Models;

namespace StrattonStudios.EosioUnity
{

    public static class AbiData
    {

        public const string AbiJsonResourcePath = "StrattonStudios/EosioUnity/abi";

        private static Abi cachedAbi;

        public static Abi GetCachedAbi()
        {
            if (cachedAbi == null)
            {
                cachedAbi = Newtonsoft.Json.JsonConvert.DeserializeObject<Abi>(UnityEngine.Resources.Load<UnityEngine.TextAsset>(AbiJsonResourcePath).text);
            }
            return cachedAbi;
        }

    }

}