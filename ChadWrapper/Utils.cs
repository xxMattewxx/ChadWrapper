using System;
using System.Security.Cryptography;
using System.Text;

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
    }
}
