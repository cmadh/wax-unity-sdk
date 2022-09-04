using System.Collections;
using System.Collections.Generic;

using Cryptography.ECDSA;

using Cysharp.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using StrattonStudios.EosioUnity;
using StrattonStudios.EosioUnity.Models;
using StrattonStudios.EosioUnity.Serialization;
using StrattonStudios.EosioUnity.Utilities;

namespace StrattonStudios.EosioUnity.Signing
{

    /// <summary>
    /// EOSIO Signing Request.
    /// </summary>
    public class SigningRequest
    {

        #region Constants

        // TODO: Upgrade to protocol version 3 with backwards compatibility to 2
        public const int PROTOCOL_VERSION = 2;
        public const string PLACEHOLDER_NAME = "............1";
        public const string PLACEHOLDER_PERMISSION = "............2";
        public static PermissionLevel PLACEHOLDER_PERMISSION_LEVEL = new PermissionLevel(PLACEHOLDER_NAME, PLACEHOLDER_PERMISSION);
        public const string PLACEHOLDER_PACKED = "0101000000000000000200000000000000";

        private const string REQ = "req";
        private const string FLAGS = "flags";
        private const string CALLBACK = "callback";
        private const string INFO = "info";
        private const string SIG = "sig";

        #endregion

        #region Static Initializers

        /// <summary>
        /// Create a new signing request.
        /// </summary>
        /// <param name="args">The signing request arguments</param>
        /// <param name="options">The signing request options</param>
        /// <returns>Returns the signing request</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static async UniTask<SigningRequest> Create(SigningRequestCreateArgs args, SigningRequestEncodingOptions options)
        {
            List<Action> actions = new List<Action>();
            if (args.action != null)
            {
                actions.Add(args.action);
            }
            else if (args.actions != null)
            {
                actions.AddRange(args.actions.GetActions());
            }
            else if (args.transaction != null)
            {
                actions.AddRange(args.transaction.Actions);
            }
            actions = actions.FindAll(action =>
            {
                return !action.Data.IsPacked();
            });
            var accounts = actions.ConvertAll(action =>
            {
                return action.Account.Value;
            });
            var abis = new Dictionary<string, Abi>();
            if (accounts.Count != 0)
            {
                var provider = options.abiProvider;
                if (provider == null)
                {
                    throw new System.ArgumentNullException("Missing abi provider");
                }
                for (int i = 0; i < accounts.Count; i++)
                {
                    abis[accounts[i]] = await provider.GetAbi(accounts[i]);
                }
            }
            return await Create(args, options, abis);
        }

