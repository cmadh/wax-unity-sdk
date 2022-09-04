using System.Collections.Generic;

using Cysharp.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using StrattonStudios.EosioUnity.Models;
using StrattonStudios.EosioUnity.Serialization;
using StrattonStudios.EosioUnity.Signing;
using StrattonStudios.EosioUnity.Utilities;

namespace StrattonStudios.EosioUnity
{

    /// <summary>
    /// Transaction model.
    /// </summary>
    public class Transaction : TransactionHeader, IESRRequest
    {

        public const string VARIANT_TYPE = "transaction";
        public const string EXPIRATION = "expiration";
        public const string REF_BLOCK_NUM = "ref_block_num";
        public const string MAX_NET_USAGE_WORDS = "max_net_usage_words";
        public const string REF_BLOCK_PREFIX = "ref_block_prefix";
        public const string MAX_CPU_USAGE_MS = "max_cpu_usage_ms";
        public const string DELAY_SEC = "delay_sec";

        protected const string CONTEXT_FREE_ACTIONS = "context_free_actions";
        protected const string ACTIONS = "actions";
        protected const string TRANSACTION_EXTENSIONS = "transaction_extensions";

        protected List<Action> contextFreeActions = new List<Action>();
        protected List<Action> actions = new List<Action>();
        protected List<Extension> transactionExtensions = new List<Extension>();

        public virtual List<Action> ContextFreeActions
        {
            get
            {
                return this.contextFreeActions;
            }
            set
            {
                this.contextFreeActions = value;
            }
        }

        public virtual List<Action> Actions
        {
            get
            {
                return this.actions;
            }
            set
            {
                this.actions = value;
            }
        }

        public virtual List<Extension> TransactionExtensions
        {
            get
            {
                return this.transactionExtensions;
            }
            set
            {
                this.transactionExtensions = value;
            }
        }

        public Transaction() : base()
        {
        }

        public Transaction(string expiration, ushort refBlockNum,
                           uint refBlockPrefix, uint maxNetUsageWords,
                           byte maxCpuUsageMs, uint delaySec,
                           List<Action> contextFreeActions, List<Action> actions,
                           List<Extension> transactionExtensions) : base(expiration, refBlockNum, refBlockPrefix, maxNetUsageWords, maxCpuUsageMs, delaySec)
        {
            this.contextFreeActions = contextFreeActions;
            this.actions = actions;
            this.transactionExtensions = transactionExtensions;
        }

        public Transaction(JObject obj) : base((string)obj[EXPIRATION], (ushort)obj[REF_BLOCK_NUM],
                    (uint)obj[REF_BLOCK_PREFIX], (uint)obj[MAX_NET_USAGE_WORDS],
                    (byte)obj[MAX_CPU_USAGE_MS], (uint)obj[DELAY_SEC])
        {
            this.contextFreeActions = new Actions((JArray)obj[CONTEXT_FREE_ACTIONS]).GetActions();
            this.actions = new Actions((JArray)obj[ACTIONS]).GetActions();
            this.transactionExtensions = MakeExtensionList((JArray)obj[TRANSACTION_EXTENSIONS]);
        }

        public Transaction(Dictionary<string, object> dictionary) : base((string)dictionary[EXPIRATION], (ushort)dictionary[REF_BLOCK_NUM],
                    (uint)dictionary[REF_BLOCK_PREFIX], (uint)dictionary[MAX_NET_USAGE_WORDS],
                    (byte)dictionary[MAX_CPU_USAGE_MS], (uint)dictionary[DELAY_SEC])
        {
            this.contextFreeActions = new Actions((List<object>)dictionary[CONTEXT_FREE_ACTIONS]).GetActions();
            this.actions = new Actions((List<object>)dictionary[ACTIONS]).GetActions();
            this.transactionExtensions = MakeExtensionList((List<object>)dictionary[TRANSACTION_EXTENSIONS]);
        }

