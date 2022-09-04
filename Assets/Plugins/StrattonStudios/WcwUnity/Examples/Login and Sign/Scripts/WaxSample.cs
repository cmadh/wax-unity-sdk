using System.Collections;
using System.Collections.Generic;

using StrattonStudios.EosioUnity;
using StrattonStudios.EosioUnity.Api;

using UnityEngine;

namespace StrattonStudios.WcwUnity.Samples
{

    public class WaxSample : MonoBehaviour
    {

        public string identifier = "wax.dat";

        [Header("Action Details")]
        public string from = "teststrasatr";
        public string to = "stratton.gm";
        public string quantity = "0.00001000 WAX";
        public string memo = "h!";
        public string[] assetIds = new string[] { "1099709961920" };

        protected WcwClient wcw;

        private void Start()
        {
            var config = new EosioClientConfig();
            config.HttpEndpoint = "https://wax.greymass.com";
            //config.ChainId = Chain.WAX.GetId();
            config.BlocksBehind = 3;
            config.ExpireSeconds = 30;

            IWaxEventSource eventSource = null;

            // Vuplex
            //eventSource = VuplexWebViewController.Instance;

            // Embedded Browser (ZF)
            //eventSource = ZFBrowserController.Instance;

            // Unity Web Browser
            //eventSource = UnityWebBrowserController.Instance;

            // WebGL
            //eventSource = WaxWebGLController.Instance;

            this.wcw = new WcwClient(eventSource, config, null, null, null, tryAutoLogin: false, freeBandwidth: false);
        }

        public async void Login()
        {
            await this.wcw.Login();
            this.from = this.wcw.UserAccount;

            Debug.Log("Logged in successfully");
        }

        public async void Sign()
        {
            if (this.wcw.Api == null)
            {
                Debug.LogWarning("Login first then use transact method");
                return;
            }
            var action = new Action()
            {
                Account = new AccountName("atomicassets"),
                Authorization = new List<PermissionLevel>()
                {
                    new PermissionLevel() {
                        Actor = new AccountName(this.wcw.UserAccount),
                        Permission= new PermissionName("active")
                    }
                },
                Name = new ActionName("transfer"),
                Data = new ActionData(new Dictionary<string, object>()
                {
                    { "from", this.from},
                    { "to", this.to },
                    { "asset_ids", this.assetIds },
                    { "memo", this.memo }
                })
            };

            var tx = new Transaction();
            tx.Actions = new List<Action>() { action };
            await this.wcw.Api.CreateTransaction(tx);

            Debug.Log("Transaction Complete");
        }

    }

}