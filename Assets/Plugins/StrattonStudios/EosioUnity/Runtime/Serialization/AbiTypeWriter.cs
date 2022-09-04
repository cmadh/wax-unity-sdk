using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;

using Cysharp.Threading.Tasks;

using StrattonStudios.EosioUnity.Api;
using StrattonStudios.EosioUnity.Models;
using StrattonStudios.EosioUnity.Utilities;

namespace StrattonStudios.EosioUnity.Serialization
{

    public class AbiTypeWriter
    {

        private delegate void WriteHandler(BinaryWriter writer, object data);

        private static Dictionary<string, WriteHandler> CachedTypeWriters = new Dictionary<string, WriteHandler>()
        {
                {"int8",                 WriteInt8               },
                {"uint8",                WriteUInt8              },
                {"int16",                WriteInt16              },
                {"uint16",               WriteUInt16             },
                {"int32",                WriteInt32              },
                {"uint32",               WriteUInt32             },
                {"int64",                WriteInt64              },
                {"uint64",               WriteUInt64             },
                {"int128",               WriteInt128             },
                {"uint128",              WriteUInt128            },
                {"varuint32",            WriteVarUInt32          },
                {"varint32",             WriteVarInt32           },
                {"float32",              WriteFloat32            },
                {"float64",              WriteFloat64            },
                {"float128",             WriteFloat128           },
                {"bytes",                WriteBytes              },
                {"bool",                 WriteBoolean            },
                {"string",               WriteString             },
                {"name",                 WriteName               },
                {"asset",                WriteAsset              },
                {"time_point",           WriteTimePoint          },
                {"time_point_sec",       WriteTimePointSec       },
                {"block_timestamp_type", WriteBlockTimestampType },
                {"symbol_code",          WriteSymbolCode         },
                {"symbol",               WriteSymbolString       },
                {"checksum160",          WriteChecksum160        },
                {"checksum256",          WriteChecksum256        },
                {"checksum512",          WriteChecksum512        },
                {"public_key",           WritePublicKey          },
                {"private_key",          WritePrivateKey         },
                {"signature",            WriteSignature          },
                {"permission_level",     WritePermissionLevel    },
                {"extended_asset",       WriteExtendedAsset      }
        };

        #region Fields

        protected EosioClient client;

        #endregion

        #region Constructors

        public AbiTypeWriter(EosioClient client)
        {
            this.client = client;
        }

        #endregion

        #region Built-in Writers

        //
        // Byte
        //

        public static void WriteByte(BinaryWriter writer, object data)
        {
            var value = System.Convert.ToByte(data);
            writer.Write(value);
        }

        //
        // Boolean
        //

        public static void WriteBoolean(BinaryWriter writer, object data)
        {
            var value = System.Convert.ToBoolean(data);
            writer.Write(value);
        }

        //
        // Numerics
        //

        public static void WriteInt8(BinaryWriter writer, object data)
        {
            var value = System.Convert.ToSByte(data);
            writer.Write(value);
        }

        public static void WriteUInt8(BinaryWriter writer, object data)
        {
            var value = System.Convert.ToByte(data);
            writer.Write(value);
        }

        public static void WriteInt16(BinaryWriter writer, object data)
        {
            var value = System.Convert.ToInt16(data);
            writer.Write(value);
        }

        public static void WriteUInt16(BinaryWriter writer, object data)
        {
            var value = System.Convert.ToUInt16(data);
            writer.Write(value);
        }

        public static void WriteInt32(BinaryWriter writer, object data)
        {
            var value = System.Convert.ToInt32(data);
            writer.Write(value);
        }

        public static void WriteUInt32(BinaryWriter writer, object data)
        {
            var value = System.Convert.ToUInt32(data);
            writer.Write(value);
        }

        public static void WriteInt64(BinaryWriter writer, object data)
        {
            var value = System.Convert.ToInt64(data);
            writer.Write(value);
        }

        public static void WriteUInt64(BinaryWriter writer, object data)
        {
            var value = System.Convert.ToUInt64(data);
            writer.Write(value);
        }

