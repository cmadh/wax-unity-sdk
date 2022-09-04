using StrattonStudios.AtomicAssetsUnity.Api;

namespace StrattonStudios.AtomicAssetsUnity
{

    /// <summary>
    /// The Atomic Assets client.
    /// </summary>
    /// <remarks>
    /// This is a convenience class to access the API methods through the <see cref="AtomicAssetsHttpApi"/>.
    /// </remarks>
    public class AtomicAssetsClient
    {

        #region Constants

        /// <summary>
        /// The main WAX URL endpoint.
        /// </summary>
        public const string WaxMainBaseUrl = "https://wax.api.atomicassets.io/atomicassets/v1";

        #endregion

        #region Fields

        /// <summary>
        /// The base URL.
        /// </summary>
        public readonly string BaseUrl;

        /// <summary>
        /// The HTTP API.
        /// </summary>
        public readonly AtomicAssetsHttpApi HttpApi;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new instance of <see cref="AtomicAssetsClient"/>.
        /// </summary>
        public AtomicAssetsClient() : this(WaxMainBaseUrl)
        {

        }

        /// <summary>
        /// Creates a new instance of <see cref="AtomicAssetsClient"/>.
        /// </summary>
        /// <param name="baseUrl">The base URL for the HTTP API</param>
        public AtomicAssetsClient(string baseUrl)
        {
            this.BaseUrl = baseUrl;
            this.HttpApi = new AtomicAssetsHttpApi(this.BaseUrl);
        }

        #endregion

    }

}
