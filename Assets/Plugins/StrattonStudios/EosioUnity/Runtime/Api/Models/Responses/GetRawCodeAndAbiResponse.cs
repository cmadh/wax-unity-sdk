using System;

namespace StrattonStudios.EosioUnity.Models
{

    [Serializable]
    public class GetRawCodeAndAbiResponse
    {

        public string account_name;

        public string wasm;

        public string abi;
    }

}