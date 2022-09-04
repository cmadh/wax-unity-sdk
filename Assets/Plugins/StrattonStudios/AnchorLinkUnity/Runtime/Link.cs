using System.Collections;
using System.Collections.Generic;
using System.Linq;

using StrattonStudios.AnchorLinkUnity.Utilities;

using Cryptography.ECDSA;

using Cysharp.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using StrattonStudios.EosioUnity;
using StrattonStudios.EosioUnity.Models;
using StrattonStudios.EosioUnity.Serialization;
using StrattonStudios.EosioUnity.Signing;
using StrattonStudios.EosioUnity.Utilities;

using UnityEngine;

namespace StrattonStudios.AnchorLinkUnity
{

    public class Link
    {

        public const string Version = "1.0.0";

        //public readonly List<LinkChain> Chains;
        public readonly LinkChain ActiveChain;
        public readonly ILinkTransport Transport;
        public readonly ILinkStorage Storage;

        private ILinkCallbackService callbackService;
        private bool verifyProofs;
        //private bool encodeChainIds;

        /// <summary>
        /// The EOS Client instance for communicating with the node.
        /// </summary>
        /// <remarks>
        /// This returns the first EosClientAPI when link is configured with multiple chains.
        /// </remarks>
        public EosioClient Client
        {
            get
            {
                //return this.Chains[0].Client;
                return this.ActiveChain.Client;
            }
        }

        /// <summary>
        /// Create a new link instance.
        /// </summary>
        /// <param name="options">The link options</param>
        /// <exception cref="System.ArgumentException"></exception>
        public Link(LinkOptions options)
        {
            if (options.Transport == null)
            {
                throw new System.ArgumentException("options.Transport is required", nameof(options.Transport));
            }
            if (options.Chain == null)
            {
                throw new System.ArgumentException("options.Chains is required", nameof(options.Chain));
            }
            //if (options.Chains == null || options.Chains.Count == 0)
            //{
            //    throw new System.ArgumentException("options.Chains is required", nameof(options.Chains));
            //}
            var chainId = options.Chain.Chain.ToChainId();
            var config = new EosioClientConfig()
            {
                //ChainId = chainId.GetChainId(),
                ChainType = options.Chain.ChainType,
                HttpEndpoint = options.Chain.NodeUrl
            };
            var client = new EosioClient(config);
            this.ActiveChain = new LinkChain(chainId, client, options.Chain.ChainType);
            //this.Chains = new List<LinkChain>();
            //for (int i = 0; i < options.Chains.Count; i++)
            //{
            //    var chain = options.Chains[i];
            //    if (chain.Chain == null)
            //    {
            //        throw new System.ArgumentNullException("Chain is required");
            //    }
            //    if (string.IsNullOrEmpty(chain.NodeUrl))
            //    {
            //        throw new System.ArgumentException("Chain NodeUrl is required");
            //    }
            //    var chainId = chain.Chain.ToChainId();
            //    var config = new EosConfigurator()
            //    {
            //        ChainId = chainId.GetChainId(),
            //        HttpEndpoint = chain.NodeUrl
            //    };
            //    var client = new EosClientAPI(config, httpHandler);
            //    this.Chains.Add(new LinkChain(chainId, client));
            //}
            this.Transport = options.Transport;
            this.callbackService = options.Service;
            if (options.Storage != null)
            {
                this.Storage = options.Storage;
            }
            else if (this.Transport.Storage != null)
            {
                this.Storage = this.Transport.Storage;
            }
            this.verifyProofs = options.VerifyProofs;
            //this.encodeChainIds = options.EncodeChainIds;
        }

        /// <summary>
        /// Create a SigningRequest instance configured for this link.
        /// </summary>
        /// <param name="args"></param>
        /// <param name="transport"></param>
        /// <returns></returns>
        public async UniTask<(SigningRequest, ILinkCallback)> CreateRequest(SigningRequestCreateArgs args, ILinkTransport transport)
        {
            SigningRequestEncodingOptions options = new SigningRequestEncodingOptions();
            options.abiProvider = this.ActiveChain;
            options.zlib = DefaultZlibProvider.Instance;

            //if (chain != null)
            //{
            //    args.chainId = Chain.FromChainId(chain.ChainId.GetChainId());
            //}
            //else
            //{
            //args.chainId = Chain.FromChainId(this.Chains[0].ChainId.GetChainId());
            //args.chainId = Chain.FromChainId(this.ActiveChain.ChainId.GetChainId());
            //}

            args.ChainType = this.ActiveChain.ChainType;

            args.broadcast = false;

            var request = await SigningRequest.Create(args, options);

            request.SetRequestFlag(RequestFlag.Background);

            var t = transport ?? this.Transport;
            await t.Prepare(request, null);

            var linkCallback = this.callbackService.Create();
            request.SetCallback(linkCallback.Url);

            return (request, linkCallback);
        }

