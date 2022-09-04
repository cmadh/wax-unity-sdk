using System.Collections;
using System.Collections.Generic;

using Cysharp.Threading.Tasks;

using UnityEngine;

namespace StrattonStudios.AnchorLinkUnity
{

    /// <summary>
    /// Callback that can be waited for.
    /// </summary>
    public interface ILinkCallback
    {

        /// <summary>
        /// Url that should be hit to trigger the callback.
        /// </summary>
        string Url { get; }

        /// <summary>
        /// Wait for the callback to resolve.
        /// </summary>
        UniTask<string> Wait();

        /// <summary>
        /// Cancel a pending callback.
        /// </summary>
        void Cancel();

    }

}