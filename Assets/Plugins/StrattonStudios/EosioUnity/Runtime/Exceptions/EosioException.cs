using System;
using System.Runtime.Serialization;

namespace StrattonStudios.EosioUnity
{

    /// <summary>
    /// The base EOSIO exception.
    /// </summary>
    [Serializable]
    public class EosioException : Exception
    {

        public EosioException() { }

        public EosioException(string message) : base(message) { }

        public EosioException(string message, Exception inner) : base(message, inner) { }

        protected EosioException(
          SerializationInfo info,
          StreamingContext context) : base(info, context) { }

    }

}