        /// <summary>
        /// Send a SigningRequest instance using this link.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="callback"></param>
        /// <param name="transport"></param>
        /// <param name="broadcast"></param>
        /// <returns></returns>
        public async UniTask<TransactResult> SendRequest(SigningRequest request, ILinkCallback callback, ILinkTransport transport, bool broadcast)
        {
            var t = transport ?? this.Transport;

            try
            {
                var linkUrl = request.GetCallback();
                if (linkUrl != callback.Url)
                {
                    throw new System.ArgumentException("Invalid request callback");
                }
                var flags = request.GetRequestFlag();
                if (!flags.IsBackground() || flags.IsBroadcast())
                {
                    throw new System.ArgumentException("Invalid request flags");
                }
                bool done = false;

                // Wait for callback or user cancel
                t.OnRequest(request, (reason) =>
                {
                    if (done)
                    {

                        // Ignore any cancel calls once callback below has resolved
                        return;
                    }

                    if (t.RecoverError(reason, request))
                    {
                        return;
                    }
                    callback.Cancel();
                });

                string response = await callback.Wait();

                done = true;
                var payload = JObject.Parse(response);
                if (payload.ContainsKey("rejected"))
                {
                    throw new CancelException((string)payload["rejected"]);
                }
                var signer = new PermissionLevel(new AccountName((string)payload["sa"]), new PermissionName((string)payload["sp"]));
                var signatures = new List<string>();
                foreach (var property in payload.Properties())
                {
                    if (property.Name.StartsWith("sig") && property.Name != "sig0")
                    {
                        signatures.Add((string)property.Value);
                    }
                }
                var theChain = this.ActiveChain;
                if (payload.ContainsKey("cid") && (string)payload["cid"] != theChain.ChainId.GetChainId())
                {
                    throw new System.Exception("Got response for wrong chain id");
                }

                // Recreate transaction from request response
                var options = new SigningRequestEncodingOptions();
                options.abiProvider = theChain;
                options.zlib = DefaultZlibProvider.Instance;

                var resolved = await ResolvedSigningRequest.FromPayload(payload, options);

                // Prepend cosigner signature if present
                var array = resolved.SigningRequest.GetInfoKey("cosig", "signature[]", AbiData.GetCachedAbi());
                if (array != null && array is IList)
                {
                    var list = array as IList;
                    foreach (string sig in list)
                    {
                        signatures.Insert(0, sig);
                    }
                }

                var result = new TransactResult();
                result.resolved = resolved;
                result.chain = theChain;
                result.signatures = signatures;
                result.signer = signer;
                result.payload = payload;
                result.transaction = resolved.Transaction;
                result.serializedTransaction = resolved.SerializedTransaction;
                result.resolvedTransaction = resolved.Transaction;
                if (broadcast)
                {
                    var signedTx = new SignedTransaction();
                    signedTx.Signatures = signatures;
                    signedTx.PackedTransaction = resolved.SerializedTransaction;
                    result.processed = await theChain.Client.ChainApi.PushTransaction(new PushTransactionRequest()
                    {
                        signatures = signedTx.Signatures.ToArray(),
                        compression = 0,
                        packed_context_free_data = "",
                        packed_trx = HexUtility.ToHexString(signedTx.PackedTransaction)
                    });
                }
                t.OnSuccess(request, result);
                return result;
            }
            catch (System.Exception e)
            {
                t.OnFailure(request, e);
                throw e;
            }
        }

