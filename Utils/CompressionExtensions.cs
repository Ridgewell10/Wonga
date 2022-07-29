using System.IO.Compression;
using System.Text;

namespace Utils
{
    public static class CompressionExtensions
    {
        public static Task<string> DecompressAsync(this byte[] data)
        {
            return Task.Factory.StartNew(() => Decompress(data));
        }

        public static string Decompress(this ReadOnlyMemory<byte> data)
        {
            return Decompress(data.ToArray());
        }

        public static string Decompress(this byte[] compressed, Encoding? encoding = null)
        {
            encoding ??= Encoding.UTF8;

            using var memoryStreamIn = new MemoryStream(compressed);
            using var gZipStream = new GZipStream(memoryStreamIn, CompressionMode.Decompress);

            var sb = new StringBuilder();
            var buffer = new byte[1024];
            int fillLength;

            while ((fillLength = gZipStream.Read(buffer, 0, buffer.Length)) > 0)
            {
                sb.Append(encoding.GetString(buffer, 0, fillLength));
            }

            return sb.ToString();
        }

        public static byte[] Compress(this string data, Encoding? encoding = null,
            CompressionLevel compressionLevel = CompressionLevel.SmallestSize)
        {
            encoding ??= Encoding.UTF8;
            var dataBytes = encoding.GetBytes(data);

            return Compress(dataBytes, compressionLevel);
        }

        public static byte[] Compress(this ReadOnlyMemory<byte> data,
            CompressionLevel compressionLevel = CompressionLevel.SmallestSize)
        {
            return Compress(data.ToArray(), compressionLevel);
        }

        public static byte[] Compress(this byte[] data, CompressionLevel compressionLevel = CompressionLevel.SmallestSize)
        {
            using var memoryStreamIn = new MemoryStream(data);
            using var memoryStreamOut = new MemoryStream();
            using var gZipStream = new GZipStream(memoryStreamOut, compressionLevel);

            memoryStreamIn.CopyTo(gZipStream);
            gZipStream.Flush();

            return memoryStreamOut.ToArray();
        }
    }
}
