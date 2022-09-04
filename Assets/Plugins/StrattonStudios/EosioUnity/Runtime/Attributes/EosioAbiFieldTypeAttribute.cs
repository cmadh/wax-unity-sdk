using System;

namespace StrattonStudios.EosioUnity
{

    /// <summary>
    /// Specifies the type of the field in the ABI schema.
    /// </summary>
    public class EosioAbiFieldTypeAttribute : Attribute
    {
        public string AbiType { get; set; }

        public EosioAbiFieldTypeAttribute(string abiType)
        {
            AbiType = abiType;
        }
    }

}