        /// <summary>
        /// Send an identity request and verify the identity proof if <see cref="LinkOptions.VerifyProofs"/> is true.
        /// </summary>
        /// <remarks>
        /// This is for advanced use-cases, you probably want to use <see cref="Login(string)"/> instead.
        /// </remarks>
        /// <param name="scope">The scope of the identity request.</param>
        /// <param name="requestPermission">Optional request permission if the request is for a specific account or permission.</param>
        /// <param name="info">Metadata to add to the request.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception"></exception>
        public async UniTask<IdentifyResult> Identify(string scope, PermissionLevel requestPermission, List<InfoPair> info)
        {
            var result = new IdentifyResult();
            var args = new SigningRequestCreateArgs();
            args.identityV2 = new IdentityV2(requestPermission);
            args.info = info;
            var (request, callback) = await CreateRequest(args, null);
            var response = await SendRequest(request, callback, null, false);
            if (!response.resolved.SigningRequest.IsIdentity())
            {
                throw new System.Exception("Unexpected response");
            }
            var proof = response.resolved.GetIdentityProof(response.signatures[0]);
            if (this.verifyProofs)
            {
                var account = await response.chain.Client.GetAccount(proof.signer.Actor.Value);
                result.account = account;
                if (account == null)
                {
                    throw new System.Exception($"Signature from unknown account: {proof.signer.Actor.Value}");
                }
                var accountPermission = account.permissions.Find(perm =>
                {
                    return proof.signer.Permission.Value.Equals(perm.perm_name);
                });
                if (accountPermission == null)
                {
                    throw new System.Exception($"{proof.signer.Actor.Value} signed for unknown permission: {proof.signer.Permission.Value}");
                }
                var proofValid = proof.Verify(accountPermission.required_auth, account.head_block_num);
                if (!proofValid)
                {
                    throw new System.Exception($"Invalid identify proof for: {proof.signer}");
                }
            }

            if (requestPermission != null)
            {
                if ((requestPermission.Actor.Value != SigningRequest.PLACEHOLDER_NAME && requestPermission.Actor.Value != proof.signer.Actor.Value) ||
                    (requestPermission.Permission.Value != SigningRequest.PLACEHOLDER_PERMISSION && requestPermission.Permission.Value != proof.signer.Permission.Value))
                {
                    throw new System.Exception($"Identity proof singed by {proof.signer}, expected: {requestPermission}");
                }
            }

            result.transactResult = response;
            result.proof = proof;
            return result;
        }

        /// <summary>
        /// Login and create a persistent session.
        /// </summary>
        /// <param name="identifier">The session identifier, an EOSIO name (`[a-z1-5]{1,12}`).
        /// Should be set to the contract account if applicable.</param>
        /// <returns></returns>
        public async UniTask<LoginResult> Login(string identifier)
        {
            var keyPair = KeyUtility.GenerateKeyPair(KeyType.K1);
            LinkCreate createInfo = new LinkCreate(identifier, keyPair.PublicKey);
            var info = new List<InfoPair>();
            info.Add(InfoPair.Create("link", Client.AbiWriter.SerializeStructData("link_create", createInfo.ToDictionary(), LinkAbiData.Types)));
            //info.Add(InfoPair.Create("scope", System.Text.Encoding.UTF8.GetBytes(identifier)));
            info.Add(InfoPair.Create("scope", Client.AbiWriter.SerializeType("name", identifier, AbiData.GetCachedAbi())));
            var response = await Identify(identifier, null, info);
            var metadata = LinkUtils.SessionMetadata(response.transactResult.payload, response.transactResult.resolved.SigningRequest);

            // TODO: Recover public key
            // const signerKey = res.proof.recover()
            //response.proof.Recover();

            LinkSession session;
            var payload = response.transactResult.payload;
            if (payload.ContainsKey("link_ch") && payload.ContainsKey("link_name") && payload.ContainsKey("link_key"))
            {
                var sessionData = new LinkChannelSessionData();
                sessionData.identifier = identifier;
                //sessionData.chainId = response.transactResult.chain.ChainId;
                sessionData.auth = response.transactResult.signer;

                //sessionData.publicKey = signerKey;

                sessionData.channel = new ChannelInfo();
                sessionData.channel.name = (string)payload["link_name"];
                sessionData.channel.key = (string)payload["link_key"];
                sessionData.channel.url = (string)payload["link_ch"];

                sessionData.requestKey = keyPair.PrivateKey;

                session = new LinkChannelSession(this, sessionData, metadata);
            }
            else
            {
                var sessionData = new LinkFallbackSessionData();
                sessionData.identifier = identifier;
                //sessionData.chainId = response.transactResult.chain.ChainId;
                sessionData.auth = response.transactResult.signer;

                //sessionData.publicKey = signerKey;
                session = new LinkFallbackSession(this, sessionData, metadata);
            }
            await StoreSession(session);

            var result = new LoginResult();
            result.identifyResult = response;
            result.session = session;
            return result;
        }

