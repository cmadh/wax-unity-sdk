using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

using Cysharp.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using StrattonStudios.AtomicAssetsUnity.Models;
using StrattonStudios.Networking.Utilities;

using UnityEditor.Search;

using UnityEngine;
using UnityEngine.Networking;

namespace StrattonStudios.AtomicAssetsUnity.Api
{

    /// <summary>
    /// The Atomic Assets HTTP API.
    /// </summary>
    public class AtomicAssetsHttpApi
    {

        #region Constants

        public const string VersionSuffix = "v1";

        protected const string SchemasEndpoint = "schemas";
        protected const string TransfersEndpoint = "transfers";
        protected const string TemplatesEndpoint = "templates";
        protected const string OffersEndpoint = "offers";
        protected const string ConfigEndpoint = "config";
        protected const string CollectionsEndpoint = "collections";
        protected const string BurnsEndpoint = "burns";
        protected const string AssetsEndpoint = "assets";
        protected const string AccountsEndpoint = "accounts";

        protected const string LogsEndpoint = "logs";
        protected const string StatsEndpoint = "stats";

        #endregion

        #region Fields

        protected string baseUrl;

        #endregion

        #region Properties

        public virtual string BaseUrl
        {
            get => this.baseUrl;
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    this.baseUrl = value.TrimEnd('/');
                    if (!string.IsNullOrEmpty(this.baseUrl) && !this.baseUrl.EndsWith(VersionSuffix))
                    {
                        this.baseUrl += "/" + VersionSuffix;
                    }
                }
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new instance of <see cref="AtomicAssetsHttpApi"/>.
        /// </summary>
        /// <param name="baseUrl">The base URL endpoint</param>
        public AtomicAssetsHttpApi(string baseUrl)
        {
            this.baseUrl = baseUrl;
        }

        #endregion

        #region Public Methods

        //
        // Transfers
        //

        public virtual UniTask<GetTransfersResponse> GetTransfersAsync()
        {
            return GetTransfersAsync(null);
        }

        public virtual UniTask<GetTransfersResponse> GetTransfersAsync(GetTransfersRequest request)
        {
            const string endpoint = "transfers";
            return MakeGetJsonRequest<GetTransfersResponse>(endpoint, request);
        }

        //
        // Templates
        //

        public virtual UniTask<GetTemplatesResponse> GetTemplatesAsync()
        {
            return GetTemplatesAsync(null);
        }

        public virtual UniTask<GetTemplatesResponse> GetTemplatesAsync(GetTemplatesRequest request)
        {
            const string endpoint = TemplatesEndpoint;
            return MakeGetJsonRequest<GetTemplatesResponse>(endpoint, request);
        }

        public virtual UniTask<GetTemplateResponse> GetTemplateAsync(string collectionName, string templateId)
        {
            const string endpoint = TemplatesEndpoint + "/{0}/{1}";
            return MakeGetJsonRequest<GetTemplateResponse>(string.Format(endpoint, collectionName, templateId));
        }

        public virtual UniTask<GetStatsResponse> GetTemplateStatsAsync(string collectionName, string templateId)
        {
            const string endpoint = TemplatesEndpoint + "/{0}/{1}/" + StatsEndpoint;
            return MakeGetJsonRequest<GetStatsResponse>(string.Format(endpoint, collectionName, templateId));
        }

        public virtual UniTask<GetLogsResponse> GetTemplateLogsAsync(string collectionName, string templateId)
        {
            const string endpoint = TemplatesEndpoint + "/{0}/{1}/" + LogsEndpoint;
            return MakeGetJsonRequest<GetLogsResponse>(string.Format(endpoint, collectionName, templateId));
        }

        public virtual UniTask<GetLogsResponse> GetTemplateLogsAsync(string collectionName, string templateId, GetTemplatesRequest request)
        {
            const string endpoint = TemplatesEndpoint + "/{0}/{1}/" + LogsEndpoint;
            return MakeGetJsonRequest<GetLogsResponse>(string.Format(endpoint, collectionName, templateId), request);
        }

        //
        // Schemas
        //

        public virtual UniTask<GetSchemasResponse> GetSchemasAsync()
        {
            return GetSchemasAsync(null);
        }

        public virtual UniTask<GetSchemasResponse> GetSchemasAsync(GetSchemasRequest request)
        {
            const string endpoint = SchemasEndpoint;
            return MakeGetJsonRequest<GetSchemasResponse>(endpoint, request);
        }

        public virtual UniTask<GetSchemaResponse> GetSchemaAsync(string collectionName, string schemaName)
        {
            const string endpoint = SchemasEndpoint + "/{0}/{1}";
            return MakeGetJsonRequest<GetSchemaResponse>(string.Format(endpoint, collectionName, schemaName));
        }

        public virtual UniTask<GetStatsResponse> GetSchemaStatsAsync(string collectionName, string schemaName)
        {
            const string endpoint = SchemasEndpoint + "/{0}/{1}/" + StatsEndpoint;
            return MakeGetJsonRequest<GetStatsResponse>(string.Format(endpoint, collectionName, schemaName));
        }

