using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Cysharp.Threading.Tasks;

using StrattonStudios.EosioUnity.Models;
using StrattonStudios.EosioUnity.Serialization;

using UnityEngine;

namespace StrattonStudios.EosioUnity.Utilities
{

    /// <summary>
    /// ABI utility methods..
    /// </summary>
    public static class AbiUtility
    {

        /// <summary>
        /// Gets the ABI schema by the smart contract's account name
        /// </summary>
        /// <param name="accountName">The account name</param>
        /// <returns>Returns the ABI schema for the smart contract's account name</returns>
        public static async UniTask<Abi> GetAbi(EosioClient client, string accountName)
        {
            if (string.IsNullOrEmpty(accountName))
            {
                return AbiData.GetCachedAbi();
            }
            var result = await client.ChainApi.GetRawAbi(new GetRawAbiRequest()
            {
                account_name = accountName
            });

            return UnpackAbi(result.abi);
        }

        /// <summary>
        /// Unpacks the ABI from the encoded string.
        /// </summary>
        /// <param name="packabi">The ABI encoded string</param>
        /// <returns>Returns the unpacked ABI schema</returns>
        public static Abi UnpackAbi(string packedAbi)
        {
            var data = Base64EncodingUtility.FromBase64FcString(packedAbi);

            using (MemoryStream ms = new MemoryStream(data))
            {
                using (BinaryReader reader = new BinaryReader(ms, Encoding.UTF8))
                {
                    return new Abi()
                    {
                        Version = AbiTypeReader.ReadString(reader),
                        Types = AbiTypeReader.ReadType<List<AbiType>>(reader),
                        Structs = AbiTypeReader.ReadType<List<AbiStruct>>(reader),
                        Actions = AbiTypeReader.ReadAbiActionList(reader),
                        Tables = AbiTypeReader.ReadAbiTableList(reader),
                        RicardianClauses = AbiTypeReader.ReadType<List<AbiRicardianClause>>(reader),
                        ErrorMessages = AbiTypeReader.ReadType<List<string>>(reader),
                        AbiExtensions = AbiTypeReader.ReadType<List<SerializableExtension>>(reader),
                        Variants = AbiTypeReader.ReadType<List<Variant>>(reader)
                    };
                }
            }
        }

    }

}