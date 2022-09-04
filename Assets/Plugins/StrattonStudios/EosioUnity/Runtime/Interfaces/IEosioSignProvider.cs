using System.Collections.Generic;

using Cysharp.Threading.Tasks;

namespace StrattonStudios.EosioUnity
{

    /// <summary>
    /// Interface that facilitates the signature provider to allow multiple signing implementations
    /// </summary>
    public interface IEosioSignProvider
    {

        /// <summary>
        /// Access public keys from the signature provider provided
        /// </summary>
        /// <returns>Returns a list of public keys that are utilized in the signing request</returns>
        UniTask<IEnumerable<string>> GetAvailableKeys();

        /// <summary>
        /// Allow the Signing of bytes utilizing the signature provider that has be supplied
        /// </summary>
        /// <param name="chainId">Chain ID that is specified</param>
        /// <param name="requiredKeys">Required public keys for signing transaction</param>
        /// <param name="serializedTransaction">Byte array of the serialized transaction that is to be signed</param>
        /// <param name="abiNames">Contract names that allow you to get ABI information</param>
        /// <returns>Returns a list of signatures per key that is used in the transaction</returns>
        UniTask<IEnumerable<string>> Sign(string chainId, IEnumerable<string> requiredKeys, byte[] serializedTransaction, IEnumerable<string> abiNames = null);

    }

}
