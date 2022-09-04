using System.Collections;
using System.Collections.Generic;

using StrattonStudios.EosioUnity.Api;
using StrattonStudios.EosioUnity.Signing;

using UnityEngine;

namespace StrattonStudios.AnchorLinkUnity
{

    /// <summary>
    /// The result of a <see cref="Link.Login(string)"/> call.
    /// </summary>
    public class LoginResult
    {

        /// <summary>
        /// The underlying identify result.
        /// </summary>
        public IdentifyResult identifyResult;

        /// <summary>
        /// he session created by the login.
        /// </summary>
        public LinkSession session;

    }

}