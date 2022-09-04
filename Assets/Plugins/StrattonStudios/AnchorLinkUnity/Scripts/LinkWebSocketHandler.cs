using System.Collections;
using System.Collections.Generic;

using NativeWebSocket;

using UnityEngine;

namespace StrattonStudios.AnchorLinkUnity
{

    /// <summary>
    /// Dispatches the WebSockets message queue.
    /// </summary>
    public class LinkWebSocketHandler : MonoBehaviour, ISocketHandler
    {

        private static LinkWebSocketHandler instance;

        public static LinkWebSocketHandler Instance
        {
            get
            {
                if (instance == null)
                {
                    CreateInstance();
                }
                return instance;
            }
        }

        private static List<WebSocket> sockets = new List<WebSocket>();

        public static List<WebSocket> Sockets
        {
            get
            {
                return sockets;
            }
        }

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
        }

#if !UNITY_WEBGL || UNITY_EDITOR
        private void Update()
        {
            if (sockets == null || sockets.Count == 0)
            {
                return;
            }
            for (int i = 0; i < sockets.Count; i++)
            {
                if (sockets[i] != null)
                {
                    sockets[i].DispatchMessageQueue();
                }
            }
        }
#endif

        private async void OnDisable()
        {
            for (int i = 0; i < sockets.Count; i++)
            {
                if (sockets[i] != null)
                {
                    await sockets[i].Close();
                }
            }
        }

        public void AddSocket(WebSocket socket)
        {
            if (instance == null)
            {
                CreateInstance();
            }
            sockets.Add(socket);
        }

        public void RemoveSocket(WebSocket socket)
        {
            sockets.Remove(socket);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void CreateInstance()
        {
            if (instance == null)
            {
                instance = new GameObject("Anchor Link - Web Socket Handler").AddComponent<LinkWebSocketHandler>();
                DontDestroyOnLoad(instance);
                LinkOptions.DefaultSocketHandler = instance;
            }
        }

    }

}