        /// <summary>
        /// Synchronously create a new signing request.
        /// </summary>
        /// <remarks>
        /// Throws error If an un-encoded action with no ABI definition is encountered.
        /// </remarks>
        /// <param name="args">The signing request arguments</param>
        /// <param name="options">The signing request options</param>
        /// <param name="abis">The ABIs mapping</param>
        /// <returns>Returns the signing request</returns>
        /// <exception cref="System.InvalidOperationException"></exception>
        /// <exception cref="System.ArgumentException"></exception>
        public static async UniTask<SigningRequest> Create(SigningRequestCreateArgs args, SigningRequestEncodingOptions options, Dictionary<string, Abi> abis)
        {
            bool isIdentity = false;
            IESRRequest requestData;
            int version = 2;
            System.Action<Action> encode = (action) =>
            {
                if (action.Data.IsPacked())
                {
                    return;
                }
                var serializer = new AbiTypeWriter(null);
                action.Data.SetData(HexUtility.ToHexString(serializer.PackActionData(action, abis[action.Account.Value])));
            };

            // TODO: Implement multi-chain requests

            // The multi-chain requests require version 3 as it is not yet supported
            if (args.ChainId == null)
            {
                version = 3;
                throw new System.NotImplementedException("The protocol version 3 is not implemented");
            }

            // Set the request data
            //if (args.identityV3 != null)
            //{
            //    version = 3;
            //    requestData = args.identityV3;
            //    isIdentity = true;
            //}
            //else 
            if (args.identityV2 != null)
            {
                if (version == 3)
                {
                    throw new System.InvalidOperationException("Cannot use protocol version 3 with " + nameof(IdentityV2));
                }
                requestData = args.identityV2;
                isIdentity = true;
            }
            else if (args.action != null && args.actions == null && args.transaction == null)
            {
                requestData = args.action;
            }
            else if (args.actions != null && args.action == null && args.transaction == null)
            {
                if (args.actions.GetActions().Count == 1)
                {
                    requestData = args.actions.GetActions()[0];
                }
                else
                {
                    requestData = args.actions;
                }
            }
            else if (args.transaction != null && args.action == null && args.actions == null)
            {
                var tx = args.transaction;

                // Set the default values of the transaction if they're missing
                if (string.IsNullOrEmpty(tx.Expiration))
                {
                    tx.Expiration = TransactionHeader.DEFAULT_EXPIRATION;
                }
                if (tx.RefBlockNum == null || !tx.RefBlockNum.HasValue)
                {
                    tx.RefBlockNum = 0;
                }
                if (tx.RefBlockPrefix == null || !tx.RefBlockPrefix.HasValue)
                {
                    tx.RefBlockPrefix = 0;
                }
                if (tx.ContextFreeActions == null)
                {
                    tx.ContextFreeActions = new List<Action>();
                }
                if (tx.TransactionExtensions == null)
                {
                    tx.TransactionExtensions = new List<Extension>();
                }
                if (tx.DelaySec == null || !tx.DelaySec.HasValue)
                {
                    tx.DelaySec = 0;
                }
                if (tx.MaxNetUsageMs == null || !tx.MaxNetUsageMs.HasValue)
                {
                    tx.MaxNetUsageMs = 0;
                }
                if (tx.MaxNetUsageWords == null || !tx.MaxNetUsageWords.HasValue)
                {
                    tx.MaxNetUsageWords = 0;
                }
                if (tx.Actions == null)
                {
                    tx.Actions = new List<Action>();
                }

                // Iterate through actions and encode as needed
                tx.Actions.ForEach(encode);
                requestData = tx;
            }
            else
            {
                throw new System.ArgumentException("Invalid arguments: Must have exactly one of action, actions or transaction");
            }

            // Set the Chain ID for the request
            Chain chainId;
            if (args.ChainId == null)
            {

                // Use EOS if no chain is specified
                chainId = Chain.EOS;
            }
            else
            {
                chainId = args.ChainId;
            }

            // Set the request flags and callback
            var flag = RequestFlag.None;
            var callback = "";
            if (isIdentity)
            {
                flag.SetBackground(true);
            }
            if (args.broadcast != null && args.broadcast.HasValue)
            {
                flag.SetBroadcast(args.broadcast.Value);
            }
            if (args.callback != null)
            {
                callback = args.callback.Url;
                if (args.callback.Background != null && args.callback.Background.HasValue)
                {
                    flag.SetBackground(args.callback.Background.Value);
                }
            }

            // Add info pairs if there are any
            var info = new List<InfoPair>();
            if (args.info != null)
            {
                info = args.info;
            }

            // TODO: To be implemented for protocol version 3
            //if (args.chainIds != null && args.chainId == null)
            //{
            //    info.Add(InfoPair.Create(ChainId.CHAIN_IDS, JSRuntime.SerializeChains(args.chainIds.ToArray())));
            //}

            var request = new SigningRequest(options.abiProvider, options.zlib);
            request.SetCallback(callback);
            request.SetRequestFlag(flag);
            await request.SetRequest(requestData);
            request.SetInfoPairs(info);
            request.SetChainId(chainId.ToChainId());

            if (options.signatureProvider != null)
            {
                request.Sign(options.signatureProvider);
            }

            return request;
        }

        /// <summary>
        /// Creates an identity request.
        /// </summary>
        /// <param name="args">The identity request arguments</param>
        /// <param name="options">The signing request options</param>
        /// <returns>Returns the signing request for identity</returns>
        public static UniTask<SigningRequest> Identity(SigningRequestCreateIdentityArgs args, SigningRequestEncodingOptions options)
        {
            var createArgs = new SigningRequestCreateArgs();
            createArgs.info = args.info;

            // TODO: To be implemented for protocol version 3
            //createArgs.chainIds = args.chainIds;
            //createArgs.ChainId = args.ChainId;
            createArgs.ChainType = args.ChainType;
            createArgs.callback = args.callback;
            createArgs.broadcast = false;
            PermissionLevel permission = new PermissionLevel(
                args.account.Value ?? PLACEHOLDER_NAME,
                args.permission.Value ?? PLACEHOLDER_PERMISSION);

            // TODO: To be implemented for protocol version 3
            //if (!string.IsNullOrEmpty(args.scope))
            //{
            //    var permission = new PermissionLevel(args.account, args.permission);
            //    createArgs.identityV3 = new IdentityV3(args.scope, permission);
            //}
            //else
            //{
            if (permission.Actor.Value == PLACEHOLDER_NAME && permission.Permission.Value == PLACEHOLDER_PERMISSION)
            {
                permission = null;
            }
            createArgs.identityV2 = new IdentityV2(permission);
            //}
            return Create(createArgs, options, null);
        }

        public static SigningRequest FromTransaction(ChainId chainId, byte[] serializedTransaction, SigningRequestEncodingOptions options)
        {
            using (var buf = new System.IO.MemoryStream())
            {
                buf.WriteByte(2); // The header
                var id = chainId.ToVariant();
                if ((string)id[0] == "chain_alias")
                {
                    buf.WriteByte(0);
                    buf.WriteByte(System.Convert.ToByte((int)id[1]));
                }
                else
                {
                    buf.WriteByte(1);
                    byte[] bytes = HexUtility.FromHexString((string)id[1]);
                    buf.Write(bytes, 0, bytes.Length);
                }

                buf.WriteByte(2); // The transaction variant
                buf.Write((byte[])serializedTransaction, 0, ((byte[])serializedTransaction).Length);
                buf.WriteByte((byte)RequestFlag.Broadcast); // The request flags
                buf.WriteByte(0); // The request callback
                buf.WriteByte(0); // The info

                return FromData(buf.ToArray(), options);
            }
        }

