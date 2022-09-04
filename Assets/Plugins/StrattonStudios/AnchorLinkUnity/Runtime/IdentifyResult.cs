using System.Collections;
using System.Collections.Generic;

using StrattonStudios.EosioUnity.Models;
using StrattonStudios.EosioUnity.Signing;

using UnityEngine;

namespace StrattonStudios.AnchorLinkUnity
{

    /// <summary>
    /// The result of a <see cref="Link.Identify(string, EosUnity.PermissionLevel, List{InfoPair})"/> call.
    /// </summary>
    public class IdentifyResult
    {

        /// <summary>
        /// The underlying transaction result.
        /// </summary>
        public TransactResult transactResult;

        /// <summary>
        /// The identified account, not present unless <see cref="LinkOptions.VerifyProofs"/> is set to true.
        /// </summary>
        public GetAccountResponse account;

        /// <summary>
        /// The identity proof.
        /// </summary>
        public IdentityProof proof;

    }

}