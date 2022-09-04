using System;
using System.IO;
using System.IO.Compression;

namespace StrattonStudios.EosioUnity.Signing
{

    /// <summary>
    /// Default compression provider using DeflateStream.
    /// </summary>
    public class DefaultZlibProvider : IZlibProvider
    {

        public static readonly IZlibProvider Instance = new DefaultZlibProvider();

        public byte[] DecompressByteArray(byte[] data)
        {
            using (var input = new MemoryStream(data))
            {
                using (var output = new MemoryStream())
                {
                    using (var deflateStream = new DeflateStream(input, CompressionMode.Decompress))
                    {
                        deflateStream.CopyTo(output);
                    }
                    return output.ToArray();
                }
            }
        }

        public byte[] CompressByteArray(byte[] data)
        {
            using (var output = new MemoryStream())
            {
                using (var deflateStream = new DeflateStream(output, CompressionMode.Compress))
                {
                    deflateStream.Write(data, 0, data.Length);
                }
                return output.ToArray();
            }
        }

    }

}