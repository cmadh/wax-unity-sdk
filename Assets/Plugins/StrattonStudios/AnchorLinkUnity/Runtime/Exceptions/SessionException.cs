using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace StrattonStudios.AnchorLinkUnity
{


    [System.Serializable]
    public class SessionException : LinkException
    {

        public readonly LinkSession Session;

        public virtual bool SkipToManual { get; set; }

        public SessionException(string reason, LinkErrorCode code, LinkSession session) : base(code, reason)
        {
            this.Session = session;
        }

    }

}