        public virtual UniTask<GetLogsResponse> GetSchemaLogsAsync(string collectionName, string schemaName)
        {
            const string endpoint = SchemasEndpoint + "/{0}/{1}/" + LogsEndpoint;
            return MakeGetJsonRequest<GetLogsResponse>(string.Format(endpoint, collectionName, schemaName));
        }

        public virtual UniTask<GetLogsResponse> GetSchemaLogsAsync(string collectionName, string schemaName, GetSchemasRequest request)
        {
            const string endpoint = SchemasEndpoint + "/{0}/{1}/" + LogsEndpoint;
            return MakeGetJsonRequest<GetLogsResponse>(string.Format(endpoint, collectionName, schemaName), request);
        }

        //
        // Offers
        //

        public virtual UniTask<GetOffersResponse> GetOffersAsync()
        {
            return GetOffersAsync(null);
        }

        public virtual UniTask<GetOffersResponse> GetOffersAsync(GetOffersRequest request)
        {
            const string endpoint = OffersEndpoint + "";
            return MakeGetJsonRequest<GetOffersResponse>(endpoint, request);
        }

        public virtual UniTask<GetOfferResponse> GetOfferAsync(string offerId)
        {
            const string endpoint = OffersEndpoint + "/{0}";
            return MakeGetJsonRequest<GetOfferResponse>(string.Format(endpoint, offerId));
        }

        public virtual UniTask<GetLogsResponse> GetOfferLogsAsync(string offerId)
        {
            const string endpoint = OffersEndpoint + "/{0}/" + LogsEndpoint;
            return MakeGetJsonRequest<GetLogsResponse>(string.Format(endpoint, offerId));
        }

        public virtual UniTask<GetLogsResponse> GetOfferLogsAsync(string offerId, GetOffersRequest request)
        {
            const string endpoint = OffersEndpoint + "/{0}/" + LogsEndpoint;
            return MakeGetJsonRequest<GetLogsResponse>(string.Format(endpoint, offerId), request);
        }

        //
        // Config
        //

        public virtual UniTask<Dictionary<string, object>> GetConfigAsync()
        {
            const string endpoint = ConfigEndpoint + "";
            return MakeGetJsonRequest<Dictionary<string, object>>(endpoint);
        }

        //
        // Collections
        //

        public virtual UniTask<GetCollectionsResponse> GetCollectionsAsync()
        {
            return GetCollectionsAsync(null);
        }

        public virtual UniTask<GetCollectionsResponse> GetCollectionsAsync(GetCollectionsRequest request)
        {
            const string endpoint = CollectionsEndpoint + "";
            return MakeGetJsonRequest<GetCollectionsResponse>(endpoint, request);
        }

        public virtual UniTask<GetCollectionResponse> GetCollectionAsync(string collectionName)
        {
            const string endpoint = CollectionsEndpoint + "/{0}";
            return MakeGetJsonRequest<GetCollectionResponse>(string.Format(endpoint, collectionName));
        }

        public virtual UniTask<GetStatsResponse> GetCollectionStatsAsync(string collectionName)
        {
            const string endpoint = CollectionsEndpoint + "/{0}/" + StatsEndpoint;
            return MakeGetJsonRequest<GetStatsResponse>(string.Format(endpoint, collectionName));
        }

        public virtual UniTask<GetLogsResponse> GetCollectionLogsAsync(string collectionName)
        {
            const string endpoint = CollectionsEndpoint + "/{0}/" + LogsEndpoint;
            return MakeGetJsonRequest<GetLogsResponse>(string.Format(endpoint, collectionName));
        }

        public virtual UniTask<GetLogsResponse> GetCollectionLogsAsync(string collectionName, GetCollectionsRequest request)
        {
            const string endpoint = CollectionsEndpoint + "/{0}/" + LogsEndpoint;
            return MakeGetJsonRequest<GetLogsResponse>(string.Format(endpoint, collectionName), request);
        }

        //
        // Burns
        //

        public virtual UniTask<GetBurnsResponse> GetBurnsAsync()
        {
            return GetBurnsAsync(null);
        }

        public virtual UniTask<GetBurnsResponse> GetBurnsAsync(GetBurnsRequest request)
        {
            const string endpoint = BurnsEndpoint + "";
            return MakeGetJsonRequest<GetBurnsResponse>(endpoint, request);
        }

        public virtual UniTask<GetBurnResponse> GetBurnAsync(string accountName)
        {
            const string endpoint = BurnsEndpoint + "/{0}";
            return MakeGetJsonRequest<GetBurnResponse>(string.Format(endpoint, accountName));
        }

        //
        // Assets
        //

        public virtual UniTask<GetAssetsResponse> GetAssetsAsync()
        {
            return GetAssetsAsync(null);
        }

        public virtual UniTask<GetAssetsResponse> GetAssetsAsync(GetAssetsRequest request)
        {
            const string endpoint = AssetsEndpoint + "";
            return MakeGetJsonRequest<GetAssetsResponse>(endpoint, request);
        }

