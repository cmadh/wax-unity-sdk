using System.Collections;
using System.Collections.Generic;

using StrattonStudios.EosioUnity;

using UnityEngine;

namespace StrattonStudios.AnchorLinkUnity
{

    /// <summary>
    /// Payload accepted by the <see cref="Link.Transact"/> method.
    /// Note that one of <see cref="TransactArgs.action"/>, <see cref="TransactArgs.actions"/> or <see cref="TransactArgs.transaction"/> must be set.
    /// </summary>
    public class TransactArgs
    {

        /// <summary>
        /// Full transaction to sign.
        /// </summary>
        public Transaction transaction;

        /// <summary>
        /// Action to sign.
        /// </summary>
        public Action action;

        /// <summary>
        /// Actions to sign.
        /// </summary>
        public Actions actions;

    }

}