using Cysharp.Threading.Tasks;

using StrattonStudios.EosioUnity.Models;

namespace StrattonStudios.EosioUnity.Signing
{

    /// <summary>
    /// The ABI schema provider interface.
    /// </summary>
    public interface IAbiProvider
    {

        /// <summary>
        /// Retrieves the ABI for the account.
        /// </summary>
        /// <param name="account">The account to retrieve the ABI for</param>
        /// <returns>Returns the ABI schema</returns>
        UniTask<Abi> GetAbi(string account);

    }

}