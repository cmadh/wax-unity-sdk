using System.Collections;
using System.Collections.Generic;

using Cysharp.Threading.Tasks;

using UnityEngine;

namespace StrattonStudios.WcwUnity
{

    /// <summary>
    /// WAX Event Source for logging in and signing transactions.
    /// </summary>
    /// <remarks>
    /// This interface is implemented by embedded browsers in Unity.
    /// </remarks>
    public interface IWaxEventSource
    {

        /// <summary>
        /// Login through the provided URL.
        /// </summary>
        /// <param name="url">The URL to use for logging in</param>
        /// <returns>Returns the login response from the URL</returns>
        UniTask<LoginResponse> Login(string url);

        /// <summary>
        /// Sign the transaction through the provided URL.
        /// </summary>
        /// <param name="url">The URL to use for signing the transaction</param>
        /// <param name="serializedTransaction">The transaction to sign</param>
        /// <param name="noModify"></param>
        /// <returns>Returns the signing response</returns>
        UniTask<SigningResponse> Sign(string url, byte[] serializedTransaction, bool noModify);

        void PrepareSign(string url);

    }

}