        public static void WriteInt128(BinaryWriter writer, object data)
        {
            var decimalBytes = BigNumberUtility.ConvertSignedDecimalToBinary(16, data.ToString());
            //var value = BigInteger.Parse(data.ToString(), CultureInfo.InvariantCulture).ToByteArray();
            writer.Write(decimalBytes);
        }

        public static void WriteUInt128(BinaryWriter writer, object data)
        {
            var decimalBytes = BigNumberUtility.ConvertDecimalToBinary(16, data.ToString());
            //var value = BigInteger.Parse(data.ToString(), CultureInfo.InvariantCulture).ToByteArray();
            writer.Write(decimalBytes);
        }

        public static void WriteVarInt32(BinaryWriter writer, object data)
        {
            var value = VarintConverter.GetVarintBytes(System.Convert.ToInt32(data));
            writer.Write(value);
        }

        public static void WriteVarUInt32(BinaryWriter writer, object data)
        {
            var value = VarintConverter.GetVarintBytes(System.Convert.ToUInt32(data));
            writer.Write(value);
        }

        //
        // Decimals
        //

        public static void WriteFloat32(BinaryWriter writer, object data)
        {
            var value = System.Convert.ToSingle(data);
            writer.Write(value);
        }

        public static void WriteFloat64(BinaryWriter writer, object data)
        {
            var value = System.Convert.ToDouble(data);
            writer.Write(value);
        }

        public static void WriteFloat128(BinaryWriter writer, object data)
        {
            var value = System.Convert.ToDecimal(data);
            writer.Write(value);
        }

        //
        // Time
        //

        public static void WriteTimePoint(BinaryWriter writer, object data)
        {
            var value = EosioTimeUtility.ConvertDateToTimePoint((System.DateTime)data);
            WriteUInt32(writer, (uint)(value & 0xffffffff));
            WriteUInt32(writer, (uint)System.Math.Floor((double)value / 0x100000000));
        }

        public static void WriteTimePointSec(BinaryWriter writer, object data)
        {
            var value = EosioTimeUtility.ConvertDateToTimePointSec((System.DateTime)data);
            WriteUInt32(writer, value);
        }

        public static void WriteBlockTimestampType(BinaryWriter writer, object data)
        {
            var value = EosioTimeUtility.ConvertDateToBlockTimestamp((System.DateTime)data);
            WriteUInt32(writer, value);
        }

        //
        // String
        //

        public static void WriteString(BinaryWriter writer, object data)
        {
            var value = (string)data;
            writer.Write(value);

            //var bytes = Encoding.UTF8.GetBytes(value);
            //WriteVarUInt32(writer, (uint)bytes.Length);
            //if (bytes.Length > 0)
            //{
            //    writer.Write(bytes, 0, bytes.Length);
            //}
        }

        public static void WriteName(BinaryWriter writer, object data)
        {
            var value = (string)data;
            var a = EosioNameUtility.ConvertNameToBytes(value);
            writer.Write(a, 0, 8);
        }

        //
        // Binary
        //

        public static void WriteBytes(BinaryWriter writer, object data)
        {
            byte[] bytes;
            if (data is string)
            {
                bytes = HexUtility.FromHexString((string)data);
            }
            else
            {
                bytes = (byte[])data;
            }
            WriteVarUInt32(writer, (uint)bytes.Length);
            writer.Write(bytes, 0, bytes.Length);
        }

        //
        // Checksums
        //

        public static void WriteChecksum160(BinaryWriter writer, object data)
        {
            var bytes = HexUtility.FromHexString((string)data);
            if (bytes.Length != 20)
            {
                throw new System.Exception("The hexadecimal string's binary data has incorrect size");
            }
            writer.Write(bytes, 0, bytes.Length);
        }

        public static void WriteChecksum256(BinaryWriter writer, object data)
        {
            var bytes = HexUtility.FromHexString((string)data);
            if (bytes.Length != 32)
            {
                throw new System.Exception("The hexadecimal string's binary data has incorrect size");
            }
            writer.Write(bytes, 0, bytes.Length);
        }

