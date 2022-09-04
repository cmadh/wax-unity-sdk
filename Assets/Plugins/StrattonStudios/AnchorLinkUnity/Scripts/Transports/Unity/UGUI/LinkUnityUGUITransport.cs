using System;
using System.Collections;
using System.Collections.Generic;

using Cysharp.Threading.Tasks;

using Newtonsoft.Json.Linq;

using StrattonStudios.EosioUnity;
using StrattonStudios.EosioUnity.Signing;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace StrattonStudios.AnchorLinkUnity.Transports.UGUI
{

    public class LinkUnityUGUITransport : ILinkTransport
    {

        public const string SceneName = "Anchor Link UI";
        public const string CanvasPath = "AnchorLink/AnchorLinkCanvas";

        protected LinkPlayerPrefsStorage storage;
        protected SigningRequest activeRequest;
        protected LinkCancelHandler activeCancel;

        protected bool showRequestStatus = true;
        protected int statusDuration = 2000;

        protected bool showingManual = false;

        public bool ShowRequestStatus
        {
            get
            {
                return this.showRequestStatus;
            }
            set
            {
                this.showRequestStatus = value;
            }
        }

        public int StatusDuration
        {
            get
            {
                return this.statusDuration;
            }
            set
            {
                this.statusDuration = value;
            }
        }

        public ILinkStorage Storage
        {
            get
            {
                if (this.storage == null)
                {
                    this.storage = new LinkPlayerPrefsStorage();
                }
                return this.storage;
            }
        }

        public string UserAgent
        {
            get
            {
                return "UnityTransport/1.0.0";
            }
        }

        public SigningRequest ActiveRequest
        {
            get
            {
                return this.activeRequest;
            }
        }

        public LinkUnityUGUITransport()
        {
            //EnsureUIScene();
        }

        private void EnsureUI()
        {
            if (AnchorLinkUIManager.Instance == null)
            {
                var prefab = Resources.Load(CanvasPath);
                UnityEngine.Object.Instantiate(prefab);
            }
            //var uiScene = SceneManager.GetSceneByName(SceneName);
            //if (!uiScene.isLoaded)
            //{
            //    var loadParams = new LoadSceneParameters(LoadSceneMode.Additive);
            //    SceneManager.LoadScene(SceneName, loadParams);
            //}
        }

        public void OnRequest(SigningRequest request, LinkCancelHandler cancel)
        {
            this.activeRequest = request;
            this.activeCancel = cancel;

            string title = request.IsIdentity() ? "Login" : "Sign";
            string subtitle = "Scan the QR-code with Anchor on another device or use the button to open it here.";

            DisplayRequest(request, title, subtitle);
        }

        public void OnSessionRequest(LinkSession session, SigningRequest request, LinkCancelHandler cancel)
        {
            if (session.Type == "fallback")
            {
                OnRequest(request, cancel);
                return;
            }

            this.activeRequest = request;
            this.activeCancel = cancel;
            int timeout;
            if (session.Metadata.ContainsKey("timeout"))
            {
                timeout = (int)session.Metadata["timeout"];
            }
            else
            {
                timeout = 60 * 1000 * 2;
            }

            float expires = (Time.unscaledTime * 1000) + timeout;

            string subtitle;
            if (session.Metadata.ContainsKey("name") && !string.IsNullOrEmpty((string)session.Metadata["name"]))
            {
                string deviceName = (string)session.Metadata["name"];
                subtitle = $"Please open Anchor Wallet on “{deviceName}” to review and sign the transaction.";
            }
            else
            {
                subtitle = "Please review and sign the transaction in the linked wallet.";
            }

            // TODO: Create and update timer for Title
            // TODO: Show manual signing options

            DisplayRequest(request, "Sign - {0}", subtitle, true, expires);
        }

        public void OnFailure(SigningRequest request, Exception error)
        {
            if (error is LinkException)
            {
                var linkError = error as LinkException;
                if (linkError.Code == LinkErrorCode.E_CANCEL)
                {
                    CloseDialog();
                    return;
                }
            }
            if (request == this.activeRequest)
            {
                if (this.showRequestStatus)
                {
                    EnsureUI();
                    AnchorLinkUIManager.Instance.Initialize(this);
                    var panel = AnchorLinkUIManager.Instance.OpenPanel(AnchorLinkPanelType.Error);
                    panel.SetTitle("Transaction Error");
                    if (error is EosioApiErrorException)
                    {
                        var apiError = (EosioApiErrorException)error;
                        panel.SetSubtitle(apiError.message);
                    }
                    else if (error is EosioApiException)
                    {
                        var apiError = (EosioApiException)error;
                        panel.SetSubtitle(apiError.Content);
                    }
                    else
                    {
                        try
                        {
                            var jsonError = JObject.Parse(error.Message);
                            panel.SetSubtitle((string)jsonError["message"]);
                        }
                        catch
                        {
                            panel.SetSubtitle(error.Message);
                        }
                    }
                }
                else
                {
                    CloseDialog();
                }
            }
        }

        public async void OnSuccess(SigningRequest request, TransactResult result)
        {
            if (request == this.activeRequest)
            {
                if (this.showRequestStatus)
                {
                    string subtitle;
                    if (request.IsIdentity())
                    {
                        subtitle = "Login completed.";
                    }
                    else
                    {
                        subtitle = "Transaction signed.";
                    }
                    ShowMessageDialog(AnchorLinkPanelType.Success, "Success!", subtitle);
                    await UniTask.Delay(this.statusDuration, true);
                    CloseDialog();
                }
                else
                {
                    CloseDialog();
                }
            }
        }

        public async UniTask<SigningRequest> Prepare(SigningRequest request, LinkSession session)
        {
            this.activeRequest = request;

            ShowLoading();

            await UniTask.Delay(2000);

            if (session == null || request.IsIdentity())
            {

                // don't attempt to cosign id request or if we don't have a session attached
                return request;
            }

            await UniTask.NextFrame();

            return request;
        }

        protected void DisplayRequest(SigningRequest request, string title, string subtitle, bool countdown = false, float expires = 0)
        {
            var panelType = request.IsIdentity() ? AnchorLinkPanelType.Login : AnchorLinkPanelType.Sign;
            ShowActionDialog(panelType, title, subtitle, "Launch Anchor", () =>
            {
                if (ActiveRequest != null)
                {
                    Application.OpenURL(ActiveRequest.Encode());
                }
            }, countdown, expires);
        }

        protected void ShowRecovery(SigningRequest request, LinkSession session)
        {
            if (session.Type == "channel")
            {
                var channelSession = session as LinkChannelSession;
                channelSession.AddLinkInfo(request);
            }
            DisplayRequest(
                request,
                "Sign manually",
                "Want to sign with another device or didn't get the signing request in your wallet, scan this QR or copy request and paste in app.");
            this.showingManual = true;
        }

        public bool RecoverError(Exception error, SigningRequest request)
        {
            if (request == this.activeRequest)
            {
                if (error is LinkException)
                {
                    Debug.LogException(error);
                    var linkError = (SessionException)error;
                    if (linkError.Code == LinkErrorCode.E_DELIVERY || linkError.Code == LinkErrorCode.E_TIMEOUT)
                    {

                        // recover from session errors by displaying a manual sign dialog
                        if (this.showingManual)
                        {

                            // already showing recovery sign
                            return true;
                        }
                        var session = linkError.Session;
                        if (linkError.SkipToManual)
                        {
                            ShowRecovery(request, session);
                            return true;
                        }
                        string subtitle;
                        if (session.Metadata.ContainsKey("name") && !string.IsNullOrEmpty((string)session.Metadata["name"]))
                        {
                            var deviceName = (string)session.Metadata["name"];
                            subtitle = $"Unable to deliver the request to “{deviceName}”.";
                        }
                        else
                        {
                            subtitle = "Unable to deliver the request to the linked wallet.";
                        }
                        subtitle += $" {error.Message}";
                        ShowActionDialog(AnchorLinkPanelType.Warning, "Unable to reach device", subtitle, "Sign manually", () =>
                        {
                            ShowRecovery(request, session);
                        });
                        return true;
                    }
                }
            }
            return false;
        }

        public bool SendSessionPayload(byte[] payload, LinkSession session)
        {
            return false;
        }

        public void ShowLoading()
        {
            ShowMessageDialog(AnchorLinkPanelType.Loading, "Loading", "Preparing request...");
        }

        public async void ShowMessageDialog(AnchorLinkPanelType panelType, string title, string subtitle)
        {
            //await UniTask.NextFrame();
            EnsureUI();
            await UniTask.NextFrame();
            //await UniTask.WaitUntil(() => AnchorLinkUIManager.Instance != null);
            AnchorLinkUIManager.Instance.Initialize(this);
            var panel = AnchorLinkUIManager.Instance.OpenPanel(panelType);
            panel.SetTitle(title);
            panel.SetSubtitle(subtitle);
        }

        public async void ShowActionDialog(AnchorLinkPanelType panelType, string title, string subtitle, string actionText, UnityAction callback, bool countdown = false, float expires = 0)
        {
            //await UniTask.NextFrame();
            EnsureUI();
            await UniTask.NextFrame();
            //await UniTask.WaitUntil(() => AnchorLinkUIManager.Instance != null);
            AnchorLinkUIManager.Instance.Initialize(this);
            var panel = AnchorLinkUIManager.Instance.OpenPanel(panelType);
            panel.SetCountdown(countdown, expires);
            panel.SetTitle(title);
            panel.SetSubtitle(subtitle);
            panel.SetActionCallback(callback);
            panel.SetActionText(actionText);
        }

        public void CloseDialog()
        {
            if (AnchorLinkUIManager.Instance != null)
            {
                AnchorLinkUIManager.Instance.ClosePanel();
            }
        }

    }

}