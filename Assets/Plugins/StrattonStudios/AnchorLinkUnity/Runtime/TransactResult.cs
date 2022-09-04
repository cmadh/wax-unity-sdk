using System.Collections;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using StrattonStudios.EosioUnity;
using StrattonStudios.EosioUnity.Models;
using StrattonStudios.EosioUnity.Signing;

using UnityEngine;

namespace StrattonStudios.AnchorLinkUnity
{

    /// <summary>
    /// The result of a <see cref="Link.Transact"/> call.
    /// </summary>
    public class TransactResult
    {

        /// <summary>
        /// The resolved signing request.
        /// </summary>
        public ResolvedSigningRequest resolved;

        /// <summary>
        /// The chain that was used.
        /// </summary>
        public LinkChain chain;

        /// <summary>
        /// The transaction signatures.
        /// </summary>
        public List<string> signatures;

        /// <summary>
        /// The callback payload.
        /// </summary>
        public JObject payload;

        /// <summary>
        /// The signer authority.
        /// </summary>
        public PermissionLevel signer;

        /// <summary>
        /// The resulting transaction.
        /// </summary>
        public Transaction transaction;

        // TODO: For MakeSignatureProvider
        public byte[] serializedTransaction;

        /// <summary>
        /// Resolved version of transaction, with the action data decoded.
        /// </summary>
        public Transaction resolvedTransaction;

        /// <summary>
        /// Push transaction response from api node, only present if transaction was broadcast.
        /// </summary>
        public PushTransactionResponse processed;

    }

}