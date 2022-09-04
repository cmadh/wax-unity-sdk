using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace StrattonStudios.EosioUnity.Signing
{

    /// <summary>
    /// The callback payload sent to background callbacks.
    /// </summary>
    public class CallbackPayload
    {

        /// <summary>
        /// The first signature.
        /// </summary>
        public string sig;

        /// <summary>
        /// Transaction ID as HEX-encoded string.
        /// </summary>
        public string tx;

        /// <summary>
        /// Block number hint (only present if transaction was broadcast).
        /// </summary>
        public string bn;

        /// <summary>
        /// Signer authority, aka account name.
        /// </summary>
        public string sa;

        /// <summary>
        /// Signer permission, e.g. "active".
        /// </summary>
        public string sp;

        /// <summary>
        /// Reference block num used when resolving request.
        /// </summary>
        public string rbn;

        /// <summary>
        /// Reference block id used when resolving request.
        /// </summary>
        public string rid;

        /// <summary>
        /// The originating signing request packed as a uri string.
        /// </summary>
        public string req;

        /// <summary>
        /// Expiration time used when resolving request.
        /// </summary>
        public string ex;

        /// <summary>
        /// The resolved chain id. 
        /// </summary>
        public string cid;

        /// <summary>
        /// The link session metadata.
        /// </summary>
        public string link_meta;

        /// <summary>
        /// All signatures 0-indexed as `sig0`, `sig1`, etc.
        /// </summary>
        public Dictionary<string, string> sigs;

        /// <summary>
        /// [sig0: string]: string | undefined
        /// </summary>
        //public Dictionary<string, string> data;

    }

}