        public static void WriteChecksum512(BinaryWriter writer, object data)
        {
            var bytes = HexUtility.FromHexString((string)data);
            if (bytes.Length != 64)
            {
                throw new System.Exception("The hexadecimal string's binary data has incorrect size");
            }
            writer.Write(bytes, 0, bytes.Length);
        }

        //
        // Keys
        //

        public static void WritePublicKey(BinaryWriter writer, object data)
        {
            var value = (string)data;
            var keyBytes = KeyUtility.ConvertPublicKeyStringToBinary(value);
            WriteByte(writer, (byte)(value.StartsWith(KeyUtility.PublicR1Prefix) ? KeyType.R1 : KeyType.K1));
            writer.Write(keyBytes, 0, KeyUtility.PublicKeySizeInBytes);
        }

        public static void WritePrivateKey(BinaryWriter writer, object data)
        {
            var value = (string)data;
            var keyBytes = KeyUtility.ConvertPrivateKeyStringToBinary(value);
            var keyType = KeyType.R1;
            if (value.StartsWith(KeyUtility.PrivateK1Prefix))
            {
                keyType = KeyType.K1;
            }
            WriteByte(writer, keyType);
            writer.Write(keyBytes, 0, KeyUtility.PrivateKeySizeInBytes);
        }

        public static void WriteSignature(BinaryWriter writer, object data)
        {
            var value = (string)data;
            var signBytes = KeyUtility.ConvertSignatureStringToBinary(value);
            if (value.StartsWith(KeyUtility.SignatureK1Prefix))
            {
                WriteByte(writer, KeyType.K1);
            }
            else if (value.StartsWith(KeyUtility.SignatureR1Prefix))
            {
                WriteByte(writer, KeyType.R1);
            }
            writer.Write(signBytes, 0, KeyUtility.SignatureKeySizeInBytes);
        }

        //
        // Symbols
        //


        public static void WriteSymbolString(BinaryWriter writer, object data)
        {
            var value = (string)data;
            Regex r = new Regex("^([0-9]+),([A-Z]+)$", RegexOptions.IgnoreCase);
            Match m = r.Match(value);
            if (!m.Success)
            {
                throw new System.Exception("The symbol is invalid.");
            }
            WriteSymbol(writer, new Symbol() { name = m.Groups[2].ToString(), precision = byte.Parse(m.Groups[1].ToString()) });
        }

        public static void WriteSymbolCode(BinaryWriter writer, object data)
        {
            var value = (string)data;

            if (value.Length > 8)
                writer.Write(Encoding.UTF8.GetBytes(value.Substring(0, 8)), 0, 8);
            else
            {
                writer.Write(Encoding.UTF8.GetBytes(value), 0, value.Length);
                if (value.Length < 8)
                {
                    var fill = new byte[8 - value.Length];
                    for (int i = 0; i < fill.Length; i++)
                    {
                        fill[i] = 0;
                    }
                    writer.Write(fill, 0, fill.Length);
                }
            }
        }

        public static void WriteSymbol(BinaryWriter writer, object data)
        {
            var symbol = (Symbol)data;
            WriteByte(writer, symbol.precision);
            if (symbol.name.Length > 7)
            {
                writer.Write(Encoding.UTF8.GetBytes(symbol.name.Substring(0, 7)), 0, 7);
            }
            else
            {
                writer.Write(Encoding.UTF8.GetBytes(symbol.name), 0, symbol.name.Length);
                if (symbol.name.Length < 7)
                {
                    var fill = new byte[7 - symbol.name.Length];
                    for (int i = 0; i < fill.Length; i++)
                    {
                        fill[i] = 0;
                    }
                    writer.Write(fill, 0, fill.Length);
                }
            }
        }

        //
        // Assets
        //

