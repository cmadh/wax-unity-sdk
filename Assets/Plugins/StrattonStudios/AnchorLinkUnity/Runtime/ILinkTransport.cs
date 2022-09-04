using System;
using System.Collections;
using System.Collections.Generic;

using Cysharp.Threading.Tasks;

using StrattonStudios.EosioUnity.Signing;

using UnityEngine;

namespace StrattonStudios.AnchorLinkUnity
{

    public delegate void LinkCancelHandler(Exception reason);

    /// <summary>
    /// Protocol link transports need to implement.
    /// </summary>
    /// <remarks>
    /// A transport is responsible for getting the request to the
    /// user, e.g. by opening request URIs or displaying QR codes.
    /// </remarks>
    public interface ILinkTransport
    {

        /// <summary>
        /// Can be implemented if transport provides a storage as well.
        /// </summary>
        ILinkStorage Storage { get; }

        /// <summary>
        /// User agent reported to the signer.
        /// </summary>
        string UserAgent { get; }

        /// <summary>
        /// Present a signing request to the user.
        /// </summary>
        /// <param name="request">The signing request.</param>
        /// <param name="cancel">Can be called to abort the request.</param>
        void OnRequest(SigningRequest request, LinkCancelHandler cancel);

        /// <summary>
        /// Called if the request was successful.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="result"></param>
        void OnSuccess(SigningRequest request, TransactResult result);

        /// <summary>
        /// Called if the request failed.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="error"></param>
        void OnFailure(SigningRequest request, Exception error);

        /// <summary>
        /// Called when a session request is initiated.
        /// </summary>
        /// <param name="session">Session where the request originated.</param>
        /// <param name="request">Signing request that will be sent over the session.</param>
        /// <param name="cancel"></param>
        void OnSessionRequest(LinkSession session, SigningRequest request, LinkCancelHandler cancel);

        /// <summary>
        /// Can be implemented to modify request just after it has been created.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="session"></param>
        UniTask<SigningRequest> Prepare(SigningRequest request, LinkSession session);

        /// <summary>
        /// Called immediately when the transaction starts
        /// </summary>
        void ShowLoading();

        /// <summary>
        /// Send session request payload, optional. Can return false to indicate it has to be sent over the socket.
        /// </summary>
        /// <param name="payload"></param>
        /// <param name="session"></param>
        /// <returns></returns>
        bool SendSessionPayload(byte[] payload, LinkSession session);

        /// <summary>
        /// Can be implemented to recover from certain errors, if the function returns true the error will
        /// not bubble up to the caller of <see cref="Link.Transact"/> or <see cref="Link.Login"/> and the link will continue waiting for the callback.
        /// </summary>
        /// <param name="error"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        bool RecoverError(Exception error, SigningRequest request);

    }

}