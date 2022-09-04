namespace StrattonStudios.EosioUnity
{

    /// <summary>
    /// The EOSIO built-in ABI types.
    /// </summary>
    public enum BuiltInAbiType
    {

        // Boolean
        Boolean,

        // Integers
        Int8,
        UInt8,
        Int16,
        UInt16,
        Int32,
        UInt32,
        Int64,
        UInt64,
        Int128,
        UInt128,
        VarInt32,
        VarUInt32,

        // Decimals
        Float32,
        Float64,
        Float128,

        // Time
        TimePoint,
        TimePointSec,
        BlockTimestampType,

        // String
        Name,
        String,

        // Binary
        Bytes,

        // Checksums
        Checksum160,
        Checksum256,
        Checksum512,


        // Keys
        PublicKey,
        Signature,

        // Symbols
        Symbol,
        SymbolCode,

        // Assets
        Asset,
        ExtendedAsset

    }

}