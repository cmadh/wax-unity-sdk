namespace StrattonStudios.WcwUnity
{

    /// <summary>
    /// The login response.
    /// </summary>
    public class LoginResponse
    {

        /// <summary>
        /// Whether the login is verified.
        /// </summary>
        public readonly bool verified;

        /// <summary>
        /// The user's account
        /// </summary>
        public readonly string userAccount;

        /// <summary>
        /// The user's public keys.
        /// </summary>
        public readonly string[] pubKeys;

        /// <summary>
        /// Whether signed in using auto login.
        /// </summary>
        public readonly bool autoLogin;

        /// <summary>
        /// The whitelisted contracts.
        /// </summary>
        public readonly WhitelistedContract[] whitelistedContracts;

        /// <summary>
        /// Creates a new instance of <see cref="LoginResponse"/>.
        /// </summary>
        /// <param name="verified"></param>
        /// <param name="userAccount"></param>
        /// <param name="pubKeys"></param>
        /// <param name="autoLogin"></param>
        /// <param name="whitelistedContracts"></param>
        public LoginResponse(bool verified, string userAccount, string[] pubKeys, bool autoLogin, WhitelistedContract[] whitelistedContracts)
        {
            this.verified = verified;
            this.userAccount = userAccount;
            this.pubKeys = pubKeys;
            this.autoLogin = autoLogin;
            this.whitelistedContracts = whitelistedContracts;
        }

    }

}