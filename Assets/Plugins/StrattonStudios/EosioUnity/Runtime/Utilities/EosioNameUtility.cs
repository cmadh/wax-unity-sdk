using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace StrattonStudios.EosioUnity.Utilities
{

    /// <summary>
    /// EISUI `name` utilities.
    /// </summary>
    public class EosioNameUtility
    {

        /// <summary>
        /// Converts a EOSIO's `name` to bytes.
        /// </summary>
        /// <param name="name">The name to convert</param>
        /// <returns>Returns the name in binary form</returns>
        public static byte[] ConvertNameToBytes(string name)
        {
            var bytes = new byte[8];
            int bit = 63;
            for (int i = 0; i < name.Length; ++i)
            {
                var c = StringUtility.CharToSymbol(name[i]);
                if (bit < 5)
                    c = (byte)(c << 1);
                for (int j = 4; j >= 0; --j)
                {
                    if (bit >= 0)
                    {
                        bytes[(int)Math.Floor((decimal)(bit / 8))] |= (byte)(((c >> j) & 1) << (bit % 8));
                        --bit;
                    }
                }
            }
            return bytes;
        }

    }

}