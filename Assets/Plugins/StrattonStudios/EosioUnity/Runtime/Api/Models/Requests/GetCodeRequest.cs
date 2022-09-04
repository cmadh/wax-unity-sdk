using System;

namespace StrattonStudios.EosioUnity.Models
{

    [Serializable]
    public class GetCodeRequest
    {
        public string account_name;
        public bool code_as_wasm;
    }

}