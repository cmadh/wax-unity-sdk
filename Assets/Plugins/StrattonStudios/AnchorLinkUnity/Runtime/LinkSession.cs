using System.Collections;
using System.Collections.Generic;

using Cysharp.Threading.Tasks;

using Newtonsoft.Json;

using StrattonStudios.EosioUnity;
using StrattonStudios.EosioUnity.Models;

using UnityEngine;

namespace StrattonStudios.AnchorLinkUnity
{

    public abstract class LinkSession
    {

        protected Link link;

        protected string identifier;

        //protected ChainId chainId;

        protected string publicKey;

        protected PermissionLevel auth;

        protected string type;

        protected Dictionary<string, object> metadata;

        /// <summary>
        /// The underlying link instance used by the session.
        /// </summary>
        public Link Link
        {
            get
            {
                return this.link;
            }
            protected set
            {
                this.link = value;
            }
        }

        /// <summary>
        /// App identifier that owns the session.
        /// </summary>
        public string Identifier
        {
            get
            {
                return this.identifier;
            }
            protected set
            {
                this.identifier = value;
            }
        }

        /// <summary>
        /// Id of the chain where the session is valid.
        /// </summary>
        //public ChainId ChainId
        //{
        //    get
        //    {
        //        return this.chainId;
        //    }
        //    protected set
        //    {
        //        this.chainId = value;
        //    }
        //}

        /// <summary>
        /// The public key the session can sign for.
        /// </summary>
        public string PublicKey
        {
            get
            {
                return this.publicKey;
            }
            protected set
            {
                this.publicKey = value;
            }
        }

        /// <summary>
        /// The EOSIO auth (a.k.a. permission level) the session can sign for.
        /// </summary>
        public PermissionLevel Auth
        {
            get
            {
                return this.auth;
            }
            protected set
            {
                this.auth = value;
            }
        }

        /// <summary>
        /// Session type, e.g. 'channel'.
        /// </summary>
        public string Type
        {
            get
            {
                return this.type;
            }
            protected set
            {
                this.type = value;
            }
        }

        /// <summary>
        /// Arbitrary metadata that will be serialized with the session.
        /// </summary>
        public Dictionary<string, object> Metadata
        {
            get
            {
                return this.metadata;
            }
            protected set
            {
                this.metadata = value;
            }
        }

        public LinkSession()
        {

        }

        /// <summary>
        /// Creates a EOS Unity compatible signature provider that can sign for the session public key.
        /// </summary>
        /// <returns></returns>
        public abstract object MakeSignatureProvider();

        /// <summary>
        /// Transact using this session. See <see cref="Link.Transact"/>.
        /// </summary>
        /// <param name="args"></param>
        /// <param name="options"></param>
        public abstract UniTask<TransactResult> Transact(TransactArgs args, TransactOptions options);

        /// <summary>
        /// Returns a JSON-encodable object that can be used recreate the session.
        /// </summary>
        /// <returns></returns>
        public abstract SerializedLinkSession Serialize();

        /// <summary>
        /// Restore a previously serialized session.
        /// </summary>
        /// <param name="link"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        /// <exception cref="System.Exception"></exception>
        public static LinkSession Restore(Link link, SerializedLinkSession data)
        {
            switch (data.type)
            {
                case "channel":
                    return new LinkChannelSession(link, JsonConvert.DeserializeObject<LinkChannelSessionData>(data.data), data.metadata);
                case "fallback":
                    return new LinkFallbackSession(link, JsonConvert.DeserializeObject<LinkFallbackSessionData>(data.data), data.metadata);
                default:
                    throw new System.Exception("Unable to restore, session data invalid");
            }
        }

    }

    public class SerializedLinkSession
    {

        public string type;
        public Dictionary<string, object> metadata;
        public string data;

    }

    public class LinkSessionData
    {

        /// <summary>
        /// App identifier that owns the session.
        /// </summary>
        public string identifier;

        /// <summary>
        /// Authenticated user permission.
        /// </summary>
        public SerializablePermissionLevel auth;

        /// <summary>
        /// Public key of authenticated user
        /// </summary>
        public string publicKey;

        /// <summary>
        /// The session chain id.
        /// </summary>
        //public ChainId chainId;

    }

}