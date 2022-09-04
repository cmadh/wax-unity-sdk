using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace StrattonStudios.AnchorLinkUnity
{


    [System.Serializable]
    public class IdentityException : LinkException
    {

        public IdentityException(string reason) : base(LinkErrorCode.E_IDENTITY, $"Unable to verify identity: {(!string.IsNullOrEmpty(reason) ? "(" + reason + ")" : "")}") { }

    }

}