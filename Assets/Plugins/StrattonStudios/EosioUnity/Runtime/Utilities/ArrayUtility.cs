using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace StrattonStudios.EosioUnity.Utilities
{

    /// <summary>
    /// Array utilities.
    /// </summary>
    public static class ArrayUtility
    {

        public static T[] CopyOfRange<T>(T[] src, int start, int end)
        {
            int len = end - start;
            T[] dest = new T[len];
            for (int i = 0; i < len; i++)
            {
                dest[i] = src[start + i];
            }
            return dest;
        }

        /// <summary>
        /// Combines multiple arrays into one
        /// </summary>
        /// <param name="arrays">The arrays</param>
        /// <returns>Returns the combined array</returns>
        public static byte[] Combine(IEnumerable<byte[]> arrays)
        {
            byte[] ret = new byte[arrays.Sum(x => x != null ? x.Length : 0)];
            int offset = 0;
            foreach (byte[] data in arrays)
            {
                if (data == null) continue;

                Buffer.BlockCopy(data, 0, ret, offset, data.Length);
                offset += data.Length;
            }

            return ret;
        }

    }

}