        public static void WriteAsset(BinaryWriter writer, object data)
        {
            var value = ((string)data).Trim();
            int pos = 0;
            string amount = "";
            byte precision = 0;

            if (value[pos] == '-')
            {
                amount += '-';
                ++pos;
            }

            bool foundDigit = false;
            while (pos < value.Length && value[pos] >= '0' && value[pos] <= '9')
            {
                foundDigit = true;
                amount += value[pos];
                ++pos;
            }

            if (!foundDigit)
            {
                throw new System.Exception("The asset must begin with a number");
            }

            if (value[pos] == '.')
            {
                ++pos;
                while (pos < value.Length && value[pos] >= '0' && value[pos] <= '9')
                {
                    amount += value[pos];
                    ++precision;
                    ++pos;
                }
            }

            string name = value.Substring(pos).Trim();

            var decimalBytes = BigNumberUtility.ConvertSignedDecimalToBinary(8, amount);
            writer.Write(decimalBytes);
            //var bigInt = BigInteger.Parse(amount);
            //writer.Write(bigInt.ToByteArray());
            WriteSymbol(writer, new Symbol() { name = name, precision = precision });
        }

        public static void WriteExtendedAsset(BinaryWriter writer, object data)
        {
            var value = (ExtendedAsset)data;
            WriteAsset(writer, value.quantity);
            WriteName(writer, value.contract);
        }

        //
        // Permissions
        //

        public static void WritePermissionLevel(BinaryWriter writer, object data)
        {
            string actor;
            string permission;
            if (data is Dictionary<string, string>)
            {
                var dictionary = data as Dictionary<string, string>;
                actor = dictionary[PermissionLevel.ActorPropertyName];
                permission = dictionary[PermissionLevel.PermissionPropertyName];
            }
            else
            {
                var auth = (PermissionLevel)data;
                actor = auth.Actor.Value;
                permission = auth.Permission.Value;
            }
            WriteName(writer, actor);
            WriteName(writer, permission);
        }

        //
        // Extensions
        //

        public static void WriteExtension(BinaryWriter writer, object data)
        {
            var extension = (Extension)data;
            if (extension.Data == null)
                return;

            WriteUInt16(writer, extension.Type);
            WriteBytes(writer, extension.Data);
        }

        #endregion

        #region ABI Serialization

