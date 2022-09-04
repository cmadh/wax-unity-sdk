using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

using Cysharp.Threading.Tasks;

using StrattonStudios.EosioUnity.Models;

using UnityEngine;

namespace StrattonStudios.EosioUnity.Utilities
{

    /// <summary>
    /// Transaction utility methods..
    /// </summary>
    public static class TransactionUtility
    {

        /// <summary>
        /// Get the ABI schemas used in the transaction.
        /// </summary>
        /// <param name="trx">The transaction to retrieve the ABI schemas for</param>
        /// <returns>Returns the ABI schemas used in the transaction</returns>
        public static UniTask<Abi[]> GetTransactionAbis(EosioClient client, Transaction trx)
        {
            var abiTasks = new List<UniTask<Abi>>();

            foreach (var action in trx.ContextFreeActions)
            {
                abiTasks.Add(AbiUtility.GetAbi(client, action.Account.Value));
            }

            foreach (var action in trx.Actions)
            {
                abiTasks.Add(AbiUtility.GetAbi(client, action.Account.Value));
            }

            return UniTask.WhenAll(abiTasks);
        }

    }

}