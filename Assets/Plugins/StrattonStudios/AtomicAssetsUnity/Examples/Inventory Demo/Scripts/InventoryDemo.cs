using System.Collections;
using System.Collections.Generic;

using Cysharp.Threading.Tasks;

using StrattonStudios.AtomicAssetsUnity.Models;

using TMPro;

using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace StrattonStudios.AtomicAssetsUnity.Examples
{

    public class InventoryDemo : MonoBehaviour
    {

        public TMP_InputField accountInputField;
        public Transform container;
        public InventorySlot slotPrefab;
        public Texture2D emptyTexture;

        public List<InventorySlot> slots = new List<InventorySlot>();

        public Dictionary<string, Texture2D> imageCache = new Dictionary<string, Texture2D>();

        protected AtomicAssetsClient client;

        private void Start()
        {
            this.client = new AtomicAssetsClient();
            RandomAccount();
        }

        public async void RandomAccount()
        {
            var request = new GetAccountsRequest();
            request.SchemaName = "boxes";
            var accounts = await this.client.HttpApi.GetAccountsAsync(request);
            this.accountInputField.text = accounts.Data[Random.Range(0, accounts.Data.Length)].Account;
        }

        public async void UpdateInventory()
        {
            DestoryAll();
            Debug.Log("Updating inventory ...");
            var getRequest = new GetAssetsRequest();
            getRequest.SchemaName = "boxes";
            getRequest.AuthorizedAccount = this.accountInputField.text;
            var assets = await this.client.HttpApi.GetAssetsAsync(getRequest);

            if (assets.Data.Length == 0)
            {
                Debug.LogWarning("This account has no assets, please try another account.");
            }

            foreach (var asset in assets.Data)
            {
                if (!string.IsNullOrEmpty(asset.Template.ImmutableData.Image))
                {
                    if (!this.imageCache.TryGetValue(asset.Template.ImmutableData.Image, out var texture))
                    {
                        if (string.IsNullOrEmpty(asset.Template.ImmutableData.Image))
                        {
                            texture = this.emptyTexture;
                        }
                        else
                        {
                            try
                            {
                                string url = string.Format("https://ipfs.io/ipfs/{0}", asset.Template.ImmutableData.Image);
                                var request = UnityWebRequestTexture.GetTexture(url);
                                await request.SendWebRequest();
                                texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
                            }
                            catch (System.Exception e)
                            {
                                Debug.LogError("Can't load image");
                                Debug.LogException(e);
                                texture = this.emptyTexture;
                            }
                        }
                        this.imageCache.Add(asset.Template.ImmutableData.Image, texture);
                    }
                    var slot = CreateNewSlot();
                    slot.rawImage.texture = texture;
                    slot.assetId = asset.AssetId;
                    slot.text.text = asset.Name;
                    this.slots.Add(slot);
                }
            }
            Debug.Log("Updated inventory.");
        }

        public InventorySlot CreateNewSlot()
        {
            return Instantiate(this.slotPrefab, this.container);
        }

        public void DestoryAll()
        {
            for (int i = 0; i < this.slots.Count; i++)
            {
                Destroy(this.slots[i].gameObject);
            }
            this.slots.Clear();
        }

    }

}