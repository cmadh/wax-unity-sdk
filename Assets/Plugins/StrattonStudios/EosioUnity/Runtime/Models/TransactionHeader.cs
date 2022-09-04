using System.Collections.Generic;

using Newtonsoft.Json.Linq;

namespace StrattonStudios.EosioUnity
{

    /// <summary>
    /// Transaction header model.
    /// </summary>
    public class TransactionHeader
    {

        public const string DEFAULT_EXPIRATION = "1970-01-01T00:00:00.000";

        private string expiration = DEFAULT_EXPIRATION;
        private ushort? refBlockNum = 0;
        private uint? refBlockPrefix = 0;
        private uint? maxNetUsageWords = 0;
        private byte? maxCpuUsageMs = 0;
        private uint? delaySec = 0;

        public string Expiration
        {
            get
            {
                return this.expiration;
            }
            set
            {
                this.expiration = value;
            }
        }

        public ushort? RefBlockNum
        {
            get
            {
                return this.refBlockNum;
            }
            set
            {
                this.refBlockNum = value;
            }
        }

        public uint? RefBlockPrefix
        {
            get
            {
                return this.refBlockPrefix;
            }
            set
            {
                this.refBlockPrefix = value;
            }
        }

        public uint? MaxNetUsageWords
        {
            get
            {
                return this.maxNetUsageWords;
            }
            set
            {
                this.maxNetUsageWords = value;
            }
        }

        public byte? MaxNetUsageMs
        {
            get
            {
                return this.maxCpuUsageMs;
            }
            set
            {
                this.maxCpuUsageMs = value;
            }
        }

        public uint? DelaySec
        {
            get
            {
                return this.delaySec;
            }
            set
            {
                this.delaySec = value;
            }
        }

        public TransactionHeader()
        {

        }

        public TransactionHeader(string expiration, ushort refBlockNum,
                                 uint refBlockPrefix, uint maxNetUsageWords,
                                 byte maxCpuUsageMs, uint delaySec)
        {
            this.expiration = expiration;
            this.refBlockNum = refBlockNum;
            this.refBlockPrefix = refBlockPrefix;
            this.maxNetUsageWords = maxNetUsageWords;
            this.maxCpuUsageMs = maxCpuUsageMs;
            this.delaySec = delaySec;
        }

        public void SetExpiration(string expiration)
        {
            this.expiration = expiration;
        }

    }

}