        /// <summary>
        /// Creates a signing request from encoded `esr:` URI string.
        /// </summary>
        /// <param name="uri">The ESR encoded URI string</param>
        /// <param name="options">The signing request options</param>
        /// <returns>Returns the signing request object decoded from the ESR URI</returns>
        public static SigningRequest From(string uri, SigningRequestEncodingOptions options)
        {
            var request = new SigningRequest(options.abiProvider, options.zlib);
            return request.Load(uri);
        }

        /// <summary>
        /// Creates a signing request from binary data.
        /// </summary>
        /// <param name="data">The binary data</param>
        /// <param name="options">The signing request options</param>
        /// <returns>Returns the signing request object</returns>
        public static SigningRequest FromData(byte[] data, SigningRequestEncodingOptions options)
        {
            var request = new SigningRequest(options.abiProvider, options.zlib);
            return request.Load(data);
        }

        #endregion

        #region Fields

        /// <summary>
        /// The signing request version, currently version 2 is only supported.
        /// </summary>
        private int version;

        private IAbiProvider abiProvider;
        private IZlibProvider zlibProvider;

        private ChainId chainId;
        private IESRRequest request;
        private RequestFlag requestFlag;
        private string callback;
        private List<InfoPair> infoPairs;
        private Signature signature;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="SigningRequest"/>.
        /// </summary>
        /// <param name="esr"></param>
        public SigningRequest(IAbiProvider abiProvider, IZlibProvider zlib)
        {
            this.abiProvider = abiProvider;
            this.zlibProvider = zlib;
            this.chainId = new ChainId(Chain.EOS);
            this.requestFlag = RequestFlagExtensions.GetDefault();
            this.callback = "";
            this.infoPairs = new List<InfoPair>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Loads the signing request from the encoded URI.
        /// </summary>
        /// <param name="uri">The ESR encoded URI</param>
        /// <returns>Returns the decoded signing request</returns>
        /// <exception cref="EosioException"></exception>
        public SigningRequest Load(string uri)
        {
            string[] parts = uri.Split(':');
            string prefix = parts[0];
            string path = parts[1];

            if (!prefix.Equals("esr") && !prefix.Equals("web+esr"))
            {
                throw new EosioException("Only esr and web+esr schemes are supported");
            }

            if (path.StartsWith("//"))
            {
                path = path.Substring(2);
            }


            byte[] decoded = Base64EncodingUtility.FromBase64UrlSafe(path);

            return Load(decoded);
        }

        /// <summary>
        /// Loads the signing request from the binary data.
        /// </summary>
        /// <param name="data">The binary data</param>
        /// <returns>Returns the decoded signing request</returns>
        /// <exception cref="EosioException"></exception>
        private SigningRequest Load(byte[] data)
        {
            int header = data[0] & 0xff;
            int version = header & ~(1 << 7);
            if (version != PROTOCOL_VERSION)
                throw new EosioException("Unsupported protocol version");

            byte[] reqArray = ArrayUtility.CopyOfRange(data, 1, data.Length);
            if ((header & (1 << 7)) != 0)
            {
                try
                {
                    reqArray = this.zlibProvider.DecompressByteArray(reqArray);
                }
                catch (System.Exception e)
                {
                    throw new EosioException("Failed to decompress request: " + e.Message);
                }
            }

            var serializer = new AbiTypeReader(null);
            var result = serializer.DeserializeStructData("signing_request", reqArray, AbiData.GetCachedAbi());
            this.chainId = ChainId.FromVariant((List<object>)result[ChainId.CHAIN_ID]);
            this.request = ESRRequestFactory.FromVariant((List<object>)result[REQ]);
            this.requestFlag = (RequestFlag)(byte)result[FLAGS];
            this.callback = (string)result[CALLBACK];
            this.infoPairs = InfoPair.ListFromDeserialized((List<object>)result[INFO]);
            if (result.ContainsKey(SIG) && result[SIG] is Dictionary<string, object>)
            {
                this.signature = new Signature((Dictionary<string, object>)result[SIG]);
            }

            return this;
        }

        /// <summary>
        /// Sets the request, checks whether the provided request is action, actions, transaction or identity to call the corresponding methods.
        /// </summary>
        /// <param name="request">The request data</param>
        /// <exception cref="System.ArgumentException"></exception>
        public UniTask SetRequest(IESRRequest request)
        {
            if (request is Action)
            {
                return SetRequest(request as Action);
            }
            if (request is Actions)
            {
                return SetRequest(request as Actions);
            }
            if (request is Transaction)
            {
                return SetRequest(request as Transaction);
            }
            if (request is IdentityV2)
            {
                SetRequest(request as IdentityV2);
                return UniTask.CompletedTask;
            }
            if (request is IdentityV3)
            {
                SetRequest(request as IdentityV3);
                return UniTask.CompletedTask;
            }
            throw new System.ArgumentException("The request is invalid");
        }

        /// <summary>
        /// Sets the request action.
        /// </summary>
        /// <param name="action">The action</param>
        public async UniTask SetRequest(Action action)
        {
            if (!action.Data.IsPacked())
            {
                var abi = await this.abiProvider.GetAbi(action.Account.Value);
                var serializer = new AbiTypeWriter(null);
                action.Data.SetData(HexUtility.ToHexString(serializer.PackActionData(action, abi)));
            }

            this.request = action;
        }

        /// <summary>
        /// Sets the request actions.
        /// </summary>
        /// <param name="actions">The actions</param>
        public async UniTask SetRequest(Actions actions)
        {
            foreach (Action action in actions.GetActions())
            {
                if (!action.Data.IsPacked())
                {
                    var abi = await this.abiProvider.GetAbi(action.Account.Value);
                    var serializer = new AbiTypeWriter(null);
                    action.Data.SetData(HexUtility.ToHexString(serializer.PackActionData(action, abi)));
                }
            }

            this.request = actions;
        }

        /// <summary>
        /// Sets the request transaction.
        /// </summary>
        /// <param name="transaction">The transaction</param>
        public async UniTask SetRequest(Transaction transaction)
        {
            foreach (Action action in transaction.Actions)
            {
                if (!action.Data.IsPacked())
                {
                    var abi = await this.abiProvider.GetAbi(action.Account.Value);
                    var serializer = new AbiTypeWriter(null);
                    action.Data.SetData(HexUtility.ToHexString(serializer.PackActionData(action, abi)));
                }
            }

            foreach (Action action in transaction.ContextFreeActions)
            {
                if (!action.Data.IsPacked())
                {
                    var abi = await this.abiProvider.GetAbi(action.Account.Value);
                    var serializer = new AbiTypeWriter(null);
                    action.Data.SetData(HexUtility.ToHexString(serializer.PackActionData(action, abi)));
                }
            }

            this.request = transaction;
        }

        /// <summary>
        /// Sets the request identity.
        /// </summary>
        /// <param name="identity">The identity</param>
        public void SetRequest(IdentityV2 identity)
        {
            this.request = identity;
        }

        // TODO: To be implemented in protocol version 3
        /// <summary>
        /// Sets the request identity V3.
        /// </summary>
        /// <param name="identity"></param>
        //public void SetRequest(IdentityV3 identity)
        //{
        //    this.request = identity;
        //}

        /// <summary>
        /// Sets the request signature.
        /// </summary>
        /// <param name="signature">The signature</param>
        public void SetSignature(Signature signature)
        {
            this.signature = signature;
        }

        /// <summary>
        /// Whether this request is identity.
        /// </summary>
        /// <returns>Returns true if the request is an identity request, otherwise false</returns>
        public bool IsIdentity()
        {
            return this.request != null && (this.request is IdentityV2 || this.request is IdentityV3);
        }

        /// <summary>
        /// Gets the request identity.
        /// </summary>
        /// <returns>Returns the identity actor of the request</returns>
        public string GetIdentity()
        {
            if (IsIdentity() && ((IdentityV2)this.request).PermissionLevel != null)
            {
                AccountName accountName = ((IdentityV2)this.request).PermissionLevel.Actor;
                if (accountName == null)
                    return null;

                string actor = accountName.Value;
                return PLACEHOLDER_NAME.Equals(actor) ? null : actor;
            }

            if (IsIdentity() && ((IdentityV3)this.request).PermissionLevel != null)
            {
                AccountName accountName = ((IdentityV3)this.request).PermissionLevel.Actor;
                if (accountName == null)
                    return null;

                string actor = accountName.Value;
                return PLACEHOLDER_NAME.Equals(actor) ? null : actor;
            }

            return null;
        }

        /// <summary>
        /// Gets the identity permission name.
        /// </summary>
        /// <returns>Returns the identity permission name of the request</returns>
        public string GetIdentityPermission()
        {
            if (IsIdentity() && ((IdentityV2)this.request).PermissionLevel != null)
            {
                PermissionName permissionName = ((IdentityV2)this.request).PermissionLevel.Permission;
                if (permissionName == null)
                    return null;

                string permission = permissionName.Value;
                return PLACEHOLDER_NAME.Equals(permission) ? null : permission;
            }

            if (IsIdentity() && ((IdentityV3)this.request).PermissionLevel != null)
            {
                PermissionName permissionName = ((IdentityV3)this.request).PermissionLevel.Permission;
                if (permissionName == null)
                    return null;

                string permission = permissionName.Value;
                return PLACEHOLDER_NAME.Equals(permission) ? null : permission;
            }

            return null;
        }

        /// <summary>
        /// Whether has any signature provided.
        /// </summary>
        /// <returns>Return true if there are any signatures provided, otherwise false</returns>
        public bool HasSignature()
        {
            return this.signature != null;
        }

        /// <summary>
        /// Gets the signature.
        /// </summary>
        /// <returns>Returns the signatures</returns>
        public Signature GetSignature()
        {
            return this.signature;
        }

        /// <summary>
        /// Signs the request using the signature provider and sets the signature,
        /// </summary>
        /// <param name="signatureProvider">The signature provider</param>
        public void Sign(ISignatureProvider signatureProvider)
        {
            SetSignature(signatureProvider.Sign(GetSignatureDigestAsHex()));
        }

        /// <summary>
        /// Encodes the request to a ESR URI.
        /// </summary>
        /// <returns>Returns the ESR encoded URI</returns>
        public string Encode()
        {
            return Encode(true, true);
        }

        /// <summary>
        /// Encodes the request to a URI.
        /// </summary>
        /// <param name="compress">Whether to compress the data</param>
        /// <param name="slashes">Whether to use slashes after `esr:`</param>
        /// <returns>Returns the ESR encoded URI</returns>
        public string Encode(bool compress, bool slashes)
        {
            byte header = PROTOCOL_VERSION;
            byte[] data = GetData();
            byte[] sigData = GetSignatureData();
            byte[] toEncode = new byte[data.Length + sigData.Length];
            data.CopyTo(toEncode, 0);
            sigData.CopyTo(toEncode, data.Length);
            if (compress)
            {
                if (this.zlibProvider == null)
                {
                    throw new System.Exception("A Zlib provider is required for compressing");
                }
                byte[] compressed = this.zlibProvider.CompressByteArray(toEncode);
                if (toEncode.Length > compressed.Length)
                {
                    header |= 1 << 7;
                    toEncode = compressed;
                }
            }

            byte[] @out = new byte[1 + toEncode.Length];
            @out[0] = header;
            toEncode.CopyTo(@out, 1);
            string scheme = "esr:";
            if (slashes)
            {
                scheme += "//";
            }

            return scheme + Base64EncodingUtility.ToBase64UrlSafe(@out);
        }

        /// <summary>
        /// Resolves the actions.
        /// </summary>
        /// <returns>Returns the list of the resolved actions</returns>
        public async UniTask<List<Action>> ResolveActions()
        {
            return ResolveActions(await FetchAbis());
        }

        /// <summary>
        /// Resolves the actions using the signer.
        /// </summary>
        /// <param name="signer">The signer</param>
        /// <returns>Returns the list of the resolved actions</returns>
        public async UniTask<List<Action>> ResolveActions(PermissionLevel signer)
        {
            return ResolveActions(await FetchAbis(), signer);
        }

        /// <summary>
        /// Resolves the actions.
        /// </summary>
        /// <param name="abiMap">The ABI mappings</param>
        /// <returns>Returns the list of the resolved actions</returns>
        public List<Action> ResolveActions(Dictionary<string, Abi> abiMap)
        {
            return ResolveActions(abiMap, null);
        }

        /// <summary>
        /// Resolves the actions.
        /// </summary>
        /// <param name="abiMap">The ABI mappings</param>
        /// <param name="signer">The signer</param>
        /// <returns>Returns the list of the resolved actions</returns>
        public List<Action> ResolveActions(Dictionary<string, Abi> abiMap, PermissionLevel signer)
        {
            List<Action> rawActions = GetRawActions();
            List<Action> resolvedActions = new List<Action>();
            foreach (Action raw in rawActions)
            {
                if (!raw.Data.IsPacked())
                {
                    throw new EosioException("Cannot resolve an already resolved action");
                }
                Abi actionAbi = AbiData.GetCachedAbi();
                Action resolvedAction = new Action();
                if (!raw.IsIdentity())
                {
                    var abi = abiMap[raw.Account.Value];
                    if (abi == null)
                        throw new EosioException("Missing ABI definition for " + raw.Account.Value);
                    actionAbi = abi;
                }
                else
                {
                    actionAbi = AbiData.GetCachedAbi();
                }
                resolvedAction.Account = new AccountName(raw.Account.Value);
                resolvedAction.Name = new ActionName(raw.Name.Value);
                resolvedAction.Authorization = new List<PermissionLevel>(raw.Authorization);
                var serializer = new AbiTypeReader(null);
                resolvedAction.Data = new ActionData(serializer.UnpackActionData(resolvedAction, HexUtility.FromHexString(raw.Data.GetPackedData()), actionAbi));
                if (signer != null)
                {
                    foreach (var auth in resolvedAction.Authorization)
                    {
                        if (auth.Actor.Value == PLACEHOLDER_NAME)
                        {
                            auth.Actor.Value = signer.Actor.Value;
                        }
                        if (auth.Permission.Value == PLACEHOLDER_PERMISSION)
                        {
                            auth.Permission.Value = signer.Permission.Value;
                        }
                        if (auth.Permission.Value == PLACEHOLDER_NAME)
                        {
                            auth.Permission.Value = signer.Permission.Value;
                        }
                    }
                }
                // TODO: Update action data placeholders
                resolvedActions.Add(resolvedAction);
            }
            return resolvedActions;
        }

        /// <summary>
        /// Resolves the transaction.
        /// </summary>
        /// <param name="signer">The signer</param>
        /// <returns>Returns the resolved transaction</returns>
        public async UniTask<Transaction> ResolveTransaction(PermissionLevel signer)
        {
            return ResolveTransaction(await FetchAbis(), signer);
        }

        /// <summary>
        /// Resolves the transaction.
        /// </summary>
        /// <param name="signer">The signer</param>
        /// <param name="transactionContext">The transaction context</param>
        /// <returns>Returns the resolved transaction</returns>
        public async UniTask<Transaction> ResolveTransaction(PermissionLevel signer, TransactionContext transactionContext)
        {
            return ResolveTransaction(await FetchAbis(), signer, transactionContext);
        }

        /// <summary>
        /// Resolves the transaction.
        /// </summary>
        /// <param name="abiMap">The ABI mappings</param>
        /// <param name="signer">The signer</param>
        /// <returns>Returns the resolved transaction</returns>
        public Transaction ResolveTransaction(Dictionary<string, Abi> abiMap, PermissionLevel signer)
        {
            return ResolveTransaction(abiMap, signer, new TransactionContext());
        }

        /// <summary>
        /// Resolves the transaction.
        /// </summary>
        /// <param name="abiMap">The ABI mappings</param>
        /// <param name="signer">The signer</param>
        /// <param name="transactionContext">The transaction context</param>
        /// <returns>Returns the resolved transaction</returns>
        /// <exception cref="EosioException"></exception>
        public Transaction ResolveTransaction(Dictionary<string, Abi> abiMap, PermissionLevel signer, TransactionContext transactionContext)
        {
            Transaction transaction = GetRawTransaction();
            if (!(this.request is IdentityV2 || this.request is IdentityV3) && !transaction.HasTapos())
            {
                if (transactionContext.Expiration != null &&
                        transactionContext.RefBlockNum != null &&
                        transactionContext.RefBlockPrefix != null)
                {
                    transaction.Expiration = transactionContext.Expiration;
                    transaction.RefBlockNum = transactionContext.RefBlockNum;
                    transaction.RefBlockPrefix = transactionContext.RefBlockPrefix;
                }
                else if (transactionContext.BlockNum != null &&
                          transactionContext.RefBlockPrefix != null &&
                          transactionContext.Timestamp != null)
                {
                    transaction.Expiration = EosioTimeUtility.GetExpirationTime(transactionContext.Timestamp.Value + transactionContext.ExpireSeconds);
                    transaction.RefBlockNum = transactionContext.BlockNum;
                    transaction.RefBlockPrefix = transactionContext.RefBlockPrefix;
                }
                else
                {
                    throw new EosioException("Invalid transaction context, need either a reference block or explicit TAPoS values");
                }
            }
            List<Action> actions = ResolveActions(abiMap, signer);

            Transaction resolved = transaction.ShallowClone();
            resolved.Actions = actions;
            return resolved;
        }

        /// <summary>
        /// Resolves the signing request.
        /// </summary>
        /// <param name="signer">The signer</param>
        /// <param name="transactionContext">The transaction context</param>
        /// <returns>Returns the resolved signing request</returns>
        public async UniTask<ResolvedSigningRequest> Resolve(PermissionLevel signer, TransactionContext transactionContext)
        {
            return await Resolve(await FetchAbis(), signer, transactionContext);
        }

        /// <summary>
        /// Resolves the signing request.
        /// </summary>
        /// <param name="abiMap">The ABI mappings</param>
        /// <param name="signer">The signer</param>
        /// <param name="transactionContext">The transaction context</param>
        /// <returns>Returns the resolved signing request</returns>
        public async UniTask<ResolvedSigningRequest> Resolve(Dictionary<string, Abi> abiMap, PermissionLevel signer, TransactionContext transactionContext)
        {
            var serializer = new AbiTypeWriter(null);
            Transaction transaction = await ResolveTransaction(signer, transactionContext);
            foreach (Action action in transaction.Actions)
            {
                Abi abi;
                if (string.IsNullOrEmpty(action.Account.Value))
                {
                    abi = AbiData.GetCachedAbi();
                }
                else
                {
                    if (abiMap.ContainsKey(action.Account.Value))
                    {
                        abi = abiMap[action.Account.Value];
                    }
                    else
                    {
                        abi = await this.abiProvider.GetAbi(action.Account.Value);
                    }
                }
                action.Data.SetData(HexUtility.ToHexString(serializer.PackActionData(action, abi)));
            }

            byte[] serializedTransaction = await serializer.SerializeTransaction(transaction, abiMap);
            return new ResolvedSigningRequest(this, signer, transaction, serializedTransaction);
        }

        /// <summary>
        /// Gets the raw transaction.
        /// </summary>
        /// <returns>Returns the raw transaction</returns>
        /// <exception cref="EosioException"></exception>
        public Transaction GetRawTransaction()
        {
            if (this.request == null)
                throw new EosioException("Cannot get raw transaction, request is not set");

            if (this.request is Transaction)
                return (Transaction)this.request;

            Transaction transaction = new Transaction();
            transaction.Actions = GetRawActions();
            return transaction;
        }

        /// <summary>
        /// Gets the raw actions.
        /// </summary>
        /// <returns>Returns the raw actions</returns>
        public List<Action> GetRawActions()
        {
            if (this.request is IdentityV2)
            {
                var identity = this.request as IdentityV2;
                Action action = new Action();
                action.Account = new AccountName("");
                action.Name = new ActionName(IdentityV3.IDENTITY);
                PermissionLevel permissionLevel = identity.PermissionLevel;
                if (permissionLevel == null || permissionLevel.Actor == null ||
                        string.IsNullOrEmpty(permissionLevel.Actor.Value) ||
                        permissionLevel.Permission == null ||
                        string.IsNullOrEmpty(permissionLevel.Permission.Value))
                {
                    action.AddAuthorization(SigningRequest.PLACEHOLDER_PERMISSION_LEVEL);
                    action.Data = new ActionData(SigningRequest.PLACEHOLDER_PACKED);
                }
                else
                {
                    var serializer = new AbiTypeWriter(null);
                    var serializedIdentity = serializer.SerializeStructData("identity", identity, AbiData.GetCachedAbi());
                    action.AddAuthorization(identity.PermissionLevel);
                    action.Data = new ActionData(HexUtility.ToHexString(serializedIdentity));
                }
                return new List<Action>() { action };
            }

            // TODO: To be implemented for protocol version 3
            //if (this.request is IdentityV3)
            //    return new List<Action>() { JSRuntime.IdentityToAction((IdentityV3)this.request) };

            return this.request.GetRawActions();
        }

        /// <summary>
        /// Gets the list of required ABIs.
        /// </summary>
        /// <returns>Returns the list of required ABIs using their smart contract account name</returns>
        public List<string> GetRequiredAbis()
        {
            List<string> accounts = new List<string>();
            foreach (Action action in GetRawActions())
            {
                if (!action.IsIdentity())
                    accounts.Add(action.Account.Value);
            }

            return accounts;
        }

        /// <summary>
        /// Fetches the required ABIs.
        /// </summary>
        /// <returns>Returns ABI mappings using their respective account name</returns>
        public async UniTask<Dictionary<string, Abi>> FetchAbis()
        {
            var abiMap = new Dictionary<string, Abi>();
            foreach (string accountName in GetRequiredAbis())
            {
                abiMap[accountName] = await this.abiProvider.GetAbi(accountName);
            }

            return abiMap;
        }

        /// <summary>
        /// Gets the chain ID.
        /// </summary>
        /// <returns>Returns the chain ID</returns>
        public ChainId GetChainId()
        {
            return this.chainId;
        }

        /// <summary>
        /// Sets the chain ID.
        /// </summary>
        /// <param name="chainId">The chain ID</param>
        public void SetChainId(ChainId chainId)
        {
            this.chainId = chainId;
        }

        /// <summary>
        /// Gets the signing request data.
        /// </summary>
        /// <returns>Returns the signing request data in binary</returns>
        public byte[] GetData()
        {
            var serializer = new AbiTypeWriter(null);
            return serializer.SerializeStructData("signing_request", ToDictionary(), AbiData.GetCachedAbi());
        }

        /// <summary>
        /// Gets the signature data.
        /// </summary>
        /// <returns>Returns the signature data</returns>
        public byte[] GetSignatureData()
        {
            if (this.signature == null)
                return new byte[0];

            var serializer = new AbiTypeWriter(null);
            return serializer.SerializeStructData("request_signature", this.signature.ToDictionary(), AbiData.GetCachedAbi());
        }

        /// <summary>
        /// Gets the signature digest as hexadecimal string.
        /// </summary>
        /// <returns>Returns the signature digest as hexadecimal string</returns>
        public string GetSignatureDigestAsHex()
        {
            // Sequence: `protocol version + utf8 "request"`
            var bytes = new List<byte> { PROTOCOL_VERSION, 0x72, 0x65, 0x71, 0x75, 0x65, 0x73, 0x74 };
            bytes.AddRange(GetData());
            return HexUtility.ToHexString(Sha256Manager.GetHash(bytes.ToArray()));
        }

        /// <summary>
        /// Gets the request.
        /// </summary>
        /// <returns>Returns the request</returns>
        public IESRRequest GetRequest()
        {
            return this.request;
        }

        /// <summary>
        /// Gets the callback.
        /// </summary>
        /// <returns>Returns the callback</returns>
        public string GetCallback()
        {
            return this.callback;
        }

        /// <summary>
        /// Sets the callback.
        /// </summary>
        /// <param name="callback">The callback</param>
        public void SetCallback(string callback)
        {
            this.callback = callback;
        }

        /// <summary>
        /// Gets the request flag.
        /// </summary>
        /// <returns>Returns the request flag</returns>
        public RequestFlag GetRequestFlag()
        {
            return this.requestFlag;
        }

        /// <summary>
        /// Sets the request flag.
        /// </summary>
        /// <param name="requestFlag">The request flag</param>
        public void SetRequestFlag(RequestFlag requestFlag)
        {
            this.requestFlag = requestFlag;
        }

        /// <summary>
        /// Sets the info pair.
        /// </summary>
        /// <param name="key">The info key</param>
        /// <param name="value">The info value in binary format</param>
        /// <exception cref="EosioException"></exception>
        public void SetInfoKey(string key, byte[] value)
        {
            if (key == null)
                throw new EosioException("Key cannot be null");

            string hexValue;
            hexValue = HexUtility.ToHexString(value);
            foreach (InfoPair pair in this.infoPairs)
            {
                if (key.Equals(pair.GetKey()))
                {
                    pair.SetHexValue(hexValue);
                    return;
                }
            }

            this.infoPairs.Add(new InfoPair(key, hexValue));
        }

        public object GetInfoKey(string key, string typeName, Abi abi)
        {
            if (key == null)
            {
                throw new EosioException("Key cannot be null");
            }

            var pair = this.infoPairs.Find(infoPair => { return infoPair.GetKey() == key; });
            if (pair == null)
            {
                return null;
            }
            var serializer = new AbiTypeReader(null);
            return serializer.DeserializeType(typeName, pair.GetHexValue(), abi);
            //return serializer.DeserializeStructData(typeName, pair.GetHexValue(), abi);
        }

        /// <summary>
        /// Gets the info pairs.
        /// </summary>
        /// <returns>Returns the list of all info pairs</returns>
        public List<InfoPair> GetInfoPairs()
        {
            return this.infoPairs;
        }

        /// <summary>
        /// Gets the raw info pairs as dictionary.
        /// </summary>
        /// <returns>Returns the raw info pairs</returns>
        public Dictionary<string, byte[]> GetRawInfo()
        {
            Dictionary<string, byte[]> rawInfo = new Dictionary<string, byte[]>();
            foreach (InfoPair pair in this.infoPairs)
                rawInfo.Add(pair.GetKey(), pair.GetBytesValue());

            return rawInfo;
        }

        /// <summary>
        /// Gets info pairs as dictionary.
        /// </summary>
        /// <returns>Returns info pairs as dictionary</returns>
        public Dictionary<string, string> GetInfo()
        {
            Dictionary<string, string> info = new Dictionary<string, string>();
            foreach (InfoPair pair in this.infoPairs)
                info.Add(pair.GetKey(), pair.GetStringValue());

            return info;
        }

        /// <summary>
        /// Adds the info pair.
        /// </summary>
        /// <param name="infoPair">The info pair to add</param>
        public void AddInfoPair(InfoPair infoPair)
        {
            this.infoPairs.Add(infoPair);
        }

        /// <summary>
        /// Sets the info pairs.
        /// </summary>
        /// <param name="infoPairs">The info pairs to set</param>
        public void SetInfoPairs(List<InfoPair> infoPairs)
        {
            this.infoPairs = infoPairs;
        }

        public Dictionary<string, object> ToDictionary()
        {
            Dictionary<string, object> toEncode = new Dictionary<string, object>();
            toEncode.Add(ChainId.CHAIN_ID, this.chainId.ToVariant());
            toEncode.Add(REQ, this.request.ToVariant());
            toEncode.Add(FLAGS, this.requestFlag.GetFlagValue());
            toEncode.Add(CALLBACK, this.callback);

            List<object> info = new List<object>();
            foreach (InfoPair pair in this.infoPairs)
                info.Add(pair.ToDictionary());

            toEncode.Add(INFO, info);

            return toEncode;
        }

        /// <summary>
        /// Converts this signing request to JSON.
        /// </summary>
        /// <returns>Returns the signing request in JSON string format</returns>
        public string ToDataJSON()
        {
            return JsonConvert.SerializeObject(ToDictionary());
        }

        /// <summary>
        /// Creates a copy of this signing request.
        /// </summary>
        /// <returns>Returns a copy of this signing request</returns>
        public SigningRequest Copy()
        {
            SigningRequest copy = new SigningRequest(this.abiProvider, this.zlibProvider);
            copy.Load(Encode());
            return copy;
        }

        #endregion

    }

}