using System;

namespace StrattonStudios.EosioUnity.Models
{

    /// <summary>
    /// <see href="https://developers.eos.io/manuals/eos/latest/nodeos/plugins/chain_api_plugin/api-reference/index#operation/get_producers"/>
    /// </summary>
    [Serializable]
    public class GetProducersRequest
    {

        /// <summary>
        /// Return result in JSON format.
        /// </summary>
        public bool json = false;

        /// <summary>
        /// In conjunction with <see cref="limit"/> can be used to paginate through the results. For example, limit=10 and lower_bound=10 would be page 2. (Required)
        /// </summary>
        public string lower_bound;

        /// <summary>
        /// Total number of producers to retrieve. (Required)
        /// </summary>
        public int limit = 50;
    }

}