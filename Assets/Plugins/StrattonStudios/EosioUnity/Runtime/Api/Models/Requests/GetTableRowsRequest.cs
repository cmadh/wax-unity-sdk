using System;

namespace StrattonStudios.EosioUnity.Models
{

    /// <summary>
    /// <see href="https://developers.eos.io/manuals/eos/latest/nodeos/plugins/chain_api_plugin/api-reference/index#operation/get_table_rows"/>
    /// </summary>
    [Serializable]
    public class GetTableRowsRequest
    {

        /// <summary>
        /// Retrieve the rows using JSON or raw format.
        /// </summary>
        public bool json = false;

        /// <summary>
        /// The name of the smart contract that controls the provided table. (Required)
        /// </summary>
        public string code;

        /// <summary>
        /// The account to which this data belongs. (Required)
        /// </summary>
        public string scope;

        /// <summary>
        /// The name of the table to query. (Required)
        /// </summary>
        public string table;

        /// <summary>
        /// Filters results to return the first element that is not less than provided value in set
        /// </summary>
        public string lower_bound = "0";

        /// <summary>
        /// Filters results to return the first element that is greater than provided value in set
        /// </summary>
        public string upper_bound = "-1";

        /// <summary>
        /// Limit number of results returned.
        /// </summary>
        public int limit = 10;

        /// <summary>
        /// Type of key specified by <see cref="index_position"/> (for example - uint64_t or name)
        /// </summary>
        public string key_type;

        /// <summary>
        /// Position of the index used, accepted parameters primary, secondary, tertiary, fourth, fifth, sixth, seventh, eighth, ninth , tenth
        /// </summary>
        public string index_position;

        /// <summary>
        /// The encoding type of <see cref="key_type"/>; <c>dec</c> for decimal encoding of (i[64|128|256], float[64|128]); <c>hex</c> for hexadecimal encoding of (i256, ripemd160, sha256)
        /// </summary>
        public string encode_type = "dec";

        /// <summary>
        /// Reverse the order of returned results
        /// </summary>
        public bool reverse;

        /// <summary>
        /// Show RAM payer
        /// </summary>
        public bool show_payer;

        // TODO: implement binary retrieval of table row

        /// <summary>
        /// Return the value as BINARY rather than using abi to interpret as JSON
        /// </summary>
        //public uint binary;

    }

}