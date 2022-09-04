using System.Collections;
using System.Collections.Generic;

using StrattonStudios.AnchorLinkUnity.Transports.UGUI;

using StrattonStudios.EosioUnity;

using UnityEngine;

namespace StrattonStudios.AnchorLinkUnity.Samples
{

    public class AnchorLinkExample : MonoBehaviour
    {

        /// <summary>
        /// This can be your App's identifier
        /// </summary>
        public string identifier = "example";
        //public string chainId = Chain.WAX.GetId();
        public ChainType chainType = ChainType.Mainnet;

        [Tooltip("The endpoint should not have trailing slash, otherwise the API calls will fail")]
        public string nodeUrl = "https://wax.greymass.com";

        [Header("Action Details")]
        public string from = "teststrasatr";
        public string to = "stratton.gm";
        public string quantity = "0.00001000 WAX";
        public string memo = "h!";

        public string password = LinkSecureStorage.RandomString(12);

        protected LinkOptions options;
        protected Link link;

        protected LinkSession session;


        private void Start()
        {
            this.options = new LinkOptions();

            // Uses an encrypted secure storage that wraps around any storage provider
            this.options.Storage = new LinkSecureStorage(new LinkPlayerPrefsStorage(), this.password);
            this.options.Transport = new LinkUnityUGUITransport();

            // Remove the trailing slash if there are any
            if (this.nodeUrl.EndsWith('/'))
            {
                this.nodeUrl = this.nodeUrl.Remove(this.nodeUrl.Length - 1);
            }
            this.options.Chain = new LinkChainConfig()
            {
                //Chain = Chain.FromChainId(this.chainId),
                ChainType = this.chainType,
                NodeUrl = this.nodeUrl
            };
            this.link = new Link(this.options);
        }

        /// <summary>
        /// This method tries to restore the saved session and if it does not exists, opens the login dialog again to login.
        /// </summary>
        public async void Login()
        {
            this.session = await this.link.RestoreSession(this.identifier, null);
            if (this.session == null)
            {
                var result = await this.link.Login(this.identifier);
                this.session = result.session;
            }
            else
            {
                Debug.Log("Restored session");
            }
            Debug.LogFormat("Logged in as: {0}", this.session.Auth.Actor);
        }

        /// <summary>
        /// If the user is not logged in, it is a once-off transaction otherwise uses the current logged in session for transaction.
        /// </summary>
        public async void Transact()
        {

            // Use the logged in acconut if available
            var account = this.session != null ? this.session.Auth.Actor.Value : this.from;

            var action = new Action()
            {
                Account = new AccountName("eosio.token"),
                Authorization = new List<PermissionLevel>()
                {
                    new PermissionLevel() {
                        Actor = new AccountName(account),
                        Permission= new PermissionName("active")
                    }
                },
                Name = new ActionName("transfer"),
                Data = new ActionData(new Dictionary<string, object>()
                {
                    { "from", account },
                    { "to", this.to },
                    { "quantity", this.quantity },
                    { "memo", this.memo }
                })
            };
            var args = new TransactArgs();
            args.action = action;
            var opts = new TransactOptions();
            if (this.session != null)
            {
                await this.session.Transact(args, opts);
            }
            else
            {
                await this.link.Transact(args, opts, null);
            }
        }

    }

}