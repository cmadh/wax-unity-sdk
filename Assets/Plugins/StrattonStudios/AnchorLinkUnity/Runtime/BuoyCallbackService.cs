using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace StrattonStudios.AnchorLinkUnity
{

    public class BuoyCallbackService : ILinkCallbackService
    {

        public readonly string Address;

        private ISocketHandler socketHandler;

        public BuoyCallbackService(string address, ISocketHandler socketHandler)
        {
            this.Address = address.Trim().TrimEnd('/');
            this.socketHandler = socketHandler;
        }

        public ILinkCallback Create()
        {
            string url = string.Format("{0}/{1}", this.Address, System.Guid.NewGuid());
            return new BuoyCallback(url, this.socketHandler);
        }

    }

}