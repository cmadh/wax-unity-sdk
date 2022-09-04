using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace StrattonStudios.AnchorLinkUnity
{


    [System.Serializable]
    public class LinkException : System.Exception
    {

        public readonly LinkErrorCode Code;

        public LinkException(LinkErrorCode code, string message) : base(message)
        {
            this.Code = code;
        }

    }

}