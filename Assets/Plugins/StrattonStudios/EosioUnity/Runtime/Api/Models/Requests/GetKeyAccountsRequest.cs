using System;

namespace StrattonStudios.EosioUnity.Models
{

    /// <summary>
    /// <see href="https://developers.eos.io/manuals/eos/v2.1/cleos/command-reference/get/accounts"/>
    /// </summary>
    [Serializable]
    public class GetKeyAccountsRequest
    {

        /// <summary>
        /// The public key to retrieve accounts for
        /// </summary>
        public string public_key;
    }

}