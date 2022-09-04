using System.Collections.Generic;

using StrattonStudios.EosioUnity.Models;

namespace StrattonStudios.AnchorLinkUnity
{

    public static class LinkAbiData
    {

        public static readonly Abi Types = new Abi()
        {
            Version = "eosio::abi/1.1",
            Types = new List<AbiType>(),
            Structs = new List<AbiStruct>()
            {
                new AbiStruct()
                {
                    name = "sealed_message",
                    @base = "",
                    fields = new List<AbiField>()
                    {
                        new AbiField()
                        {
                            name = "from",
                            type = "public_key"
                        },
                        new AbiField()
                        {
                            name = "nonce",
                            type = "uint64"
                        },
                        new AbiField()
                        {
                            name = "ciphertext",
                            type = "bytes"
                        },
                        new AbiField()
                        {
                            name = "checksum",
                            type = "uint32"
                        }
                    }
                },
                new AbiStruct()
                {
                    name = "link_create",
                    @base = "",
                    fields = new List<AbiField>()
                    {
                        new AbiField()
                        {
                            name = "session_name",
                            type = "name"
                        },
                        new AbiField()
                        {
                            name = "request_key",
                            type = "public_key"
                        }
                    }
                }, new AbiStruct()
                {
                    name = "link_info",
                    @base = "",
                    fields = new List<AbiField>()
                    {
                        new AbiField()
                        {
                            name = "expiration",
                            type = "time_point_sec"
                        }
                    }
                }
            }
        };

    }

}