using System;

namespace StrattonStudios.EosioUnity.Models
{

    [Serializable]
    public class GetRawAbiResponse
    {

        public string account_name;

        public string code_hash;

        public string abi_hash;

        public string abi;
    }

}