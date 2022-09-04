using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

using Cysharp.Threading.Tasks;

using StrattonStudios.EosioUnity.Api;
using StrattonStudios.EosioUnity.Models;
using StrattonStudios.EosioUnity.Utilities;

namespace StrattonStudios.EosioUnity.Serialization
{

    public class AbiTypeReader
    {

        #region Delegates

        private delegate object ReadHandler(BinaryReader reader);

        #endregion

        #region Static Fields

        private static readonly char[] Charmap = new[] { '.', '1', '2', '3', '4', '5', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };

        private static readonly Dictionary<string, ReadHandler> CachedTypeReader = new Dictionary<string, ReadHandler>()
        {
                {"int8",                 WrapReader(ReadInt8)                },
                {"uint8",                WrapReader(ReadByte)                },
                {"int16",                WrapReader(ReadInt16)               },
                {"uint16",               WrapReader(ReadUInt16)              },
                {"int32",                WrapReader(ReadInt32)               },
                {"uint32",               WrapReader(ReadUInt32)              },
                {"int64",                WrapReader(ReadInt64)               },
                {"uint64",               WrapReader(ReadUInt64)              },
                {"int128",               WrapReader(ReadInt128)              },
                {"uint128",              WrapReader(ReadUInt128)             },
                {"varuint32",            WrapReader(ReadVarUInt32)           },
                {"varint32",             WrapReader(ReadVarInt32)            },
                {"float32",              WrapReader(ReadFloat32)             },
                {"float64",              WrapReader(ReadFloat64)             },
                {"float128",             WrapReader(ReadFloat128)            },
                {"bytes",                ReadBytes                           },
                {"bool",                 WrapReader(ReadBoolean)             },
                {"string",               ReadString                          },
                {"name",                 ReadName                            },
                {"asset",                ReadAsset                           },
                {"time_point",           WrapReader(ReadTimePoint)           },
                {"time_point_sec",       WrapReader(ReadTimePointSec)        },
                {"block_timestamp_type", WrapReader(ReadBlockTimestampType)  },
                {"symbol_code",          ReadSymbolCode                      },
                {"symbol",               ReadSymbolString                    },
                {"checksum160",          ReadChecksum160                     },
                {"checksum256",          ReadChecksum256                     },
                {"checksum512",          ReadChecksum512                     },
                {"public_key",           ReadPublicKey                       },
                {"private_key",          ReadPrivateKey                      },
                {"signature",            ReadSignature                       },
                {"permission_level",     ReadPermissionLevel                 },
                {"extended_asset",       ReadExtendedAsset                   },
        };

        #endregion

        #region Fields

        protected EosioClient client;

        #endregion

        #region Constructors

        public AbiTypeReader(EosioClient client)
        {
            this.client = client;
        }

        #endregion

        #region Built-in Readers

        private static ReadHandler WrapReader<TResult>(System.Func<BinaryReader, TResult> func)
        {
            return (BinaryReader reader) =>
            {
                return func(reader);
            };
        }

        //
        // Byte
        //

        public static byte ReadByte(BinaryReader reader)
        {
            return reader.ReadByte();
        }

        //
        // Boolean
        //

        public static bool ReadBoolean(BinaryReader reader)
        {
            return reader.ReadBoolean();
        }

        //
        // Numerics
        //

        public static sbyte ReadInt8(BinaryReader reader)
        {
            return reader.ReadSByte();
        }

        public static byte ReadUInt8(BinaryReader reader)
        {
            return reader.ReadByte();
        }

        public static short ReadInt16(BinaryReader reader)
        {
            return reader.ReadInt16();
        }

        public static ushort ReadUInt16(BinaryReader reader)
        {
            return reader.ReadUInt16();
        }

        public static int ReadInt32(BinaryReader reader)
        {
            return reader.ReadInt32();
        }

        public static uint ReadUInt32(BinaryReader reader)
        {
            return reader.ReadUInt32();
        }

        public static long ReadInt64(BinaryReader reader)
        {
            return reader.ReadInt64();
        }

        public static ulong ReadUInt64(BinaryReader reader)
        {
            return reader.ReadUInt64();
        }

