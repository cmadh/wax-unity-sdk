using System.Collections.Generic;
using System.Text;

using Cysharp.Threading.Tasks;

using Newtonsoft.Json;

using StrattonStudios.EosioUnity.Models;

using UnityEngine;
using UnityEngine.Networking;

namespace StrattonStudios.EosioUnity.Api
{

    /// <summary>
    /// The EOS History API provides methods for interacting with the blockchain get historical information.
    /// </summary>
    public class EosioHistoryHttpApi : EosioHttpApiBase
    {

        #region Constants

        public const string Endpoint = "history";

        #endregion

        #region Properties

        public override string BaseUrl
        {
            get => base.BaseUrl;
            set
            {
                base.BaseUrl = value;
                if (!string.IsNullOrEmpty(base.BaseUrl) && !base.BaseUrl.EndsWith(Endpoint))
                {
                    this.baseUrl += "/" + Endpoint;
                }
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new instance of <see cref="EosioHistoryHttpApi"/>.
        /// </summary>
        /// <param name="baseUrl">The base URL endpoint for the blockchain</param>
        public EosioHistoryHttpApi(string baseUrl) : base(baseUrl)
        {
        }

        #endregion

        #region Public Methods

        public UniTask<GetActionsResponse> GetActions(GetActionsRequest data)
        {
            const string endpoint = "get_actions";
            return MakePostJsonRequest<GetActionsResponse>(endpoint, data);
        }

        public UniTask<GetTransactionResponse> GetTransaction(GetTransactionRequest data)
        {
            const string endpoint = "get_transaction";
            return MakePostJsonRequest<GetTransactionResponse>(endpoint, data);
        }

        public UniTask<GetKeyAccountsResponse> GetKeyAccounts(GetKeyAccountsRequest data)
        {
            const string endpoint = "get_key_accounts";
            return MakePostJsonRequest<GetKeyAccountsResponse>(endpoint, data);
        }

        public UniTask<GetControlledAccountsResponse> GetControlledAccounts(GetControlledAccountsRequest data)
        {
            const string endpoint = "get_controlled_accounts";
            return MakePostJsonRequest<GetControlledAccountsResponse>(endpoint, data);
        }

        #endregion

    }

}
