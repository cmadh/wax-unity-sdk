using System;
using System.Runtime.Serialization;

namespace StrattonStudios.EosioUnity
{

    /// <summary>
    /// The EOSIO API exception.
    /// </summary>
    [Serializable]
    public class EosioApiException : EosioException
    {

        public int StatusCode { get; set; }
        public string Content { get; set; }

        public EosioApiException()
        {

        }

        public EosioApiException(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                return;

            StatusCode = info.GetInt32("StatusCode");
            Content = info.GetString("Content");
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                return;

            base.GetObjectData(info, context);
            info.AddValue("StatusCode", StatusCode);
            info.AddValue("Content", Content);
        }

    }

}