        public static string ReadInt128(BinaryReader reader)
        {
            var bytes = reader.ReadBytes(16);
            //return new BigInteger(bytes);
            return BigNumberUtility.ConvertSignedBinaryToDecimal(bytes);
        }

        public static string ReadUInt128(BinaryReader reader)
        {
            var bytes = reader.ReadBytes(16);
            //return new BigInteger(bytes);
            return BigNumberUtility.ConvertBinaryToDecimal(bytes);
        }

        public static long ReadVarInt32(BinaryReader reader)
        {
            var v = ReadVarUInt32(reader);

            if ((v & 1) != 0)
                return (long)((~v) >> 1) | 0x80000000;
            else
                return (long)v >> 1;
        }

        public static ulong ReadVarUInt32(BinaryReader reader)
        {
            ulong v = 0;
            int bit = 0;
            while (true)
            {
                byte b = reader.ReadByte();
                v |= (uint)((b & 0x7f) << bit);
                bit += 7;
                if ((b & 0x80) == 0)
                    break;
            }
            return v >> 0;
        }

        //
        // Decimals
        //

        public static float ReadFloat32(BinaryReader reader)
        {
            return reader.ReadSingle();
        }

        public static double ReadFloat64(BinaryReader reader)
        {
            return reader.ReadDouble();
        }

        public static decimal ReadFloat128(BinaryReader reader)
        {
            return reader.ReadDecimal();
        }

        //
        // Time
        //

        public static System.DateTime ReadTimePoint(BinaryReader reader)
        {
            var low = ReadUInt32(reader);
            var high = ReadUInt32(reader);
            return EosioTimeUtility.ConvertTimePointToDate((high >> 0) * 0x100000000 + (low >> 0));
        }

        public static System.DateTime ReadTimePointSec(BinaryReader reader)
        {
            var secs = ReadUInt32(reader);
            return EosioTimeUtility.ConvertTimePointSecToDate(secs);
        }

        public static System.DateTime ReadBlockTimestampType(BinaryReader reader)
        {
            var slot = ReadUInt32(reader);
            return EosioTimeUtility.ConvertBlockTimestampToDate(slot);
        }

        //
        // String
        //

        public static string ReadString(BinaryReader reader)
        {
            return reader.ReadString();
        }

        public static string ReadName(BinaryReader reader)
        {
            var binary = System.BitConverter.ToUInt64(reader.ReadBytes(8), 0);
            var str = new[] { '.', '.', '.', '.', '.', '.', '.', '.', '.', '.', '.', '.', '.' };

            var tmp = binary;
            for (uint i = 0; i <= 12; ++i)
            {
                var c = Charmap[tmp & (ulong)(i == 0 ? 0x0f : 0x1f)];
                str[(int)(12 - i)] = c;
                tmp >>= (i == 0 ? 4 : 5);
            }

            return new string(str).TrimEnd('.');
        }

        //
        // Binary
        //

        public static byte[] ReadBytes(BinaryReader reader)
        {
            var size = System.Convert.ToInt32(ReadVarUInt32(reader));
            return reader.ReadBytes(size);
        }

        //
        // Checksums
        //

        public static string ReadChecksum160(BinaryReader reader)
        {
            var bytes = reader.ReadBytes(20);
            return HexUtility.ToHexString(bytes);
        }

        public static string ReadChecksum256(BinaryReader reader)
        {
            var bytes = reader.ReadBytes(32);
            return HexUtility.ToHexString(bytes);
        }

        public static string ReadChecksum512(BinaryReader reader)
        {
            var bytes = reader.ReadBytes(64);
            return HexUtility.ToHexString(bytes);
        }

        //
        // Keys
        //

        public static string ReadPublicKey(BinaryReader reader)
        {
            var type = ReadByte(reader);
            var keyBytes = reader.ReadBytes(KeyUtility.PublicKeySizeInBytes);

            if (type == (int)KeyType.K1)
            {
                return KeyUtility.ConvertPublicKeyBinaryToString(keyBytes, KeyType.K1);
            }
            if (type == (int)KeyType.R1)
            {
                return KeyUtility.ConvertPublicKeyBinaryToString(keyBytes, KeyType.R1);
            }
            else
            {
                throw new System.Exception("The public key type is not supported.");
            }
        }

