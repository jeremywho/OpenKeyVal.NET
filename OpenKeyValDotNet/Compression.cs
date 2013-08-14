using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;

namespace OpenKeyValDotNet
{
    public static class Compression
    {
        /// <summary>
        /// Compresses a string using System.IO.Compression.GZipStream
        /// </summary>
        /// <param name="s">String to compress</param>
        /// <returns>Compressed string</returns>
        public static string Compress(string s)
        {
            var bytes = Encoding.Unicode.GetBytes(s);
            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(mso, CompressionMode.Compress))
                {
                    msi.CopyTo(gs);
                }

                return Convert.ToBase64String(mso.ToArray());
            }
        }

        /// <summary>
        /// Asynchronously compresses a string using System.IO.Compression.GZipStream
        /// </summary>
        /// <param name="s">String to compress</param>
        /// <returns>Compressed string</returns>
        public static async Task<string> CompressAsync(string s)
        {
            var bytes = Encoding.Unicode.GetBytes(s);
            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(mso, CompressionMode.Compress))
                {
                    await msi.CopyToAsync(gs);
                }

                return Convert.ToBase64String(mso.ToArray());
            }
        }

        /// <summary>
        /// Decompresses a string using System.IO.Compression.GZipStream
        /// </summary>
        /// <param name="s">String to decompress</param>
        /// <returns>Decompressed string</returns>
        public static string Decompress(string s)
        {
            var bytes = Convert.FromBase64String(s);
            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(msi, CompressionMode.Decompress))
                {
                    gs.CopyTo(mso);
                }
                return Encoding.Unicode.GetString(mso.ToArray());
            }
        }

        /// <summary>
        /// Asychronously Decompresses a string using System.IO.Compression.GZipStream
        /// </summary>
        /// <param name="s">String to decompress</param>
        /// <returns>Decompressed string</returns>
        public static async Task<string> DecompressAsync(string s)
        {
            var bytes = Convert.FromBase64String(s);
            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(msi, CompressionMode.Decompress))
                {
                    await gs.CopyToAsync(mso);
                }
                return Encoding.Unicode.GetString(mso.ToArray());
            }
        }
    }
}