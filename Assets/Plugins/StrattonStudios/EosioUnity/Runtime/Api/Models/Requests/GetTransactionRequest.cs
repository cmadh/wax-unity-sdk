using System;

namespace StrattonStudios.EosioUnity.Models
{

    /// <summary>
    /// <see href="https://developers.eos.io/manuals/eos/v2.1/cleos/command-reference/get/transaction"/>
    /// </summary>
    [Serializable]
    public class GetTransactionRequest
    {

        /// <summary>
        /// ID of the transaction to retrieve
        /// </summary>
        public string id;

        /// <summary>
        /// The block number this transaction may be in
        /// </summary>
        public uint? block_num_hint;
    }

}