        public static string ReadPrivateKey(BinaryReader reader)
        {
            var type = ReadByte(reader);
            var keyBytes = reader.ReadBytes(KeyUtility.PrivateKeySizeInBytes);

            if (type == (int)KeyType.R1)
            {
                return KeyUtility.ConvertPrivateKeyBinaryToString(keyBytes, KeyType.R1);
            }
            else
            {
                throw new System.Exception("The private key type is not supported.");
            }
        }

        public static string ReadSignature(BinaryReader reader)
        {
            var type = ReadByte(reader);
            var signBytes = reader.ReadBytes(KeyUtility.SignatureKeySizeInBytes);

            if (type == (int)KeyType.R1)
            {
                return KeyUtility.ConvertSignatureBinaryToString(signBytes, KeyType.R1);
            }
            else if (type == (int)KeyType.K1)
            {
                return KeyUtility.ConvertSignatureBinaryToString(signBytes, KeyType.K1);
            }
            else
            {
                throw new System.Exception("The signature type is not supported.");
            }
        }

        //
        // Symbols
        //


        public static string ReadSymbolString(BinaryReader reader)
        {
            var value = (Symbol)ReadSymbol(reader);
            return value.precision + ',' + value.name;
        }

        public static string ReadSymbolCode(BinaryReader reader)
        {
            byte[] a = reader.ReadBytes(8);

            int len;
            for (len = 0; len < a.Length; ++len)
                if (a[len] == 0)
                    break;

            return string.Join("", a.Take(len));
        }

        public static Symbol ReadSymbol(BinaryReader reader)
        {
            var value = new Symbol
            {
                precision = ReadByte(reader)
            };

            byte[] a = reader.ReadBytes(7);

            int len;
            for (len = 0; len < a.Length; ++len)
            {
                if (a[len] == 0)
                {
                    break;
                }
            }

            value.name = string.Join("", a.Take(len).Select(b => (char)b));

            return value;
        }

        //
        // Assets
        //

        public static string ReadAsset(BinaryReader reader)
        {
            byte[] amount = reader.ReadBytes(8);

            var symbol = ReadSymbol(reader);
            //string s = new BigInteger(amount).ToString();
            string s = BigNumberUtility.ConvertSignedBinaryToDecimal(amount, symbol.precision + 1);

            if (symbol.precision > 0)
            {
                s = s.Substring(0, s.Length - symbol.precision) + '.' + s.Substring(s.Length - symbol.precision);
            }

            return s + ' ' + symbol.name;
        }

        public static ExtendedAsset ReadExtendedAsset(BinaryReader reader)
        {
            return new ExtendedAsset()
            {
                quantity = ReadAsset(reader),
                contract = ReadName(reader)
            };
        }

        //
        // Permissions
        //

        public static PermissionLevel ReadPermissionLevel(BinaryReader reader)
        {
            var value = new PermissionLevel()
            {
                Actor = new AccountName(ReadName(reader)),
                Permission = new PermissionName(ReadName(reader)),
            };
            return value;
        }

        #endregion

        #region ABI Deserialization

        /// <summary>
        /// Deserializes the transaction from packed hexadecimal string format.
        /// </summary>
        /// <param name="packedTrx">The packed transaction in hexadecimal string format</param>
        /// <param name="abiMap">The ABI schema mappings</param>
        /// <returns>Returns the unpacked transaction</returns>
        public UniTask<Transaction> DeserializePackedTransaction(string packedTrx, Dictionary<string, Abi> abiMap = null)
        {
            var data = HexUtility.FromHexString(packedTrx);
            return DeserializePackedTransaction(data, abiMap);
        }

