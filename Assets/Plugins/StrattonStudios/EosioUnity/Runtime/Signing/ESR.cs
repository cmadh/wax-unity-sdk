using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace StrattonStudios.EosioUnity.Signing
{

    /// <summary>
    /// EOSIO Signing Request.
    /// </summary>
    public class ESR
    {

        private IAbiProvider abiProvider;
        private IZlibProvider zlibProvider;

        public ESR(IAbiProvider abiProvider) : this(abiProvider, DefaultZlibProvider.Instance)
        {

        }

        public ESR(IAbiProvider abiProvider, IZlibProvider zlibProvider)
        {
            if (zlibProvider == null)
            {
                zlibProvider = DefaultZlibProvider.Instance;
            }
            this.abiProvider = abiProvider;
            this.zlibProvider = zlibProvider;
        }

        public IAbiProvider GetAbiProvider()
        {
            return this.abiProvider;
        }

        public IZlibProvider GetZlibProvider()
        {
            return this.zlibProvider;
        }

    }

}