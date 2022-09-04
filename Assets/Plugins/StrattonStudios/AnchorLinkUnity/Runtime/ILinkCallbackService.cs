using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace StrattonStudios.AnchorLinkUnity
{

    /// <summary>
    /// Service that handles waiting for a ESR callback to be sent to an url.
    /// </summary>
    public interface ILinkCallbackService
    {

        ILinkCallback Create();

    }

}