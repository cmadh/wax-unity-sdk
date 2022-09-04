using System;
using System.Collections.Generic;
using System.Text;

using NativeWebSocket;

namespace StrattonStudios.AnchorLinkUnity
{

    public interface ISocketHandler
    {

        void AddSocket(WebSocket socket);

        void RemoveSocket(WebSocket socket);

    }

}
