using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace StrattonStudios.EosioUnity.Signing
{

    public class SigningRequestCreateArgs : SigningRequestCommonArgs
    {

        /// <summary>
        /// Single action to create request with.
        /// </summary>
        public Action action;

        /// <summary>
        /// Multiple actions to create request with.
        /// </summary>
        public Actions actions;

        /// <summary>
        /// Full or partial transaction to create request with.
        /// </summary>
        /// <remarks>
        /// If TAPoS info is omitted it will be filled in when resolving the request.
        /// </remarks>
        public Transaction transaction;

        /// <summary>
        /// Create an identity request.
        /// </summary>
        /// <remarks>
        /// This uses Protocol Version 2
        /// </remarks>
        public IdentityV2 identityV2;

        /// <summary>
        /// Create an identity request.
        /// </summary>
        /// <remarks>
        /// This uses Protocol Version 3
        /// </remarks>
        //public IdentityV3 identityV3;

        /// <summary>
        /// Whether wallet should broadcast tx, defaults to true.
        /// </summary>
        public bool? broadcast;

        /// <summary>
        /// Optional callback URL the signer should hit after broadcasting or signing.
        /// </summary>
        public LinkCallback callback;

    }

}