        public virtual Transaction ShallowClone()
        {
            Transaction cloned = new Transaction();
            cloned.Expiration = Expiration;
            cloned.RefBlockNum = RefBlockNum;
            cloned.RefBlockPrefix = RefBlockPrefix;
            cloned.MaxNetUsageWords = MaxNetUsageWords;
            cloned.MaxNetUsageMs = MaxNetUsageMs;
            cloned.ContextFreeActions = ContextFreeActions;
            cloned.Actions = Actions;
            cloned.TransactionExtensions = TransactionExtensions;
            return cloned;
        }

        private static List<Extension> MakeExtensionList(JArray array)
        {
            List<Extension> extensions = new List<Extension>();
            foreach (JToken el in array)
            {
                if (!(el is JObject))
                {
                    throw new EosioException("Extensions should be an object");
                }

                JObject obj = (JObject)el;
                extensions.Add(new Extension(obj));
            }

            return extensions;
        }

        private static List<Extension> MakeExtensionList(List<object> array)
        {
            List<Extension> extensions = new List<Extension>();
            foreach (var item in array)
            {
                if (!(item is Dictionary<string, object>))
                {
                    throw new EosioException("Extensions should be an object");
                }

                var dictionary = item as Dictionary<string, object>;
                extensions.Add(new Extension(dictionary));
            }

            return extensions;
        }

        public virtual List<Action> GetRawActions()
        {
            return Actions;
        }

        public virtual List<object> ToVariant()
        {
            List<object> variant = new List<object>();
            variant.Add(VARIANT_TYPE);
            variant.Add(ToDictionary());
            return variant;
        }

        public virtual async UniTask<string> SigningDigest(AbiTypeWriter serializer, string chainId)
        {
            var data = await SigningData(serializer, chainId);
            return HexUtility.ToHexString(Cryptography.ECDSA.Sha256Manager.GetHash(data));
        }

        public virtual async UniTask<byte[]> SigningData(AbiTypeWriter serializer, string chainId)
        {
            var data = new List<byte>();
            data.AddRange(HexUtility.FromHexString(chainId));
            data.AddRange(await serializer.SerializeTransaction(this));
            data.AddRange(new byte[32]);
            return data.ToArray();
        }

        public virtual string ToJSON()
        {
            return JsonConvert.SerializeObject(ToDictionary(), new Converters.ByteArrayConverter());
        }

        public override string ToString()
        {
            return ToJSON();
        }

        protected virtual Dictionary<string, object> ToDictionary()
        {
            Dictionary<string, object> transactionMap = new Dictionary<string, object>();
            transactionMap.Add(EXPIRATION, Expiration);
            transactionMap.Add(REF_BLOCK_NUM, RefBlockNum);
            transactionMap.Add(REF_BLOCK_PREFIX, RefBlockPrefix);
            transactionMap.Add(MAX_NET_USAGE_WORDS, MaxNetUsageWords);
            transactionMap.Add(MAX_CPU_USAGE_MS, MaxNetUsageMs);
            transactionMap.Add(DELAY_SEC, DelaySec);

            List<Dictionary<string, object>> contextFreeActionMaps = new List<Dictionary<string, object>>();
            foreach (Action action in this.contextFreeActions)
                contextFreeActionMaps.Add(action.ToDictionary());

            transactionMap.Add(CONTEXT_FREE_ACTIONS, contextFreeActionMaps);

            List<Dictionary<string, object>> actionMaps = new List<Dictionary<string, object>>();
            foreach (Action action in this.actions)
                actionMaps.Add(action.ToDictionary());

            transactionMap.Add(ACTIONS, actionMaps);

            List<Dictionary<short, string>> transactionExtensionMaps = new List<Dictionary<short, string>>();
            foreach (Extension extension in this.transactionExtensions)
                transactionExtensionMaps.Add(extension.ToDictionary());

            transactionMap.Add(TRANSACTION_EXTENSIONS, transactionExtensionMaps);

            return transactionMap;
        }

        public virtual bool HasTapos()
        {
            return !(DEFAULT_EXPIRATION.Equals(Expiration) &&
                    RefBlockNum == 0 &&
                    RefBlockPrefix == 0);
        }

    }

}