        /// <summary>
        /// Serializes a transaction to packed binary format.
        /// </summary>
        /// <param name="trx">The transaction to serialize</param>
        /// <returns>Returns the transaction in packed binary format</returns>
        public async UniTask<byte[]> SerializeTransaction(Transaction trx, Dictionary<string, Abi> abiMap = null)
        {
            int actionIndex = 0;
            Abi[] abiResponses = null;
            if (abiMap == null)
            {
                abiResponses = await TransactionUtility.GetTransactionAbis(this.client, trx);
            }

            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(ms, Encoding.UTF8))
                {
                    // Transaction Headers
                    WriteUInt32(writer, EosioTimeUtility.ConvertDateToTimePointSec(System.DateTime.Parse(trx.Expiration, System.Globalization.CultureInfo.InvariantCulture)));
                    WriteUInt16(writer, trx.RefBlockNum);
                    WriteUInt32(writer, trx.RefBlockPrefix);

                    // Transaction Information
                    WriteVarUInt32(writer, trx.MaxNetUsageWords);
                    WriteByte(writer, trx.MaxNetUsageMs);
                    WriteVarUInt32(writer, trx.DelaySec);

                    WriteVarUInt32(writer, (uint)trx.ContextFreeActions.Count);
                    foreach (var action in trx.ContextFreeActions)
                    {
                        Abi abi;
                        if (action.IsIdentity())
                        {
                            abi = AbiData.GetCachedAbi();
                        }
                        else if (abiMap != null && abiMap.ContainsKey(action.Account.Value))
                        {
                            abi = abiMap[action.Account.Value];
                        }
                        else
                        {
                            abi = abiResponses[actionIndex];
                        }
                        WriteAction(writer, action, abi);
                        actionIndex++;
                    }

                    WriteVarUInt32(writer, (uint)trx.Actions.Count);
                    foreach (var action in trx.Actions)
                    {
                        Abi abi;
                        if (action.IsIdentity())
                        {
                            abi = AbiData.GetCachedAbi();
                        }
                        else if (abiMap != null && abiMap.ContainsKey(action.Account.Value))
                        {
                            abi = abiMap[action.Account.Value];
                        }
                        else
                        {
                            abi = abiResponses[actionIndex];
                        }
                        WriteAction(writer, action, abi);
                        actionIndex++;
                    }

                    WriteVarUInt32(writer, (uint)trx.TransactionExtensions.Count);
                    foreach (var extension in trx.TransactionExtensions)
                    {
                        WriteExtension(writer, extension);
                    }

                    return ms.ToArray();
                }
            }
        }

        public byte[] SerializeStructData(string structType, object value, Abi abi)
        {
            var abiStruct = abi.Structs.First(s => s.name == structType);
            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(ms, Encoding.UTF8))
                {
                    WriteAbiStruct(writer, value, abiStruct, abi);
                }
                return ms.ToArray();
            }
        }

        public byte[] SerializeType(string typeName, object value, Abi abi)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(ms, Encoding.UTF8))
                {
                    WriteAbiType(writer, value, typeName, abi, true);
                }
                return ms.ToArray();
            }
        }

        /// <summary>
        /// Packs the action data and returns it in binary format.
        /// </summary>
        /// <param name="action">The action to pack</param>
        /// <param name="abi">The ABI information</param>
        /// <returns>Returns the action data in binary format</returns>
        public byte[] PackActionData(Action action, Abi abi)
        {
            if (action.Data.IsPacked())
            {
                return HexUtility.FromHexString(action.Data.GetPackedData());
            }
            var abiAction = abi.Actions.FirstOrDefault(aa => aa.Name == action.Name.Value);

            if (abiAction == null)
            {
                throw new System.ArgumentException(string.Format("The action name {0} was not found on ABI.", action.Name.Value));
            }

            var abiStruct = abi.Structs.FirstOrDefault(s => s.name == abiAction.type);

            if (abiStruct == null)
            {
                throw new System.ArgumentException(string.Format("The struct type {0} was not found on ABI.", abiAction.type));
            }

            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(ms, Encoding.UTF8))
                {
                    WriteAbiStruct(writer, action.Data.GetData(), abiStruct, abi);
                }
                return ms.ToArray();
            }
        }

        //
        // Actions
        //

        public void WriteAction(BinaryWriter writer, Action action, Abi abi)
        {
            WriteName(writer, action.Account.Value);
            WriteName(writer, action.Name.Value);

            WriteVarUInt32(writer, (uint)action.Authorization.Count);
            foreach (var perm in action.Authorization)
            {
                WritePermissionLevel(writer, perm);
            }

            WriteBytes(writer, PackActionData(action, abi));
        }

        private void WriteAbiType(BinaryWriter writer, object data, string type, Abi abi, bool isBinaryExtensionAllowed)
        {
            var uwtype = AbiTypeUtility.UnwrapTypeDefinition(abi, type);

            // Binary extension type
            if (uwtype.EndsWith("$"))
            {
                if (!isBinaryExtensionAllowed)
                {
                    throw new System.Exception("The binary Extension type is not allowed.");
                }
                WriteAbiType(writer, data, uwtype.Substring(0, uwtype.Length - 1), abi, isBinaryExtensionAllowed);

                return;
            }

            // Optional type
            if (uwtype.EndsWith("?"))
            {
                if (data != null)
                {
                    WriteByte(writer, (byte)1);
                    type = uwtype.Substring(0, uwtype.Length - 1);
                }
                else
                {
                    WriteByte(writer, (byte)0);
                    return;
                }
            }

            // Array type
            if (uwtype.EndsWith("[]"))
            {
                var items = (ICollection)data;
                var arrayType = uwtype.Substring(0, uwtype.Length - 2);

                WriteVarUInt32(writer, (uint)items.Count);
                foreach (var item in items)
                    WriteAbiType(writer, item, arrayType, abi, false);

                return;
            }

            var typeWriter = GetTypeWriter(type, CachedTypeWriters, abi);
            if (typeWriter != null)
            {
                typeWriter(writer, data);
                return;
            }

            var abiStruct = abi.Structs.FirstOrDefault(s => s.name == uwtype);
            if (abiStruct != null)
            {
                WriteAbiStruct(writer, data, abiStruct, abi);
                return;
            }

            var abiVariant = abi.Variants.FirstOrDefault(v => v.name == uwtype);
            if (abiVariant != null)
            {
                WriteAbiVariant(writer, data, abiVariant, abi, isBinaryExtensionAllowed);
            }
            else
            {
                throw new System.Exception("The type has no supported writer: " + type);
            }
        }

        private void WriteAbiStruct(BinaryWriter writer, object data, AbiStruct abiStruct, Abi abi)
        {
            if (data == null)
            {
                return;
            }

            if (!string.IsNullOrWhiteSpace(abiStruct.@base))
            {
                WriteAbiType(writer, data, abiStruct.@base, abi, true);
            }

            if (data is System.Collections.IDictionary)
            {
                var skippedBinaryExtension = false;
                var valueDict = data as System.Collections.IDictionary;
                foreach (var field in abiStruct.fields)
                {
                    var fieldName = AbiTypeUtility.FindObjectFieldName(field.name, valueDict);
                    if (string.IsNullOrWhiteSpace(fieldName))
                    {
                        if (field.type.EndsWith("$"))
                        {
                            skippedBinaryExtension = true;
                            continue;
                        }
                        throw new System.Exception("The ABI struct field is missing: " + abiStruct.name + "." + field.name + " (type=" + field.type + ")");
                    }
                    else if (skippedBinaryExtension)
                    {
                        throw new System.Exception("Unexpected " + abiStruct.name + "." + field.name + " (type=" + field.type + ")");
                    }
                    WriteAbiType(writer, valueDict[fieldName], field.type, abi, true);
                }
            }
            else
            {
                var valueType = data.GetType();
                foreach (var field in abiStruct.fields)
                {
                    var fieldInfo = valueType.GetField(field.name);
                    if (fieldInfo != null)
                    {
                        WriteAbiType(writer, fieldInfo.GetValue(data), field.type, abi, true);
                    }
                    else
                    {
                        var propName = AbiTypeUtility.FindObjectPropertyName(field.name, valueType);
                        var propInfo = valueType.GetProperty(propName);
                        if (propInfo != null)
                        {
                            WriteAbiType(writer, propInfo.GetValue(data), field.type, abi, true);
                        }
                        else
                        {
                            throw new System.Exception("The object's property for the ABI field is missing: " + abiStruct.name + "." + field.name + " (type=" + field.type + ")");
                        }
                    }
                }
            }
        }

        private void WriteAbiVariant(BinaryWriter writer, object data, Variant abiVariant, Abi abi, bool isBinaryExtensionAllowed)
        {
            string variantType;
            object variantData;
            if (data is List<object>)
            {
                var list = data as List<object>;
                variantType = (string)list[0];
                variantData = list[1];
            }
            else
            {
                var pair = (KeyValuePair<string, object>)data;
                variantType = pair.Key;
                variantData = pair.Value;
            }
            var i = abiVariant.types.IndexOf(variantType);
            if (i < 0)
            {
                throw new System.Exception("The type " + variantType + " is not valid for variant");
            }
            WriteVarUInt32(writer, (uint)i);
            WriteAbiType(writer, variantData, variantType, abi, isBinaryExtensionAllowed);
        }

        private TSerializer GetTypeWriter<TSerializer>(string type, Dictionary<string, TSerializer> typeSerializers, Abi abi)
        {
            TSerializer nativeSerializer;
            if (typeSerializers.TryGetValue(type, out nativeSerializer))
            {
                return nativeSerializer;
            }

            var abiTypeDef = abi.Types.FirstOrDefault(t => t.new_type_name == type);
            if (abiTypeDef != null)
            {
                var serializer = GetTypeWriter(abiTypeDef.type, typeSerializers, abi);

                if (serializer != null)
                {
                    typeSerializers.Add(type, serializer);
                    return serializer;
                }
            }
            return default(TSerializer);
        }

        #endregion

    }

}