using System;

namespace StrattonStudios.EosioUnity.Models
{

    [Serializable]
    public class GetRawAbiRequest
    {
        public string account_name;
        public string abi_hash;
    }

}