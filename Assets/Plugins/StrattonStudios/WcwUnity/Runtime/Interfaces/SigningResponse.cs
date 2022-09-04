namespace StrattonStudios.WcwUnity
{

    /// <summary>
    /// The signing response.
    /// </summary>
    public class SigningResponse
    {

        /// <summary>
        /// Whether the operation is verified.
        /// </summary>
        public readonly bool verified;

        /// <summary>
        /// The transaction that has been signed.
        /// </summary>
        public readonly byte[] serializedTransaction;

        /// <summary>
        /// The signatures.
        /// </summary>
        public readonly string[] signatures;

        /// <summary>
        /// The whitelisted contracts
        /// </summary>
        public readonly WhitelistedContract[] whitelistedContracts;

        /// <summary>
        /// Creates a new instance of <see cref="SigningResponse"/>.
        /// </summary>
        /// <param name="verified"></param>
        /// <param name="serializedTransaction"></param>
        /// <param name="signatures"></param>
        /// <param name="whitelistedContracts"></param>
        public SigningResponse(bool verified, byte[] serializedTransaction, string[] signatures, WhitelistedContract[] whitelistedContracts)
        {
            this.verified = verified;
            this.serializedTransaction = serializedTransaction;
            this.signatures = signatures;
        }

    }

}