using System;

namespace StrattonStudios.EosioUnity.Models
{

    /// <summary>
    /// <see href="https://developers.eos.io/manuals/eos/latest/nodeos/plugins/chain_api_plugin/api-reference/index#operation/get_scheduled_transaction"/>
    /// </summary>
    [Serializable]
    public class GetScheduledTransactionsRequest
    {

        /// <summary>
        /// true/false whether the packed transaction is converted to JSON
        /// </summary>
        public bool json = false;

        /// <summary>
        /// Date/time string in the format YYYY-MM-DDTHH:MM:SS.sss
        /// </summary>
        public string lower_bound;

        /// <summary>
        /// The maximum number of transactions to return
        /// </summary>
        public int limit = 50;
    }

}