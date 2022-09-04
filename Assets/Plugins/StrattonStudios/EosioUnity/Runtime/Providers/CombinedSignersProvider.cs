using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Cysharp.Threading.Tasks;

namespace StrattonStudios.EosioUnity
{

    /// <summary>
    /// A single signature provider that combines multiple signature providers to complete all the signatures for a transaction
    /// </summary>
    public class CombinedSignersProvider : IEosioSignProvider
    {

        private List<IEosioSignProvider> Signers { get; set; }

        /// <summary>
        /// Creates the provider with a list of signature providers
        /// </summary>
        /// <param name="signers"></param>
        public CombinedSignersProvider(List<IEosioSignProvider> signers)
        {
            if (signers == null || signers.Count == 0)
            {
                throw new ArgumentNullException("Requires at least one signer.");
            }

            Signers = signers;
        }

        /// <summary>
        /// Get available public keys from the list of signature providers
        /// </summary>
        /// <returns>Returns the combined list of public keys</returns>
        public async UniTask<IEnumerable<string>> GetAvailableKeys()
        {
            var availableKeysListTasks = Signers.Select(s => s.GetAvailableKeys());
            var availableKeysList = await UniTask.WhenAll(availableKeysListTasks);
            return availableKeysList.SelectMany(k => k).Distinct();
        }

        /// <summary>
        /// Sign the transaction using the list of signature providers.
        /// </summary>
        /// <param name="chainId">The EOSIO Chain ID</param>
        /// <param name="requiredKeys">The required public keys for signing this transaction</param>
        /// <param name="signBytes">The signature bytes</param>
        /// <param name="abiNames">The ABI contract names to get the ABI information from</param>
        /// <returns>Returns the combined list of signatures per required keys</returns>
        public async UniTask<IEnumerable<string>> Sign(string chainId, IEnumerable<string> requiredKeys, byte[] signBytes, IEnumerable<string> abiNames = null)
        {
            var signatureTasks = Signers.Select(s => s.Sign(chainId, requiredKeys, signBytes, abiNames));
            var signatures = await UniTask.WhenAll(signatureTasks);
            return signatures.SelectMany(k => k).Distinct();
        }

    }

}
