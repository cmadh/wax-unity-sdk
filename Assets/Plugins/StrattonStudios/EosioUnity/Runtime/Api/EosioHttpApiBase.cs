using System.Collections.Generic;
using System.Net;
using System.Text;

using Cysharp.Threading.Tasks;

using Newtonsoft.Json;

using StrattonStudios.EosioUnity.Models;

using UnityEngine;
using UnityEngine.Networking;

namespace StrattonStudios.EosioUnity.Api
{

    /// <summary>
    /// The base class for the EOS HTTP APIs.
    /// </summary>
    public abstract class EosioHttpApiBase
    {

        #region Constants

        public const string VersionSuffix = "v1";

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
        /// Creates a new instance of <see cref="EosioHttpApiBase"/>.
        /// </summary>
        /// <param name="baseUrl">The base URL endpoint for the blockchain</param>
        public EosioHttpApiBase(string baseUrl)
        {
            BaseUrl = baseUrl;
        }

        #endregion

        #region Protected Methods

        protected virtual string SerializeJson(object obj)
        {
            string json;
            if (obj is IJsonModel)
            {
                json = (obj as IJsonModel).ToJSON();
            }
            else
            {
                json = JsonConvert.SerializeObject(obj);
            }
            return json;
        }

        protected virtual T DeserializeJson<T>(string json)
        {
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
            var uwr = new UnityWebRequest(uri, UnityWebRequest.kHttpVerbPOST)
            {
                uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(postData)),
                downloadHandler = new DownloadHandlerBuffer()
            };
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

        protected virtual async UniTask<TResponse> MakeGetJsonRequest<TResponse>(string endpoint, string query = null)
        {
            var urlBuilder = new StringBuilder(BaseUrl).Append("/" + endpoint);
            if (!string.IsNullOrEmpty(query))
            {
                urlBuilder.Append("?" + query);
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
