using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace StrattonStudios.EosioUnity.Signing
{

    public class SigningRequestEncodingOptions
    {

        /// <summary>
        /// Optional zlib, if provided the request will be compressed when encoding.
        /// </summary>
        public IZlibProvider zlib;

        /// <summary>
        /// Abi provider, required if the arguments contain un-encoded actions.
        /// </summary>
        public IAbiProvider abiProvider;

        /// <summary>
        /// Optional signature provider, will be used to create a request signature if provided.
        /// </summary>
        public ISignatureProvider signatureProvider;

    }

}