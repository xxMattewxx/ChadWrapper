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

            return GenerateHexString(buffer);
        }

        public static string SHA256File(string filePath)
        {
            using SHA256 SHA256 = SHA256Managed.Create();
            using FileStream fileStream = File.OpenRead(filePath);

            byte[] hash = SHA256.ComputeHash(fileStream);
            return BitConverter.ToString(hash).Replace("-", String.Empty).ToLower();
        }

        public static byte[] CalculateMD5(string filePath)
        {
            using MD5 md5 = MD5.Create();
            using FileStream fileStream = File.OpenRead(filePath);

            return md5.ComputeHash(fileStream);
        }

        public static string GenerateHexString(byte[] bytes)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                sb.Append(bytes[i].ToString("x2"));
            }
            return sb.ToString();
        }

        public static long ConvertToUnixTimestamp(DateTime date)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan diff = date.ToUniversalTime() - origin;
            return (long)Math.Floor(diff.TotalSeconds);
        }

        static string SafeAlphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        public static bool IsSafeString(string str)
        {
            foreach (char aux in str)
                if (!SafeAlphabet.Contains(aux))
                    return false;

            return true;
        }
    }
}
