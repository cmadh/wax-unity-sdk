using System;

namespace StrattonStudios.EosioUnity.Models
{

    [Serializable]
    public class GetCodeResponse
    {

        public string account_name;

        public string wast;

        public string wasm;

        public string code_hash;

        public Abi abi;
    }

}