        /// <summary>
        /// Deserializes the transaction from packed binary format.
        /// </summary>
        /// <param name="data">The packed transaction in binary format</param>
        /// <param name="abiMap">The ABI schema mappings</param>
        /// <returns>Returns the unpacked transaction</returns>
        public async UniTask<Transaction> DeserializePackedTransaction(byte[] data, Dictionary<string, Abi> abiMap = null)
        {
            using (MemoryStream ms = new MemoryStream(data))
            {
                using (BinaryReader reader = new BinaryReader(ms, Encoding.UTF8))
                {
                    var trx = new Transaction();
                    trx.Expiration = (ReadTimePointSec(reader)).ToUniversalTime().ToString("yyyy-MM-dd'T'HH:mm:ss.ffffffZ", CultureInfo.InvariantCulture);
                    trx.RefBlockNum = ReadUInt16(reader);
                    trx.RefBlockPrefix = ReadUInt32(reader);
                    trx.MaxNetUsageWords = (uint)ReadVarUInt32(reader);
                    trx.MaxNetUsageMs = ReadByte(reader);
                    trx.DelaySec = (uint)ReadVarUInt32(reader);

                    var contextFreeActionsSize = System.Convert.ToInt32(ReadVarUInt32(reader));
                    trx.ContextFreeActions = new List<Action>(contextFreeActionsSize);

                    for (int i = 0; i < contextFreeActionsSize; i++)
                    {
                        var action = (Action)ReadActionHeader(reader);
                        Abi abi = await AbiUtility.GetAbi(this.client, action.Account.Value);

                        trx.ContextFreeActions.Add((Action)ReadAction(reader, action, abi));
                    }

                    var actionsSize = System.Convert.ToInt32(ReadVarUInt32(reader));
                    trx.Actions = new List<Action>(actionsSize);

                    for (int i = 0; i < actionsSize; i++)
                    {
                        var action = (Action)ReadActionHeader(reader);

                        Abi abi;
                        if (abiMap != null && abiMap.ContainsKey(action.Account.Value))
                        {
                            abi = abiMap[action.Account.Value];
                        }
                        else
                        {

                            abi = await AbiUtility.GetAbi(this.client, action.Account.Value);
                        }

                        trx.Actions.Add((Action)ReadAction(reader, action, abi));
                    }
                    return trx;
                }
            }
        }

        /// <summary>
        /// Unpacks the action data to dictionary.
        /// </summary>
        /// <param name="action">The action to unpack its data</param>
        /// <param name="data">The action data in binary format</param>
        /// <param name="abi">The ABI schema</param>
        /// <returns>Returns the unpacked action data in dictionary form</returns>
        /// <exception cref="ArgumentException"></exception>
        public Dictionary<string, object> UnpackActionData(Action action, byte[] data, Abi abi)
        {
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

            using (MemoryStream ms = new MemoryStream(data))
            {
                using (BinaryReader reader = new BinaryReader(ms, Encoding.UTF8))
                {
                    return ReadAbiStruct<Dictionary<string, object>>(reader, abiStruct, abi);
                }
            };
        }

        /// <summary>
        /// Deserializes the ABI structure data from the encoded hexadecimal string as dictionary.
        /// </summary>
        /// <param name="structType">The ABI struct type</param>
        /// <param name="dataHex">The data in hexadecimal string</param>
        /// <param name="abi">The ABI schema</param>
        /// <returns>Returns the deserialized ABI structure in dictionary form</returns>
        public Dictionary<string, object> DeserializeStructData(string structType, string dataHex, Abi abi)
        {
            return DeserializeStructData<Dictionary<string, object>>(structType, dataHex, abi);
        }

        /// <summary>
        /// Deserializes the ABI structure data from the binary data as dictionary.
        /// </summary>
        /// <param name="structType">The ABI struct type</param>
        /// <param name="data">The binary data</param>
        /// <param name="abi">The ABI schema</param>
        /// <returns>Returns the deserialized ABI structure in dictionary form</returns>
        public Dictionary<string, object> DeserializeStructData(string structType, byte[] data, Abi abi)
        {
            return DeserializeStructData<Dictionary<string, object>>(structType, data, abi);
        }