        public virtual UniTask<GetAssetResponse> GetAssetAsync(string assetId)
        {
            const string endpoint = AssetsEndpoint + "/{0}";
            return MakeGetJsonRequest<GetAssetResponse>(string.Format(endpoint, assetId));
        }

        public virtual UniTask<GetStatsResponse> GetAssetStatsAsync(string assetId)
        {
            const string endpoint = AssetsEndpoint + "/{0}/" + StatsEndpoint;
            return MakeGetJsonRequest<GetStatsResponse>(string.Format(endpoint, assetId));
        }

        public virtual UniTask<GetLogsResponse> GetAssetLogsAsync(string assetId)
        {
            const string endpoint = AssetsEndpoint + "/{0}/" + LogsEndpoint;
            return MakeGetJsonRequest<GetLogsResponse>(string.Format(endpoint, assetId));
        }

        public virtual UniTask<GetLogsResponse> GetAssetLogsAsync(string assetId, GetAssetsRequest request)
        {
            const string endpoint = AssetsEndpoint + "/{0}/" + LogsEndpoint;
            return MakeGetJsonRequest<GetLogsResponse>(string.Format(endpoint, assetId), request);
        }

        //
        // Accounts
        //

        public virtual UniTask<GetAccountsResponse> GetAccountsAsync()
        {
            return GetAccountsAsync(null);
        }

        public virtual UniTask<GetAccountsResponse> GetAccountsAsync(GetAccountsRequest request)
        {
            const string endpoint = AccountsEndpoint + "";
            return MakeGetJsonRequest<GetAccountsResponse>(endpoint, request);
        }

        public virtual UniTask<GetAccountResponse> GetAccountAsync(string accountName)
        {
            const string endpoint = AccountsEndpoint + "/{0}";
            return MakeGetJsonRequest<GetAccountResponse>(string.Format(endpoint, accountName));
        }

        public virtual UniTask<GetAccountResponse> GetAccountCollectionAsync(string accountName, string collectionName)
        {
            const string endpoint = AccountsEndpoint + "/{0}/{1}";
            return MakeGetJsonRequest<GetAccountResponse>(string.Format(endpoint, accountName, collectionName));
        }

        #endregion

        #region Protected Methods

        protected virtual string SerializeJson(object obj)
        {
            string json = JsonConvert.SerializeObject(obj);
            return json;
        }

        protected virtual T DeserializeJson<T>(string json)
        {
            Debug.Log(json);
            return JsonConvert.DeserializeObject<T>(json);
        }

        /// <summary>
        /// Override this to apply changes to the web request before submitting it.
        /// </summary>
        /// <param name="uwr"></param>
        protected virtual void PrepareRequest(UnityWebRequest uwr)
        {

        }

        protected virtual async UniTask<TResponse> MakePostJsonRequest<TResponse>(string endpoint, object request = null)
        {
            var urlBuilder = new StringBuilder(BaseUrl).Append("/" + endpoint);
            var uri = urlBuilder.ToString();
            string postData = string.Empty;
            if (request != null)
            {
                postData = SerializeJson(request);
            }
            var uwr = UnityWebRequest.Post(uri, postData);
            switch (Application.platform)
            {
                default:
                    uwr.SetRequestHeader("Content-Type", "application/json");
                    break;
                case RuntimePlatform.WebGLPlayer:
                    uwr.SetRequestHeader("Content-Type", "text/plain");
                    break;
            }
            PrepareRequest(uwr);
            try
            {
                await uwr.SendWebRequest();
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }
            var json = uwr.downloadHandler.text;
            return DeserializeJson<TResponse>(json);
        }

        protected virtual async UniTask<TResponse> MakePostFormRequest<TResponse>(string endpoint, List<IMultipartFormSection> formData)
        {
            var urlBuilder = new StringBuilder(BaseUrl).Append("/" + endpoint);
            var uri = urlBuilder.ToString();
            var uwr = UnityWebRequest.Post(uri, formData);
            PrepareRequest(uwr);
            try
            {
                await uwr.SendWebRequest();
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }
            var json = uwr.downloadHandler.text;
            return DeserializeJson<TResponse>(json);
        }

        protected virtual async UniTask<TResponse> MakeGetJsonRequest<TResponse>(string endpoint, object query = null)
        {
            var urlBuilder = new StringBuilder(BaseUrl).Append("/" + endpoint);
            string queryString = null;
            if (query != null)
            {
                queryString = query.ToQueryString();
            }
            if (!string.IsNullOrEmpty(queryString))
            {
                urlBuilder.Append("?" + queryString);
            }
            var uri = urlBuilder.ToString();
            var uwr = UnityWebRequest.Get(uri);
            PrepareRequest(uwr);
            try
            {
                await uwr.SendWebRequest();
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }
            var json = uwr.downloadHandler.text;
            return DeserializeJson<TResponse>(json);
        }

        #endregion

    }

}