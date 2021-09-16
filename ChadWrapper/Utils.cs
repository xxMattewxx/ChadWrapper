using ChadWrapper.Data.Types;
using System;
using System.Net;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Linq;
using System.IO;

namespace ChadWrapper
{
    class Utils
    {
        private static RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
        public static string GenerateGarbage(int length)
        {
            byte[] buffer = new byte[length];
            rng.GetNonZeroBytes(buffer);

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < buffer.Length; i++)
            {
                sb.Append(buffer[i].ToString("X2"));
            }
            return sb.ToString();
        }

        public static string SHA256File(string filePath)
        {
            using SHA256 SHA256 = SHA256Managed.Create();
            using FileStream fileStream = File.OpenRead(filePath);

            byte[] hash = SHA256.ComputeHash(fileStream);
            return BitConverter.ToString(hash).Replace("-", String.Empty).ToLower();
        }

        public static long ConvertToUnixTimestamp(DateTime date)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan diff = date.ToUniversalTime() - origin;
            return (long)Math.Floor(diff.TotalSeconds);
        }
    }
}
