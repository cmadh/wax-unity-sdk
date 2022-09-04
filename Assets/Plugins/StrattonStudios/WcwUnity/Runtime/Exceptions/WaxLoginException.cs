namespace StrattonStudios.WcwUnity
{

    [System.Serializable]
    public class WaxLoginException : WaxException
    {

        public WaxLoginException() { }
        public WaxLoginException(string message) : base(message) { }
        public WaxLoginException(string message, System.Exception inner) : base(message, inner) { }
        protected WaxLoginException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }

    }

}