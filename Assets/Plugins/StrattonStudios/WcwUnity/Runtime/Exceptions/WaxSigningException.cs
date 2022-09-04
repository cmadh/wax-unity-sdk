namespace StrattonStudios.WcwUnity
{

    [System.Serializable]
    public class WaxSigningException : WaxException
    {

        public WaxSigningException() { }
        public WaxSigningException(string message) : base(message) { }
        public WaxSigningException(string message, System.Exception inner) : base(message, inner) { }
        protected WaxSigningException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }

    }

}