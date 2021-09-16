using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace ChadWrapperMake
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

        static string alphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        public static bool IsStringSafe(string input)
        {
            if (input == null || input.Length > 30) return false;

            foreach(char aux in input.ToCharArray())
            {
                if (!alphabet.Contains(aux))
                    return false;
            }
            return true;
        }

        public static string GetFromResources(string resourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();

            using Stream stream = assembly.GetManifestResourceStream(resourceName);
            using StreamReader reader = new StreamReader(stream);

            return reader.ReadToEnd();
        }
    }
}