        /// <summary>
        /// Deserializes the ABI structure data from the encoded hexadecimal string.
        /// </summary>
        /// <typeparam name="TStructData">The struct object type</typeparam>
        /// <param name="structType">The ABI struct type</param>
        /// <param name="dataHex">The data in hexadecimal string</param>
        /// <param name="abi">The ABI schema</param>
        /// <returns>Returns the deserialized ABI structure as the object type specified</returns>
        public TStructData DeserializeStructData<TStructData>(string structType, string dataHex, Abi abi)
        {
            var data = HexUtility.FromHexString(dataHex);
            return DeserializeStructData<TStructData>(structType, data, abi);
        }

        /// <summary>
        /// Deserializes the ABI structure data from the binary data.
        /// </summary>
        /// <typeparam name="TStructData">The struct object type</typeparam>
        /// <param name="structType">The ABI struct type</param>
        /// <param name="data">The binary data</param>
        /// <param name="abi">The ABI schema</param>
        /// <returns>Returns the deserialized ABI structure as the object type specified</returns>
        public TStructData DeserializeStructData<TStructData>(string structType, byte[] data, Abi abi)
        {
            var abiStruct = abi.Structs.First(s => s.name == structType);
            using (MemoryStream ms = new MemoryStream(data))
            {
                using (BinaryReader reader = new BinaryReader(ms, Encoding.UTF8))
                {
                    return ReadAbiStruct<TStructData>(reader, abiStruct, abi);
                }
            }
        }

        public object DeserializeType(string typeName, string dataHex, Abi abi)
        {
            var data = HexUtility.FromHexString(dataHex);
            using (MemoryStream ms = new MemoryStream(data))
            {
                using (BinaryReader reader = new BinaryReader(ms, Encoding.UTF8))
                {
                    return ReadAbiType(reader, typeName, abi, true);
                }
            }
        }

        /// <summary>
        /// Deserializes the object with the provided type from the encoded hexadecimal string.
        /// </summary>
        /// <typeparam name="T">The type of object to deserialize</typeparam>
        /// <param name="dataHex">The hexadecimal data</param>
        /// <returns>Returns the deserialized object</returns>
        public T DeserializeType<T>(string dataHex)
        {
            return DeserializeType<T>(HexUtility.FromHexString(dataHex));
        }

        /// <summary>
        /// Deserializes the object with the provided type from binary data.
        /// </summary>
        /// <typeparam name="T">The type of object to deserialize</typeparam>
        /// <param name="data">The binary data</param>
        /// <returns>Returns the deserialized object</returns>
        public T DeserializeType<T>(byte[] data)
        {
            using (MemoryStream ms = new MemoryStream(data))
            {
                using (BinaryReader reader = new BinaryReader(ms, Encoding.UTF8))
                {
                    return ReadType<T>(reader);
                }
            }
        }

        private object ReadActionHeader(BinaryReader reader)
        {
            return new Action()
            {
                Account = new AccountName(ReadName(reader)),
                Name = new ActionName((ReadName(reader)))
            };
        }

        private object ReadAction(BinaryReader reader, Action action, Abi abi)
        {
            if (action == null)
                throw new System.ArgumentNullException("action");

            var size = System.Convert.ToInt32(ReadVarUInt32(reader));

            action.Authorization = new List<PermissionLevel>(size);
            for (var i = 0; i < size; i++)
            {
                action.AddAuthorization((PermissionLevel)ReadPermissionLevel(reader));
            }

            var abiAction = abi.Actions.First(aa => aa.Name == action.Name.Value);
            var abiStruct = abi.Structs.First(s => s.name == abiAction.type);

            var dataSize = System.Convert.ToInt32(ReadVarUInt32(reader));

            action.Data = new ActionData(ReadAbiStruct<Dictionary<string, object>>(reader, abiStruct, abi));
            //action.Data.SetData(ReadAbiStruct<Dictionary<string, object>>(data, abiStruct, abi, ref readIndex));

            return action;
        }

        public static List<AbiAction> ReadAbiActionList(BinaryReader reader)
        {
            var size = System.Convert.ToInt32(ReadVarUInt32(reader));
            List<AbiAction> items = new List<AbiAction>();

            for (int i = 0; i < size; i++)
            {
                items.Add(new AbiAction()
                {
                    Name = ReadName(reader),
                    type = ReadString(reader),
                    ricardian_contract = ReadString(reader)
                });
            }

            return items;
        }

