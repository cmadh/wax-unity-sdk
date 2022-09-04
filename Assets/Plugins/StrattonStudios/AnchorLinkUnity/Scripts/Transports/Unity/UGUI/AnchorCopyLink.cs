using System.Collections;
using System.Collections.Generic;

using Cysharp.Threading.Tasks;

using UnityEngine;

using UnityEngine.Events;

namespace StrattonStudios.AnchorLinkUnity.Transports.UGUI
{

    public class AnchorCopyLink : MonoBehaviour
    {

        [SerializeField]
        private string linkToCopy;

        [SerializeField]
        protected int millisecondsDelay = 2000;

        [SerializeField]
        private string copyText = "Copy request link";
        [SerializeField]
        private string copiedText = "Link copied - Paste in Anchor";

        [SerializeField]
        private Sprite copySprite;
        [SerializeField]
        private Sprite copiedSprite;

        [SerializeField]
        private UnityEvent<string> onSetText;
        [SerializeField]
        private UnityEvent<Sprite> onSetSprite;

        public string LinkToCopy
        {
            get
            {
                return this.linkToCopy;
            }
            set
            {
                this.linkToCopy = value;
            }
        }

        public void CopyLink()
        {
            TextEditor editor = new TextEditor
            {
                text = this.linkToCopy
            };
            editor.SelectAll();
            editor.Copy();
            OnCopied();
        }

        private async void OnCopied()
        {
            this.onSetText?.Invoke(this.copiedText);
            this.onSetSprite?.Invoke(this.copiedSprite);
            await UniTask.Delay(this.millisecondsDelay, true);
            this.onSetText?.Invoke(this.copyText);
            this.onSetSprite?.Invoke(this.copySprite);
        }

    }

}