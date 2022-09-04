using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace StrattonStudios.AnchorLinkUnity
{


    [System.Serializable]
    public class CancelException : LinkException
    {

        public CancelException(string reason) : base(LinkErrorCode.E_CANCEL, $"User canceled request: {(!string.IsNullOrEmpty(reason) ? "(" + reason + ")" : "")}") { }

    }

}