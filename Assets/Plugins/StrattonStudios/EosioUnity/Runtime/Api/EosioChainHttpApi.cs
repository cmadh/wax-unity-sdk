using System.Collections.Generic;
using System.Text;

using Cryptography.ECDSA;

using Cysharp.Threading.Tasks;

using Newtonsoft.Json;

using StrattonStudios.EosioUnity.Models;

using UnityEngine;
using UnityEngine.Networking;

namespace StrattonStudios.EosioUnity.Api
{

    /// <summary>
    /// The EOS Chain HTTP API provides methods for interacting with the blockchain.
    /// </summary>
    public class EosioChainHttpApi : EosioHttpApiBase
    {

        #region Constants

        public const string Endpoint = "chain";

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
        /// Creates a new instance of <see cref="EosioChainHttpApi"/>.
        /// </summary>
        /// <param name="baseUrl">The base URL endpoint for the blockchain</param>
        public EosioChainHttpApi(string baseUrl) : base(baseUrl)
        {
        }

        #endregion

        #region Public Methods

        public UniTask<GetInfoResponse> GetInfo()
        {
            const string endpoint = "get_info";
            return MakeGetJsonRequest<GetInfoResponse>(endpoint);
        }

        public UniTask<GetAccountResponse> GetAccount(GetAccountRequest data)
        {
            const string endpoint = "get_account";
            return MakePostJsonRequest<GetAccountResponse>(endpoint, data);
        }

        public UniTask<GetCodeResponse> GetCode(GetCodeRequest data, bool reload = false)
        {
            const string endpoint = "get_code";
            return MakePostJsonRequest<GetCodeResponse>(endpoint, data);
        }

        public UniTask<GetAbiResponse> GetAbi(GetAbiRequest data, bool reload = false)
        {
            const string endpoint = "get_abi";
            return MakePostJsonRequest<GetAbiResponse>(endpoint, data);
        }

        public UniTask<GetRawCodeAndAbiResponse> GetRawCodeAndAbi(GetRawCodeAndAbiRequest data, bool reload = false)
        {
            const string endpoint = "get_raw_code_and_abi";
            return MakePostJsonRequest<GetRawCodeAndAbiResponse>(endpoint, data);
        }

        public UniTask<GetRawAbiResponse> GetRawAbi(GetRawAbiRequest data, bool reload = false)
        {
            const string endpoint = "get_raw_abi";
            return MakePostJsonRequest<GetRawAbiResponse>(endpoint, data);
        }

        public UniTask<AbiJsonToBinResponse> AbiJsonToBin(AbiJsonToBinRequest data)
        {
            const string endpoint = "abi_json_to_bin";
            return MakePostJsonRequest<AbiJsonToBinResponse>(endpoint, data);
        }

        public UniTask<AbiBinToJsonResponse> AbiBinToJson(AbiBinToJsonRequest data)
        {
            const string endpoint = "abi_bin_to_json";
            return MakePostJsonRequest<AbiBinToJsonResponse>(endpoint, data);
        }

        public UniTask<GetRequiredKeysResponse> GetRequiredKeys(GetRequiredKeysRequest data)
        {
            const string endpoint = "get_required_keys";
            return MakePostJsonRequest<GetRequiredKeysResponse>(endpoint, data);
        }

        public UniTask<GetBlockResponse> GetBlock(GetBlockRequest data)
        {
            const string endpoint = "get_block";
            return MakePostJsonRequest<GetBlockResponse>(endpoint, data);
        }

        public UniTask<GetBlockHeaderStateResponse> GetBlockHeaderState(GetBlockHeaderStateRequest data)
        {
            const string endpoint = "get_block_header_state";
            return MakePostJsonRequest<GetBlockHeaderStateResponse>(endpoint, data);
        }

        public UniTask<GetTableRowsResponse<TRowType>> GetTableRows<TRowType>(GetTableRowsRequest data)
        {
            const string endpoint = "get_table_rows";
            return MakePostJsonRequest<GetTableRowsResponse<TRowType>>(endpoint, data);
        }

        public UniTask<GetTableRowsResponse> GetTableRows(GetTableRowsRequest data)
        {
            const string endpoint = "get_table_rows";
            return MakePostJsonRequest<GetTableRowsResponse>(endpoint, data);
        }

        public UniTask<GetTableByScopeResponse> GetTableByScope(GetTableByScopeRequest data)
        {
            const string endpoint = "get_table_by_scope";
            return MakePostJsonRequest<GetTableByScopeResponse>(endpoint, data);
        }

        public async UniTask<GetCurrencyBalanceResponse> GetCurrencyBalance(GetCurrencyBalanceRequest data)
        {
            const string endpoint = "get_currency_balance";
            return new GetCurrencyBalanceResponse() { assets = await MakePostJsonRequest<List<string>>(endpoint, data) };
        }

        public async UniTask<GetCurrencyStatsResponse> GetCurrencyStats(GetCurrencyStatsRequest data)
        {
            const string endpoint = "get_currency_stats";
            return new GetCurrencyStatsResponse() { stats = await MakePostJsonRequest<Dictionary<string, CurrencyStat>>(endpoint, data) };
        }

        public UniTask<GetProducersResponse> GetProducers(GetProducersRequest data)
        {
            const string endpoint = "get_producers";
            return MakePostJsonRequest<GetProducersResponse>(endpoint, data);
        }

        public UniTask<GetProducerScheduleResponse> GetProducerSchedule()
        {
            const string endpoint = "get_producer_schedule";
            return MakeGetJsonRequest<GetProducerScheduleResponse>(endpoint);
        }

        public UniTask<GetScheduledTransactionsResponse> GetScheduledTransactions(GetScheduledTransactionsRequest data)
        {
            const string endpoint = "get_scheduled_transactions";
            return MakePostJsonRequest<GetScheduledTransactionsResponse>(endpoint, data);
        }

        public UniTask<PushTransactionResponse> PushTransaction(PushTransactionRequest data)
        {
            const string endpoint = "push_transaction";
            return MakePostJsonRequest<PushTransactionResponse>(endpoint, data);
        }

        #endregion

    }

}
