using System.Collections;
using System.Collections.Generic;

using Cysharp.Threading.Tasks;

using Newtonsoft.Json.Linq;

using StrattonStudios.EosioUnity;
using StrattonStudios.EosioUnity.Models;

using UnityEngine;
using UnityEngine.Networking;

namespace StrattonStudios.WcwUnity
{

    public class WcwSigningAPI
    {

        protected readonly string waxSigningURL;
        protected readonly string waxAutoSigningURL;

        protected IWaxEventSource waxEventSource;

        protected LoginResponse user;
        protected List<WhitelistedContract> whitelistedContracts = new List<WhitelistedContract>();

        public WcwSigningAPI(IWaxEventSource waxEventSource, string waxSigningURL, string waxAutoSigningURL)
        {
            this.waxEventSource = waxEventSource;

            this.waxSigningURL = waxSigningURL;
            this.waxAutoSigningURL = waxAutoSigningURL;
        }

        public virtual async UniTask<LoginResponse> Login()
        {
            if (this.user == null)
            {
                await LoginViaWindow();
            }

            if (this.user != null)
            {
                return this.user;
            }

            throw new WaxLoginException("Login Failed");
        }

        public virtual async UniTask<bool> TryAutoLogin()
        {
            if (this.user != null)
            {
                return true;
            }

            try
            {
                await LoginViaEndpoint();

                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public virtual void PrepareTransaction(Transaction transaction)
        {
            if (!CanAutoSign(transaction))
            {
                this.waxEventSource.PrepareSign($"{this.waxSigningURL}/cloud-wallet/signing/");
                // TODO: Open popup window
                // await window.open(url, "WaxPopup", "height=800,width=600");
            }
        }

        public virtual async UniTask<SigningResponse> Signing(Transaction transaction, byte[] serializedTransaction, bool noModify = false)
        {
            if (CanAutoSign(transaction))
            {
                try
                {
                    return await SignViaEndpoint(serializedTransaction, noModify);
                }
                catch
                {
                    // Handled by continuing
                }
            }

            return await SignViaWindow(serializedTransaction, noModify);
        }

        public virtual async UniTask<bool> LoginViaWindow()
        {
            var response = await this.waxEventSource.Login($"{this.waxSigningURL}/cloud-wallet/login/");
            return ReceiveLogin(response);
        }

        public virtual async UniTask<bool> LoginViaEndpoint()
        {
            var request = UnityWebRequest.Get($"{this.waxAutoSigningURL}login");

            await request.SendWebRequest();

#if UNITY_2020_1_OR_NEWER
            if (request.result != UnityWebRequest.Result.Success)
#else
            if (request.isHttpError || request.isNetworkError)
#endif
            {
                throw new WaxLoginException(string.Format("Login Endpoint Error {0} {1}", request.responseCode, request.error));
            }

            // TODO: Should be removed, Debug only
            //Debug.Log(request.downloadHandler.text);
            var data = JsonUtility.FromJson<PushTransactionResponse>(request.downloadHandler.text);
            if (data == null || (data.processed != null && data.processed.except != null))
            {
                throw new WaxLoginException(string.Format("Error returned from login endpoint {0}", request.downloadHandler.text));
            }

            //TODO: ReceiveLogin();
            return true;
        }

        public virtual async UniTask<SigningResponse> SignViaWindow(byte[] serializedTransaction, bool noModify = false)
        {
            var response = await this.waxEventSource.Sign($"{this.waxSigningURL}/cloud-wallet/signing/", serializedTransaction, noModify);
            return ReceiveSignatures(response);
        }

        public virtual async UniTask<SigningResponse> SignViaEndpoint(byte[] serializedTransaction, bool noModify = false)
        {
            var postData = new SignViaEndpointRequest();
            postData.freeBandwidth = !noModify;
            postData.transaction = serializedTransaction;
            var request = UnityWebRequest.Post(string.Format("{0}signing", this.waxAutoSigningURL), JsonUtility.ToJson(postData));
            request.SetRequestHeader("Content-Type", "application/json");

            await request.SendWebRequest();

#if UNITY_2020_1_OR_NEWER
            if (request.result != UnityWebRequest.Result.Success)
#else
            if (request.isHttpError || request.isNetworkError)
#endif
            {
                this.whitelistedContracts.Clear();

                throw new WaxSigningException(string.Format("Signing Endpoint Error {0} {1}", request.responseCode, request.error));
            }

            // TODO: Debug only
            Debug.Log(request.downloadHandler.text);
            var data = JsonUtility.FromJson<PushTransactionResponse>(request.downloadHandler.text);
            if (data == null || (data.processed != null && data.processed.except != null))
            {
                throw new WaxLoginException(string.Format("Error returned from signing endpoint {0}", request.downloadHandler.text));
            }

            // TODO: receiveSignatures
            return null;
        }

        protected virtual bool CanAutoSign(Transaction transaction)
        {
            return transaction.Actions.Find(action => !IsWhitelisted(action)) == null;
        }

        protected virtual bool IsWhitelisted(Action action)
        {
            return !!(this.whitelistedContracts != null && this.whitelistedContracts.Find(w =>
            {
                if (w.Contract == action.Account.Value)
                {
                    if (action.Account.Value == "eosio.token" && action.Account.Value == "transfer")
                    {
                        var selfDelegatedBandwidth = action.Data.GetData();
                        return w.Recipients.Contains((string)selfDelegatedBandwidth["to"]);
                    }

                    return true;
                }

                return false;
            }) != null);
        }

        protected virtual bool ReceiveLogin(LoginResponse response)
        {
            if (!response.verified)
            {
                throw new System.Exception("User declined to share their user account");
            }
            if (string.IsNullOrEmpty(response.userAccount) || response.pubKeys == null)
            {
                throw new System.Exception("User does not have a blockchain account");
            }

            this.user = response;

            if (response.whitelistedContracts == null)
            {
                this.whitelistedContracts = new List<WhitelistedContract>();
            }
            else
            {
                this.whitelistedContracts = new List<WhitelistedContract>(response.whitelistedContracts);
            }

            return true;
        }

        public virtual SigningResponse ReceiveSignatures(SigningResponse response)
        {
            if (!response.verified || response.signatures == null)
            {
                throw new System.Exception("User declined to sign the transaction");
            }

            return response;
        }

    }

    [System.Serializable]
    public class SignViaEndpointRequest
    {

        public bool freeBandwidth;
        public byte[] transaction;

    }

}