        public static List<AbiTable> ReadAbiTableList(BinaryReader reader)
        {
            var size = System.Convert.ToInt32(ReadVarUInt32(reader));
            List<AbiTable> items = new List<AbiTable>();

            for (int i = 0; i < size; i++)
            {
                items.Add(new AbiTable()
                {
                    name = ReadName(reader),
                    index_type = ReadString(reader),
                    key_names = ReadType<List<string>>(reader),
                    key_types = ReadType<List<string>>(reader),
                    type = ReadString(reader)
                });
            }

            return items;
        }

        private object ReadAbiType(BinaryReader reader, string type, Abi abi, bool isBinaryExtensionAllowed)
        {
            var uwtype = AbiTypeUtility.UnwrapTypeDefinition(abi, type);
            string mainTypeName = uwtype;

            // Binary extension type
            if (uwtype.EndsWith("$"))
            {
                if (!isBinaryExtensionAllowed)
                {
                    throw new System.Exception("The binary Extension type is not allowed.");
                }
                return ReadAbiType(reader, uwtype.Substring(0, uwtype.Length - 1), abi, isBinaryExtensionAllowed);
            }

            // Optional type
            if (uwtype.EndsWith("?"))
            {
                var opt = (byte)ReadByte(reader);
                mainTypeName = mainTypeName.Remove(mainTypeName.Length - 1);

                if (opt == 0)
                {
                    return null;
                }
            }

            // Array type
            if (uwtype.EndsWith("[]"))
            {
                var arrayType = uwtype.Substring(0, uwtype.Length - 2);
                var size = System.Convert.ToInt32(ReadVarUInt32(reader));
                var items = new List<object>(size);

                for (int i = 0; i < size; i++)
                {
                    items.Add(ReadAbiType(reader, arrayType, abi, false));
                }

                return items;
            }

            var typeReader = GetTypeReader(mainTypeName, CachedTypeReader, abi);

            if (typeReader != null)
            {
                return typeReader(reader);
            }

            var abiStruct = abi.Structs.FirstOrDefault(s => s.name == uwtype);
            if (abiStruct != null)
            {
                return ReadAbiStruct(reader, abiStruct, abi);
            }

            var abiVariant = abi.Variants.FirstOrDefault(v => v.name == uwtype);
            if (abiVariant != null)
            {
                return ReadAbiVariant(reader, abiVariant, abi, isBinaryExtensionAllowed);
            }
            else
            {
                throw new System.Exception("The type has no supported reader.");
            }
        }

        private object ReadAbiStruct(BinaryReader reader, AbiStruct abiStruct, Abi abi)
        {
            return ReadAbiStruct<Dictionary<string, object>>(reader, abiStruct, abi);
        }

        private T ReadAbiStruct<T>(BinaryReader reader, AbiStruct abiStruct, Abi abi)
        {
            object value = default(T);

            if (!string.IsNullOrWhiteSpace(abiStruct.@base))
            {
                value = (T)ReadAbiType(reader, abiStruct.@base, abi, true);
            }
            else
            {
                value = System.Activator.CreateInstance(typeof(T));
            }

            if (value is IDictionary<string, object>)
            {
                var valueDict = value as IDictionary<string, object>;
                foreach (var field in abiStruct.fields)
                {
                    var abiValue = ReadAbiType(reader, field.type, abi, true);
                    if (field.type.EndsWith("$") && abiValue == null) break;
                    valueDict.Add(field.name, abiValue);
                }
            }
            else
            {
                var valueType = value.GetType();
                foreach (var field in abiStruct.fields)
                {
                    var abiValue = ReadAbiType(reader, field.type, abi, true);
                    if (field.type.EndsWith("$") && abiValue == null) break;
                    var fieldName = AbiTypeUtility.FindObjectFieldName(field.name, value.GetType());
                    valueType.GetField(fieldName).SetValue(value, abiValue);
                }
            }

            return (T)value;
        }

