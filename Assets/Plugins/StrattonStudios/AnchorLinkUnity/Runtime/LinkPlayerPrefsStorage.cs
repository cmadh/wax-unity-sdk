using System.Collections;
using System.Collections.Generic;

using Cysharp.Threading.Tasks;

using UnityEngine;

namespace StrattonStudios.AnchorLinkUnity
{

    /// <summary>
    /// Link storage using Unity PlayerPrefs.
    /// </summary>
    public class LinkPlayerPrefsStorage : ILinkStorage
    {

        public UniTask<string> Read(string key)
        {
            return UniTask.FromResult(PlayerPrefs.GetString(key));
        }

        public UniTask Remove(string key)
        {
            PlayerPrefs.DeleteKey(key);
            return UniTask.CompletedTask;
        }

        public UniTask Write(string key, string data)
        {
            PlayerPrefs.SetString(key, data);
            return UniTask.CompletedTask;
        }

    }

}