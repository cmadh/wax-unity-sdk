using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

using ZXing;
using ZXing.QrCode;

namespace StrattonStudios.AnchorLinkUnity.Transports.UGUI
{

    public class AnchorLinkPanel : MonoBehaviour
    {

        [SerializeField]
        protected CanvasGroup canvasGroup;

        [Header("Logo")]
        [SerializeField]
        protected Animator logoAnimator;

        [Header("QR Code")]
        [SerializeField]
        protected GameObject qrSection;
        [SerializeField]
        protected RawImage qrImage;

        [Header("Actions")]
        [SerializeField]
        protected AnchorCopyLink copyLink;
        [SerializeField]
        protected GameObject actionSection;

        [Header("Events")]
        [SerializeField]
        protected UnityEvent<string> onSetTitle;
        [SerializeField]
        protected UnityEvent<string> onSetSubtitle;
        [SerializeField]
        protected UnityEvent<string> onSetActionText;
        [SerializeField]
        protected UnityEvent onActionClick;

        protected LinkUnityUGUITransport transport;

        protected Texture2D qrTexture;
        protected bool isOpen = false;
        protected AnchorLinkPanelType panelType = AnchorLinkPanelType.Login;
        protected float expires = 0f;
        protected bool countdown = false;
        protected string title;

        protected virtual void OnDisable()
        {
            CancelInvoke("UpdateCountodnw");
        }

        protected virtual void UpdateCountdown()
        {
            if (this.countdown)
            {
                float timeLeft = this.expires - (Time.unscaledTime * 1000);
                if (timeLeft > 0f)
                {
                    var timeSpan = System.TimeSpan.FromMilliseconds(timeLeft);
                    this.onSetTitle?.Invoke(string.Format(this.title, timeSpan.ToString(@"mm\:ss")));
                }
                else
                {
                    this.onSetTitle?.Invoke(string.Format(this.title, "00:00"));
                }
            }
        }

        public virtual void OnActionClicked()
        {
            this.onActionClick?.Invoke();
        }

        public virtual void Initialize(LinkUnityUGUITransport transport, AnchorLinkPanelType type)
        {
            this.transport = transport;
            if (this.logoAnimator != null)
            {
                this.logoAnimator.ResetTrigger(this.panelType.ToString());
                this.logoAnimator.SetTrigger(type.ToString());
            }
            this.panelType = type;
            this.qrSection.SetActive(this.panelType == AnchorLinkPanelType.Login || this.panelType == AnchorLinkPanelType.Sign);
            this.actionSection.SetActive(this.panelType == AnchorLinkPanelType.Login || this.panelType == AnchorLinkPanelType.Sign);

            if (this.transport.ActiveRequest == null)
            {
                return;
            }
            if (this.qrImage != null)
            {
                if (this.qrTexture == null)
                {
                    this.qrTexture = new Texture2D(256, 256);
                    this.qrImage.texture = this.qrTexture;
                }
                var color32 = EncodeQR(this.transport.ActiveRequest.Encode(), this.qrTexture.width, this.qrTexture.height);
                this.qrTexture.SetPixels32(color32);
                this.qrTexture.Apply();
            }
            if (this.copyLink != null)
            {
                this.copyLink.LinkToCopy = this.transport.ActiveRequest.Encode();
            }
        }

        public virtual void Open()
        {
            SetOpen(true);
            InvokeRepeating("UpdateCountdown", 0f, 0.2f);
        }

        public virtual void Close()
        {
            SetOpen(false);
            CancelInvoke("UpdateCountdown");
        }

        public virtual void SetOpen(bool open)
        {
            this.isOpen = open;
            this.canvasGroup.interactable = open;
            this.canvasGroup.blocksRaycasts = open;
            this.canvasGroup.alpha = open ? 1f : 0f;
        }

        public virtual void SetTitle(string title)
        {
            this.title = title;
            if (!this.countdown)
            {
                this.onSetTitle?.Invoke(title);
            }
        }

        public virtual void SetSubtitle(string subtitle)
        {
            this.onSetSubtitle?.Invoke(subtitle);
        }

        public virtual void SetActionText(string text)
        {
            this.onSetActionText?.Invoke(text);
        }

        public virtual void SetActionCallback(UnityAction callback)
        {
            this.onActionClick.RemoveAllListeners();
            this.onActionClick.AddListener(callback);
        }

        public virtual void SetCountdown(bool countdown, float expires)
        {
            this.countdown = countdown;
            this.expires = expires;
        }

        public virtual void LaunchAnchor()
        {
            if (this.transport.ActiveRequest != null)
            {
                Application.OpenURL(this.transport.ActiveRequest.Encode());
            }
        }

        private static Color32[] EncodeQR(string textForEncoding, int width, int height)
        {
            var writer = new BarcodeWriter
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new QrCodeEncodingOptions
                {
                    Height = height,
                    Width = width
                }
            };
            return writer.Write(textForEncoding);
        }

    }

}