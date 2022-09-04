using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace StrattonStudios.EosioUnity.Signing
{

    public class SigningRequestCreateIdentityArgs : SigningRequestCommonArgs
    {

        /// <summary>
        /// Callback where the identity should be delivered.
        /// </summary>
        public LinkCallback callback;

        /// <summary>
        /// Requested account name of identity.
        /// </summary>
        /// <remarks>
        /// Defaults to placeholder (any identity) if omitted.
        /// </remarks>
        public AccountName account;

        /// <summary>
        /// Requested account permission.
        /// </summary>
        /// <remarks>
        /// Defaults to placeholder (any permission) if omitted.
        /// </remarks>
        public PermissionName permission;

        /// <summary>
        /// Scope for the request.
        /// </summary>
        //public string scope;

    }

}