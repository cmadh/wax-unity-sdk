using System.Collections;
using System.Collections.Generic;

using Cysharp.Threading.Tasks;

using NativeWebSocket;

using UnityEngine;

using UnityEngine.Networking;

namespace StrattonStudios.AnchorLinkUnity
{

    public class BuoyCallback : ILinkCallback
    {

        private bool active;
        private WebSocket socket;
        private ISocketHandler socketHandler;

        public string Url { get; private set; }

        public bool Cancelled { get; private set; } = false;

        public BuoyCallback(string url, ISocketHandler socketHandler)
        {
            Url = url;
            this.socketHandler = socketHandler;
        }

        public UniTask<string> Wait()
        {
            Cancelled = false;
            if (Url.Contains("hyperbuoy"))
            {
                return PollForCallback(Url);
            }
            else
            {
                return WaitForCallback(Url);
            }
        }

        public async void Cancel()
        {
            this.active = false;
            Cancelled = true;
            if (this.socket != null && (this.socket.State == WebSocketState.Open || this.socket.State == WebSocketState.Connecting))
            {
                await this.socket.Close();
            }
        }

        /// <summary>
        /// Connect to a WebSocket channel and wait for a message.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private async UniTask<string> WaitForCallback(string url)
        {
            var utcs = new UniTaskCompletionSource<string>();
            int retries = 0;
            string socketUrl = url.Replace("http", "ws");

            this.socket = new WebSocket(socketUrl);
            this.socket.OnMessage += async (bytes) =>
            {
                this.active = false;
                if (this.socket.State == WebSocketState.Open)
                {
                    await this.socket.Close();
                }
                string stringData = System.Text.Encoding.UTF8.GetString(bytes);
                utcs.TrySetResult(stringData);
            };

            this.socket.OnError += (error) =>
            {
                Debug.LogError(error);
            };

            this.socket.OnOpen += () =>
            {
                this.socketHandler.AddSocket(this.socket);
                retries = 0;
            };

            this.socket.OnClose += async (error) =>
            {
                this.socketHandler.RemoveSocket(this.socket);
                if (this.active)
                {
                    retries++;
                    await UniTask.Delay(Backoff(retries));
                    await this.socket.Connect();
                }
            };

            await this.socket.Connect();
            if (Cancelled)
            {
                throw new System.InvalidOperationException("Cancelled by user");
            }

            string result = await utcs.Task;
            return result;
        }

        /// <summary>
        /// Long-poll for message.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private async UniTask<string> PollForCallback(string url)
        {
            while (this.active)
            {
                try
                {
                    var request = UnityWebRequest.Get(url);
                    await request.SendWebRequest();
                    if (request.responseCode == 408)
                    {
                        continue;
                    }
                    else if (request.responseCode == 200)
                    {
                        return request.downloadHandler.text;
                    }
                    else
                    {
                        throw new System.Exception($"HTTP {request.responseCode}: {request.downloadHandler.text}");
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogWarning("Unexpected hyperbuoy error");
                    Debug.LogException(e);
                }
                await UniTask.Delay(1000, true);
            }
            if (Cancelled)
            {
                throw new System.InvalidOperationException("Cancelled by user");
            }
            return null;
        }

        /// <summary>
        /// Exponential backoff function that caps off at 10s after 10 tries.
        /// https://i.imgur.com/IrUDcJp.png
        /// </summary>
        /// <param name="tries"></param>
        /// <returns></returns>
        private int Backoff(int tries)
        {
            return Mathf.Min((int)Mathf.Pow(tries * 10, 2), 10 * 1000);
        }

    }

}