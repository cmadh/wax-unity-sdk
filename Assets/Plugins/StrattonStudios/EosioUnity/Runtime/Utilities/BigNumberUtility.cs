using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace StrattonStudios.EosioUnity.Utilities
{

    /// <summary>
    /// Utilities for interacting with big numbers in string format.
    /// </summary>
    public static class BigNumberUtility
    {

        /// <summary>
        /// Checks whether the big number is negative in binary format.
        /// </summary>
        /// <param name="bin">The big number to check in binary format</param>
        /// <returns>Returns true if is negative, otherwise false</returns>
        public static bool IsNegative(byte[] bin)
        {
            return (bin[bin.Length - 1] & 0x80) != 0;
        }

        /// <summary>
        /// Negates a big number in binary format.
        /// </summary>
        /// <param name="bin">The big number to negate in binary format</param>
        public static void Negate(byte[] bin)
        {
            int carry = 1;
            for (int i = 0; i < bin.Length; ++i)
            {
                int x = (~bin[i] & 0xff) + carry;
                bin[i] = (byte)x;
                carry = x >> 8;
            }
        }

        /// <summary>
        /// Convert an unsigned decimal number encoded as string to a big number binary format.
        /// </summary>
        /// <param name="size">The size of the big number in bytes</param>
        /// <param name="s">The decimal encoded as string</param>
        /// <returns>Returns the big number in binary format</returns>
        public static byte[] ConvertDecimalToBinary(uint size, string s)
        {
            byte[] result = new byte[size];
            for (int i = 0; i < s.Length; ++i)
            {
                char srcDigit = s[i];
                if (srcDigit < '0' || srcDigit > '9')
                    throw new Exception("invalid number");
                int carry = srcDigit - '0';
                for (int j = 0; j < size; ++j)
                {
                    int x = result[j] * 10 + carry;
                    result[j] = (byte)x;
                    carry = x >> 8;
                }

                if (carry != 0)
                    throw new Exception("number is out of range");
            }

            return result;
        }

        /// <summary>
        /// Convert a signed decimal number encoded as string to a big number binary format.
        /// </summary>
        /// <param name="size">The size of the big number in bytes</param>
        /// <param name="s">The decimal encoded as string</param>
        /// <returns>Returns the big number in binary format</returns>
        public static byte[] ConvertSignedDecimalToBinary(uint size, string s)
        {
            bool negative = s[0] == '-';
            if (negative)
                s = s.Substring(0, 1);
            byte[] result = ConvertDecimalToBinary(size, s);
            if (negative)
                Negate(result);
            return result;
        }

        /// <summary>
        /// Convert the big number from binary format to an unsigned decimal number encoded as string.
        /// </summary>
        /// <param name="bin">The big number in binary format</param>
        /// <param name="minDigits">The 0-pad result to this many digits</param>
        /// <returns>Returns the decimal number encoded as string</returns>
        public static string ConvertBinaryToDecimal(byte[] bin, int minDigits = 1)
        {
            var result = new List<char>(minDigits);

            for (int i = 0; i < minDigits; i++)
            {
                result.Add('0');
            }

            for (int i = bin.Length - 1; i >= 0; --i)
            {
                int carry = bin[i];
                for (int j = 0; j < result.Count; ++j)
                {
                    int x = ((result[j] - '0') << 8) + carry;
                    result[j] = (char)('0' + (x % 10));
                    carry = (x / 10) | 0;
                }

                while (carry != 0)
                {
                    result.Add((char)('0' + carry % 10));
                    carry = (carry / 10) | 0;
                }
            }

            result.Reverse();
            return string.Join("", result);
        }

        /// <summary>
        /// Convert the big number from binary format to a signed decimal number encoded as string.
        /// </summary>
        /// <param name="bin">The big number in binary format</param>
        /// <param name="minDigits">The 0-pad result to this many digits</param>
        /// <returns>Returns the decimal number encoded as string</returns>
        public static string ConvertSignedBinaryToDecimal(byte[] bin, int minDigits = 1)
        {
            if (IsNegative(bin))
            {
                Negate(bin);
                return '-' + ConvertBinaryToDecimal(bin, minDigits);
            }

            return ConvertBinaryToDecimal(bin, minDigits);
        }

    }

}