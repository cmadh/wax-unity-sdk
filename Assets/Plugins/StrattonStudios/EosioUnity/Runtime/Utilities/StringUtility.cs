using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

using UnityEngine;

namespace StrattonStudios.EosioUnity.Utilities
{

    /// <summary>
    /// String utility methods..
    /// </summary>
    public static class StringUtility
    {

        /// <summary>
        /// Converts the snake case string to pascal case.
        /// </summary>
        /// <param name="str">The string to convert</param>
        /// <returns>Returns the string in pascal case</returns>
        public static string SnakeCaseToPascalCase(string str)
        {
            var result = str.ToLower().Replace("_", " ");
            TextInfo info = CultureInfo.CurrentCulture.TextInfo;
            result = info.ToTitleCase(result).Replace(" ", string.Empty);
            return result;
        }

        /// <summary>
        /// Converts a pascal case string to snake case.
        /// </summary>
        /// <param name="str">The string to convert</param>
        /// <returns>Returns the string in snake case</returns>
        public static string PascalCaseToSnakeCase(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return str;
            }

            var builder = new StringBuilder();
            bool first = true;
            foreach (var c in str)
            {
                if (char.IsUpper(c))
                {
                    if (!first)
                        builder.Append('_');
                    builder.Append(char.ToLower(c));
                }
                else
                {
                    builder.Append(c);
                }

                if (first)
                    first = false;
            }

            return builder.ToString();
        }

        /// <summary>
        /// Convert ASCII char to symbol value.
        /// </summary>
        /// <param name="c">The char</param>
        /// <returns>Returns the symbol value</returns>
        public static byte CharToSymbol(char c)
        {
            if (c >= 'a' && c <= 'z')
                return (byte)(c - 'a' + 6);
            if (c >= '1' && c <= '5')
                return (byte)(c - '1' + 1);
            return 0;
        }

    }

}