        /// <summary>
        /// Sign and optionally broadcast a EOSIO transaction, action or actions.
        /// </summary>
        /// <param name="args">The action, actions or transaction to use.</param>
        /// <param name="options">Options for this transact call.</param>
        /// <param name="transport">Transport override, for internal use.</param>
        /// <returns></returns>
        public async UniTask<TransactResult> Transact(TransactArgs args, TransactOptions options, ILinkTransport transport)
        {
            var o = options ?? new TransactOptions();
            var t = transport ?? this.Transport;
            var broadcast = o.broadcast != null ? o.broadcast.Value : false;
            var noModify = o.noModify != null ? o.noModify.Value : !broadcast;

            // Initialize the loading state of the transport
            if (t != null)
            {
                t.ShowLoading();
            }

            // eosjs transact compat: upgrade to transaction if args have any header fields
            // TODO: Might be unnecessary

            var createArgs = new SigningRequestCreateArgs();
            createArgs.action = args.action;
            createArgs.actions = args.actions;
            createArgs.transaction = args.transaction;
            //createArgs.chainId = Chain.FromChainId(this.ActiveChain.ChainId.GetChainId());
            createArgs.ChainType = this.ActiveChain.ChainType;
            createArgs.broadcast = broadcast;
            var (request, callback) = await CreateRequest(createArgs, t);
            if (noModify)
            {
                var serializer = new AbiTypeWriter(null);
                request.SetInfoKey("no_modify", serializer.SerializeType("bool", true, LinkAbiData.Types));
            }
            var result = await SendRequest(request, callback, t, broadcast);
            return result;
        }

        /// <summary>
        /// Restore previous session, use <see cref="Login(string)"/> to create a new session.
        /// </summary>
        /// <param name="identifier">The session identifier, must be same as what was used when creating the session with <see cref="Login(string)"/>.</param>
        /// <param name="auth">A specific session auth to restore, if omitted the most recently used session will be restored.</param>
        /// <returns>A <see cref="LinkSession"/> instance or null if no session can be found.</returns>
        /// <exception cref="LinkException"></exception>
        public async UniTask<LinkSession> RestoreSession(string identifier, SerializablePermissionLevel auth)
        {
            if (this.Storage == null)
            {
                throw new System.Exception("Unable to list sessions: No storage adapter configured");
            }
            string key;
            if (auth != null)
            {

                // auth given, we can look up on specific key
                key = SessionKey(identifier, FormatAuth(auth));
            }
            else
            {

                // otherwise we use the session list to filter down to most recently used matching given params
                var list = await ListSessions(identifier);
                if (list == null || list.Count == 0)
                {
                    return null;
                }
                var latest = list[0];
                key = SessionKey(identifier, FormatAuth(latest));
            }
            var data = await this.Storage.Read(key);
            if (data == null)
            {
                return null;
            }
            SerializedLinkSession sessionData;
            try
            {
                sessionData = JsonConvert.DeserializeObject<SerializedLinkSession>(data);
            }
            catch (System.Exception)
            {

                throw;
            }
            var session = LinkSession.Restore(this, sessionData);
            if (auth != null)
            {

                // update latest used
                await TouchSession(identifier, session.Auth);
            }
            return session;
        }

        /// <summary>
        /// List stored session auths for given identifier.
        /// </summary>
        /// <remarks>
        /// The most recently used session is at the top (index 0).
        /// </remarks>
        /// <param name="identifier"></param>
        /// <returns></returns>
        /// <exception cref="System.Exception"></exception>
        public async UniTask<List<SerializablePermissionLevel>> ListSessions(string identifier)
        {
            if (this.Storage == null)
            {
                throw new System.Exception("Unable to list sessions: No storage adapter configured");
            }
            var key = SessionKey(identifier, "list");
            var list = new List<SerializablePermissionLevel>();
            try
            {
                var json = await this.Storage.Read(key);
                list = JsonConvert.DeserializeObject<List<SerializablePermissionLevel>>(json);
                //var array = JArray.Parse(json);
                //foreach (var item in array)
                //{
                //    list.Add(new PermissionLevel((JObject)item));
                //}
            }
            catch (System.Exception ex)
            {
                throw new System.Exception($"Unable to list sessions: Stored JSON invalid ({ex.Message ?? ex.ToString()})");
            }
            return list;
        }

