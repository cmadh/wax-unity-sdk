using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace StrattonStudios.AnchorLinkUnity.Transports.UGUI
{

    public class AnchorLinkUIManager : MonoBehaviour
    {

        public static AnchorLinkUIManager Instance { get; private set; }

        [SerializeField]
        protected AnchorLinkPanel panel;

        protected LinkUnityUGUITransport transport;

        protected virtual void Awake()
        {
            Instance = this;
        }

        protected virtual void OnDestroy()
        {
            Instance = null;
        }

        public virtual void Initialize(LinkUnityUGUITransport transport)
        {
            this.transport = transport;
        }

        public virtual AnchorLinkPanel OpenPanel(AnchorLinkPanelType panelType)
        {
            this.panel.Open();
            this.panel.Initialize(this.transport, panelType);
            return this.panel;
        }

        public virtual void ClosePanel()
        {
            this.panel.Close();
        }

    }

    public enum AnchorLinkPanelType
    {
        Login,
        Sign,
        Loading,
        Success,
        Warning,
        Error
    }

}