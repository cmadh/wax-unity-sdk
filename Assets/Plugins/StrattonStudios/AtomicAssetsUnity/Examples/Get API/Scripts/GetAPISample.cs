using System.Collections;
using System.Collections.Generic;

using Cysharp.Threading.Tasks;

using StrattonStudios.AtomicAssetsUnity.Models;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

namespace StrattonStudios.AtomicAssetsUnity.Examples
{

    /// <summary>
    /// An example of fetching different kind of information from the API.
    /// </summary>
    public class GetAPISample : MonoBehaviour
    {

        [SerializeField]
        protected TextMeshProUGUI statusText;

        [SerializeField]
        protected CanvasGroup canvasGroup;

        protected AtomicAssetsClient client;

        private void Start()
        {
            this.client = new AtomicAssetsClient();
        }

        public async void GetAssets()
        {
            OnRequest("Retrieving assets ...");
            try
            {
                var result = await this.client.HttpApi.GetAssetsAsync();
                Debug.Log("Here are the assets:");
                for (int i = 0; i < result.Data.Length; i++)
                {
                    Debug.Log(result.Data[i].AssetId);
                }
                OnResponse("Retrieved assets successfully.");
            }
            catch (System.Exception e)
            {
                OnResponse("Retrieving assets failed, check console for more information.");
                Debug.LogException(e);
            }
        }

        public async void GetOffers()
        {
            OnRequest("Retrieving offers ...");
            try
            {
                var result = await this.client.HttpApi.GetOffersAsync();
                Debug.Log("Here are the offers:");
                for (int i = 0; i < result.Data.Length; i++)
                {
                    Debug.Log(result.Data[i].OfferId);
                }
                OnResponse("Retrieved offers successfully.");
            }
            catch (System.Exception e)
            {
                OnResponse("Retrieving offers failed, check console for more information.");
                Debug.LogException(e);
            }
        }

        public async void GetSchemas()
        {
            OnRequest("Retrieving schemas ...");
            try
            {
                var result = await this.client.HttpApi.GetSchemasAsync();
                Debug.Log("Here are the schemas:");
                for (int i = 0; i < result.Data.Length; i++)
                {
                    Debug.Log(result.Data[i].SchemaName);
                }
                OnResponse("Retrieved schemas successfully.");
            }
            catch (System.Exception e)
            {
                OnResponse("Retrieving schemas failed, check console for more information.");
                Debug.LogException(e);
            }
        }

        public async void GetAccounts()
        {
            OnRequest("Retrieving accounts ...");
            try
            {
                var request = new GetAccountsRequest();
                request.SchemaName = "boxes";
                var result = await this.client.HttpApi.GetAccountsAsync(request);
                Debug.Log("Here are the accounts:");
                for (int i = 0; i < result.Data.Length; i++)
                {
                    Debug.Log(result.Data[i].Account);
                }
                OnResponse("Retrieved accounts successfully.");
            }
            catch (System.Exception e)
            {
                OnResponse("Retrieving accounts failed, check console for more information.");
                Debug.LogException(e);
            }
        }

        public async void GetBurns()
        {
            OnRequest("Retrieving burns ...");
            try
            {
                var request = new GetBurnsRequest();
                request.SchemaName = "boxes";
                var result = await this.client.HttpApi.GetBurnsAsync(request);
                Debug.Log("Here are the burns:");
                for (int i = 0; i < result.Data.Length; i++)
                {
                    Debug.Log(result.Data[i].Account);
                }
                OnResponse("Retrieved burns successfully.");
            }
            catch (System.Exception e)
            {
                OnResponse("Retrieving burns failed, check console for more information.");
                Debug.LogException(e);
            }
        }

        public async void GetCollections()
        {
            OnRequest("Retrieving collections ...");
            try
            {
                var result = await this.client.HttpApi.GetCollectionsAsync();
                Debug.Log("Here are the collections:");
                for (int i = 0; i < result.Data.Length; i++)
                {
                    Debug.Log(result.Data[i].CollectionName);
                }
                OnResponse("Retrieved collections successfully.");
            }
            catch (System.Exception e)
            {
                OnResponse("Retrieving collections failed, check console for more information.");
                Debug.LogException(e);
            }
        }

        public async void GetTemplates()
        {
            OnRequest("Retrieving templates ...");
            try
            {
                var result = await this.client.HttpApi.GetTemplatesAsync();
                Debug.Log("Here are the templates:");
                for (int i = 0; i < result.Data.Length; i++)
                {
                    Debug.Log(result.Data[i].TemplateId);
                }
                OnResponse("Retrieved templates successfully.");
            }
            catch (System.Exception e)
            {
                OnResponse("Retrieving templates failed, check console for more information.");
                Debug.LogException(e);
            }
        }

        public async void GetTransfers()
        {
            OnRequest("Retrieving transfers ...");
            try
            {
                var result = await this.client.HttpApi.GetTransfersAsync();
                Debug.Log("Here are the transfers:");
                for (int i = 0; i < result.Data.Length; i++)
                {
                    Debug.Log(result.Data[i].TransferId);
                }
                OnResponse("Retrieved transfers successfully.");
            }
            catch (System.Exception e)
            {
                OnResponse("Retrieving transfers failed, check console for more information.");
                Debug.LogException(e);
            }
        }

        protected void OnRequest(string text)
        {
            this.statusText.text = text;
            SetControlsInteractable(false);
        }

        protected void OnResponse(string text)
        {
            this.statusText.text = text;
            SetControlsInteractable(true);
        }

        public void SetControlsInteractable(bool interactable)
        {
            this.canvasGroup.interactable = interactable;
        }

    }

}