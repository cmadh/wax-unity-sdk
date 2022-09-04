using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Cysharp.Threading.Tasks;

using Newtonsoft.Json;

using StrattonStudios.EosioUnity;

using UnityEngine;

namespace StrattonStudios.WcwUnity
{

    /// <summary>
    /// The WAX Cloud Wallet Client.
    /// </summary>
    /// <remarks>
    /// This is the main facade of the WAX Cloud Wallet API.
    /// </remarks>
    public class WcwClient
    {

        public delegate void VerifyTxHandler(LoginResponse user, Transaction originalTx, Transaction augmentedTx);

        /// <summary>
        /// The default signing URL.
        /// </summary>
        public const string DefaultSigningURL = "https://all-access.wax.io";

        /// <summary>
        /// The default auto signing URL.
        /// </summary>
        public const string DefaultAutoSigningURL = "https://api-idm.wax.io/v1/accounts/auto-accept/";

        protected EosioClient api;
        protected LoginResponse user;

        protected WcwSigningAPI signingAPI;

        protected EosioClientConfig eosConfigurator;

        protected IEosioSignProvider apiSigner;

        protected string waxSigningURL;
        protected string waxAutoSigningURL;
        protected bool freeBandwidth;

        protected VerifyTxHandler verifyTx;

        /// <summary>
        /// Gets the current logged in user's account.
        /// </summary>
        public virtual string UserAccount
        {
            get
            {
                if (this.user != null)
                {
                    return this.user.userAccount;
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the current logged in user's public keys.
        /// </summary>
        public virtual string[] PubKeys
        {
            get
            {
                if (this.user != null)
                {
                    return this.user.pubKeys;
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the EOS Client initialized after logging in.
        /// </summary>
        public virtual EosioClient Api
        {
            get
            {
                return this.api;
            }
        }

        /// <summary>
        /// Creates a new instance of <see cref="WcwClient"/>.
        /// </summary>
        /// <param name="waxEventSource">The event source to use for logging in and signing</param>
        /// <param name="eosConfigurator">The EOS config</param>
        /// <param name="userAccount">(Optional) The user's account</param>
        /// <param name="pubKeys">(Optional) The user's public keys</param>
        /// <param name="apiSigner">(Optional) A custom signature provider</param>
        /// <param name="waxSigningURL">(Optional) A custom WAX signing URL</param>
        /// <param name="waxAutoSigningURL">(Optional) A custom WAX auto signing URL</param>
        /// <param name="tryAutoLogin">(Optional) Whether to try auto login</param>
        /// <param name="freeBandwidth">(Optional) Whether to use free bandwidth</param>
        /// <param name="verifyTx">(Optional) Custom delegate for verifying the transaction</param>
        public WcwClient(IWaxEventSource waxEventSource, EosioClientConfig eosConfigurator, string userAccount, string[] pubKeys, IEosioSignProvider apiSigner, string waxSigningURL = DefaultSigningURL, string waxAutoSigningURL = DefaultAutoSigningURL, bool tryAutoLogin = true, bool freeBandwidth = true, VerifyTxHandler verifyTx = null)
        {
            if (verifyTx == null)
            {
                verifyTx = DefaultTxVerifier;
            }
            this.apiSigner = apiSigner;
            this.waxSigningURL = waxSigningURL;
            this.waxAutoSigningURL = waxAutoSigningURL;
            this.signingAPI = new WcwSigningAPI(waxEventSource, this.waxSigningURL, this.waxAutoSigningURL);
            this.freeBandwidth = freeBandwidth;
            this.verifyTx = verifyTx;

            this.eosConfigurator = eosConfigurator;

            InternalLogin(userAccount, pubKeys, tryAutoLogin);
        }

        protected virtual async void InternalLogin(string userAccount, string[] pubKeys, bool tryAutoLogin = true)
        {
            if (!string.IsNullOrEmpty(userAccount) && pubKeys != null)
            {
                // login from constructor
                ReceiveLogin(new LoginResponse(true, userAccount, pubKeys, false, null));
            }
            else
            {
                // try to auto-login via endpoint
                if (tryAutoLogin)
                {
                    await this.signingAPI.TryAutoLogin();
                    ReceiveLogin(await this.signingAPI.Login());
                }
            }
        }

        /// <summary>
        /// Login the user.
        /// </summary>
        /// <returns>Returns the logged in user account.</returns>
        public virtual async UniTask<string> Login()
        {
            if (this.user == null)
            {
                ReceiveLogin(await this.signingAPI.Login());
            }

            return this.user.userAccount;
        }

        /// <summary>
        /// Whether the auto login is available or not.
        /// </summary>
        /// <returns>Returns true if auto login is available otherwise false</returns>
        public virtual async UniTask<bool> IsAutoLoginAvailable()
        {
            if (this.user != null)
            {
                return true;
            }
            else if (await this.signingAPI.TryAutoLogin())
            {
                ReceiveLogin(await this.signingAPI.Login());

                return true;
            }

            return false;
        }

        protected virtual void ReceiveLogin(LoginResponse data)
        {
            this.user = data;

            var signatureProvider = new SignatureProvider(this);

            this.eosConfigurator.SignProvider = signatureProvider;
            this.api = new EosioClient(this.eosConfigurator);
            this.api.CreatingTransaction += OnCreatingTransaction;
        }

        protected virtual void OnCreatingTransaction(object sender, TransactionEventArgs e)
        {
            this.signingAPI.PrepareTransaction(e.Transaction);
        }

        protected virtual void DefaultTxVerifier(LoginResponse user, Transaction originalTx, Transaction augmentedTx)
        {
            var originalActions = originalTx.Actions;
            var augmentedActions = augmentedTx.Actions;

            var serializeOriginal = new Actions(originalActions);
            var serializeAugmented = new Actions(augmentedActions.Skip(augmentedActions.Count - originalActions.Count).ToList());

            if (serializeOriginal.ToJSON() != serializeAugmented.ToJSON())
            {
                throw new System.Exception($"Augmented transaction actions has modified actions from the original.\nOriginal: {serializeOriginal.ToJSON()}\nAugmented: {new Actions(augmentedActions).ToJSON()}");
            }

            foreach (var extraAction in augmentedActions.Take(augmentedActions.Count - originalActions.Count))
            {
                var userAuthedAction = extraAction.Authorization.Find(auth =>
                {
                    return auth.Actor.Value == user.userAccount;
                });

                if (userAuthedAction != null)
                {
                    throw new System.Exception($"Augmented transaction actions has an extra action from the original authorizing the user.\nOriginal: {serializeOriginal.ToJSON()}\nAugmented: {new Actions(augmentedActions).ToJSON()}");
                }
            }
        }

        public class SignatureProvider : IEosioSignProvider
        {

            protected WcwClient wax;

            public SignatureProvider(WcwClient wax)
            {
                this.wax = wax;
            }

            public async UniTask<IEnumerable<string>> GetAvailableKeys()
            {
                var allKeys = new List<string>();
                if (this.wax.apiSigner != null)
                {
                    var availableKeys = await this.wax.apiSigner.GetAvailableKeys();
                    if (availableKeys != null)
                    {
                        allKeys.AddRange(availableKeys);
                    }
                }
                allKeys.AddRange(this.wax.user.pubKeys);
                return allKeys.ToArray();

            }

            public async UniTask<IEnumerable<string>> Sign(string chainId, IEnumerable<string> requiredKeys, byte[] serializedTransaction, IEnumerable<string> abiNames = null)
            {
                var originalTx = await this.wax.api.AbiReader.DeserializePackedTransaction(serializedTransaction);

                var response = await this.wax.signingAPI.Signing(originalTx, serializedTransaction, !this.wax.freeBandwidth);

                var augmentedTx = await this.wax.api.AbiReader.DeserializePackedTransaction(response.serializedTransaction);

                this.wax.verifyTx(this.wax.user, originalTx, augmentedTx);

                var result = new List<string>(response.signatures);
                if (this.wax.apiSigner != null)
                {
                    result.AddRange(await this.wax.apiSigner.Sign(chainId, requiredKeys, response.serializedTransaction, abiNames));
                }

                return result;
            }

        }

    }

}