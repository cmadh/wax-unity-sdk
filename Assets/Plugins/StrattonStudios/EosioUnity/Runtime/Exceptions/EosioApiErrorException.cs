using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace StrattonStudios.EosioUnity
{

    /// <summary>
    /// Wrapper exception for EOSIO API error.
    /// </summary>
    [Serializable]
    public class EosioApiErrorException : EosioException
    {

        public int code;
        public string message;
        public APIError error;

        public EosioApiErrorException()
        {

        }

        public EosioApiErrorException(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                return;

            this.code = info.GetInt32("code");
            this.message = info.GetString("message");
            this.error = (APIError)info.GetValue("error", typeof(APIError));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                return;

            base.GetObjectData(info, context);
            info.AddValue("code", this.code);
            info.AddValue("message", this.message);
            info.AddValue("error", this.error);
        }

    }

    /// <summary>
    /// EOSIO API Error
    /// </summary>
    [Serializable]
    public class APIError
    {
        public int code;
        public string name;
        public string what;
        public List<APIErrorDetail> details;
    }


    /// <summary>
    /// EOSIO API Error detail
    /// </summary>
    [Serializable]
    public class APIErrorDetail
    {
        public string message;
        public string file;
        public int line_number;
        public string method;
    }

}
