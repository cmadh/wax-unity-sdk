using System.Collections;
using System.Collections.Generic;

using StrattonStudios.EosioUnity;

using UnityEngine;

namespace StrattonStudios.AnchorLinkUnity
{

    /// <summary>
    /// Options for the <see cref="Link.Transact"/> method.
    /// </summary>
    public class TransactOptions
    {

        /// <summary>
        /// Whether to broadcast the transaction or just return the signature.
        /// </summary>
        public bool? broadcast = true;

        /// <summary>
        /// Chain to use when configured with multiple chains.
        /// </summary>
        //public Chain chain;

        /// <summary>
        /// Whether the signer can make modifications to the request
        /// (e.g. applying a cosigner action to pay for resources).
        /// </summary>
        public bool? noModify = false;

    }

}