        /// <summary>
        /// Remove stored session for given identifier and auth.
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="auth"></param>
        /// <returns></returns>
        /// <exception cref="LinkException"></exception>
        public async UniTask RemoveSession(string identifier, SerializablePermissionLevel auth)
        {
            if (this.Storage == null)
            {
                throw new System.Exception("Unable to list sessions: No storage adapter configured");
            }
            var key = SessionKey(identifier, FormatAuth(auth));
            await this.Storage.Remove(key);
            await TouchSession(identifier, auth, true);
        }

        /// <summary>
        /// Remove all stored sessions for given identifier.
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        /// <exception cref="LinkException"></exception>
        public async UniTask ClearSessions(string identifier)
        {
            if (this.Storage == null)
            {
                throw new System.Exception("Unable to list sessions: No storage adapter configured");
            }
            foreach (var auth in await ListSessions(identifier))
            {
                await RemoveSession(identifier, auth);
            }
        }

        /// <summary>
        /// Makes sure session is in storage list of sessions and moves it to top (most recently used).
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="auth"></param>
        /// <param name="remove"></param>
        /// <returns></returns>
        private async UniTask TouchSession(string identifier, SerializablePermissionLevel auth, bool remove = false)
        {
            var list = await ListSessions(identifier);
            if (list == null)
            {
                list = new List<SerializablePermissionLevel>();
            }
            var existing = list.FindIndex(item =>
            {
                return item.permission == auth.permission && item.actor == auth.actor;
            });
            if (existing >= 0)
            {
                list.RemoveRange(existing, 1);
            }
            if (!remove)
            {
                list.Insert(0, auth);
            }
            var key = SessionKey(identifier, "list");
            await this.Storage.Write(key, JsonConvert.SerializeObject(list));
        }

        /// <summary>
        /// Makes sure session is in storage list of sessions and moves it to top (most recently used).
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public async UniTask StoreSession(LinkSession session)
        {
            if (this.Storage != null)
            {
                var key = SessionKey(session.Identifier, FormatAuth(session.Auth));
                var json = JsonConvert.SerializeObject(session.Serialize());
                await this.Storage.Write(key, json);
                await TouchSession(session.Identifier, session.Auth);
            }
        }

        private string SessionKey(string identifier, params string[] suffix)
        {
            var key = new List<string>();
            key.Add(identifier);
            key.AddRange(suffix);
            return string.Join(".", key);
        }

        /// <summary>
        /// Create an eosjs compatible signature provider using this link.
        /// </summary>
        /// <remarks>
        /// We don't know what keys are available so those have to be provided,
        /// to avoid this use <see cref="LinkSession.MakeSignatureProvider"/> instead. Sessions can be created with <see cref="Login(string)"/>.
        /// </remarks>
        /// <param name="availableKeys">Keys the created provider will claim to be able to sign for.</param>
        /// <param name="chain">Chain to use when configured with multiple chains.</param>
        /// <param name="transport">Transport override for this call.</param>
        /// <returns></returns>
        public LinkSignatureProvider MakeSignatureProvider(string[] availableKeys, ILinkTransport transport)
        {
            //if (chain != null && chain.GetChainId() != this.ActiveChain.ChainId.GetChainId())
            //{
            //    throw new System.NotImplementedException("The multi-chain request is not implemented");
            //}
            return new LinkSignatureProvider(this, this.callbackService, transport, availableKeys);
        }

        public static string FormatAuth(SerializablePermissionLevel auth)
        {
            string actor = auth.actor;
            string permission = auth.permission;

            if (actor == SigningRequest.PLACEHOLDER_NAME)
            {
                actor = "<any>";
            }
            if (permission == SigningRequest.PLACEHOLDER_NAME || permission == SigningRequest.PLACEHOLDER_PERMISSION)
            {
                permission = "<any>";
            }

            return $"{actor}@{permission}";
        }

    }

}