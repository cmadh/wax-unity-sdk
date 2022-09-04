namespace StrattonStudios.WcwUnity
{

    [System.Serializable]
    public class WaxException : System.Exception
    {

        public WaxException() { }
        public WaxException(string message) : base(message) { }
        public WaxException(string message, System.Exception inner) : base(message, inner) { }
        protected WaxException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }

    }

}