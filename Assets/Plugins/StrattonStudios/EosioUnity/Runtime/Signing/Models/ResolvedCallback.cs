using System.Collections.Generic;

using Newtonsoft.Json.Linq;

namespace StrattonStudios.EosioUnity.Signing
{

    /// <summary>
    /// Resolved callback model
    /// </summary>
    public class ResolvedCallback
    {

        public const string SIG = "SIG";
        public const string TX = "tx";
        public const string RBN = "rbn";
        public const string RID = "rid";
        public const string EX = "ex";
        public const string REQ = "req";
        public const string SA = "sa";
        public const string SP = "sp";
        public const string BN = "bn";

        private string url;
        private bool background;

        // TODO: Use CallbackPayload
        private Dictionary<string, string> payload;

        public ResolvedCallback(string url, bool background, Dictionary<string, string> payload)
        {
            this.url = url;
            this.background = background;
            this.payload = payload;
        }

        public string GetUrl()
        {
            return this.url;
        }

        public bool IsBackground()
        {
            return this.background;
        }

        public Dictionary<string, string> GetPayload()
        {
            return this.payload;
        }

    }

}
