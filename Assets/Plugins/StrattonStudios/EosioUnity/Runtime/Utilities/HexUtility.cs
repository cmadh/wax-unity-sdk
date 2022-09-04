using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace StrattonStudios.EosioUnity.Utilities
{

    /// <summary>
    /// Hexadecimal utilities.
    /// </summary>
    public static class HexUtility
    {

        /// <summary>
        /// Converts the bytes to hexadecimal string.
        /// </summary>
        /// <param name="bytes">The bytes to convert</param>
        /// <returns>Returns the hexadecimal encoded string</returns>
        public static string ToHexString(byte[] bytes)
        {
            StringBuilder hex = new StringBuilder(bytes.Length * 2);
            foreach (byte b in bytes)
            {
                hex.AppendFormat("{0:x2}", b);
            }
            return hex.ToString();
        }

        /// <summary>
        /// Converts the hexadecimal string to bytes.
        /// </summary>
        /// <param name="hex">The hexadecimal string to convert</param>
        /// <returns>Returns the decoded bytes</returns>
        public static byte[] FromHexString(string hex)
        {
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }

        /// <summary>
        /// Reverses the hexadecimal string.
        /// </summary>
        /// <param name="h">The hexadecimal string</param>
        /// <returns>Returns the reversed string</returns>
        public static string ReverseHex(string h)
        {
            return h.Substring(6, 2) + h.Substring(4, 2) + h.Substring(2, 2) + h.Substring(0, 2);
        }

    }

}