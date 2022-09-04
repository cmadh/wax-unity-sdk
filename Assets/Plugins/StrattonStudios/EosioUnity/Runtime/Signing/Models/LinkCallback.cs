using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace StrattonStudios.EosioUnity.Signing
{

    /// <summary>
    /// Callback model.
    /// </summary>
    public class LinkCallback
    {

        private string url;
        private bool? background;

        public string Url
        {
            get
            {
                return this.url;
            }
        }

        public bool? Background
        {
            get
            {
                return this.background;
            }
        }

        public LinkCallback(string url)
        {
            this.url = url;
        }

        public LinkCallback(string url, bool background)
        {
            this.url = url;
            this.background = background;
        }

    }

}