        private object ReadAbiVariant(BinaryReader reader, Variant abiVariant, Abi abi, bool isBinaryExtensionAllowed)
        {
            var i = (uint)ReadVarUInt32(reader);
            if (i >= abiVariant.types.Count)
            {
                throw new System.Exception("type index " + i + " is not valid for variant");
            }
            var type = abiVariant.types[(int)i];
            var value = ReadAbiType(reader, type, abi, isBinaryExtensionAllowed);
            return new List<object>() { type, value };
            //return new KeyValuePair<string, object>(abiVariant.name, value);
        }

        public static T ReadType<T>(BinaryReader reader)
        {
            return (T)ReadType(reader, typeof(T));
        }

        public static object ReadType(BinaryReader reader, System.Type objectType)
        {
            if (IsCollection(objectType))
            {
                return ReadCollectionType(reader, objectType);
            }
            else if (IsOptional(objectType))
            {
                var opt = (byte)ReadByte(reader);
                if (opt == 1)
                {
                    var optionalType = GetFirstGenericType(objectType);
                    return ReadType(reader, optionalType);
                }
            }
            else if (IsPrimitive(objectType))
            {
                var readerName = GetNormalizedReaderName(objectType);
                return CachedTypeReader[readerName](reader);
            }

            var value = System.Activator.CreateInstance(objectType);

            foreach (var member in objectType.GetFields())
            {
                if (IsCollection(member.FieldType))
                {
                    objectType.GetField(member.Name).SetValue(value, ReadCollectionType(reader, member.FieldType));
                }
                else if (IsOptional(member.FieldType))
                {
                    var opt = ReadByte(reader);
                    if (opt == 1)
                    {
                        var optionalType = GetFirstGenericType(member.FieldType);
                        objectType.GetField(member.Name).SetValue(value, ReadType(reader, optionalType));
                    }
                }
                else if (IsPrimitive(member.FieldType))
                {
                    var readerName = GetNormalizedReaderName(member.FieldType, member.GetCustomAttributes());
                    objectType.GetField(member.Name).SetValue(value, CachedTypeReader[readerName](reader));
                }
                else
                {
                    objectType.GetField(member.Name).SetValue(value, ReadType(reader, member.FieldType));
                }
            }

            return value;
        }

        private static IList ReadCollectionType(BinaryReader reader, System.Type objectType)
        {
            var collectionType = GetFirstGenericType(objectType);
            var size = System.Convert.ToInt32(ReadVarUInt32(reader));
            IList items = (IList)System.Activator.CreateInstance(objectType);

            for (int i = 0; i < size; i++)
            {
                items.Add(ReadType(reader, collectionType));
            }

            return items;
        }

        private static bool IsCollection(System.Type type)
        {
            return type.Name.StartsWith("List");
        }

        private static bool IsOptional(System.Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(System.Nullable<>);
        }

        private static System.Type GetFirstGenericType(System.Type type)
        {
            return type.GetGenericArguments().First();
        }

        private static bool IsPrimitive(System.Type type)
        {
            return type.IsPrimitive ||
                   type.Name.ToLower() == "string" ||
                   type.Name.ToLower() == "byte[]";
        }

        private static string GetNormalizedReaderName(System.Type type, IEnumerable<System.Attribute> customAttributes = null)
        {
            if (customAttributes != null)
            {
                var abiFieldAttr = (EosioAbiFieldTypeAttribute)customAttributes.FirstOrDefault(attr => attr.GetType() == typeof(EosioAbiFieldTypeAttribute));
                if (abiFieldAttr != null)
                {
                    return abiFieldAttr.AbiType;
                }
            }

            var typeName = type.Name.ToLower();

            if (typeName == "byte[]")
            {
                return "bytes";
            }
            else if (typeName == "boolean")
            {
                return "bool";
            }
            return typeName;
        }

        private TSerializer GetTypeReader<TSerializer>(string type, Dictionary<string, TSerializer> typeSerializers, Abi abi)
        {
            TSerializer nativeSerializer;
            if (typeSerializers.TryGetValue(type, out nativeSerializer))
            {
                return nativeSerializer;
            }

            var abiTypeDef = abi.Types.FirstOrDefault(t => t.new_type_name == type);
            if (abiTypeDef != null)
            {
                var serializer = GetTypeReader(abiTypeDef.type, typeSerializers, abi);

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