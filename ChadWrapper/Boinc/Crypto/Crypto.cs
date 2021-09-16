using ChadWrapper.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ChadWrapper.Boinc.Crypto
{
    class Crypto
    {
        public static string GetFileSignature(string MD5, string key)
        {
            string[] lines = key.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            int keySize;
            List<byte> bytes = GetKeyData(lines, out keySize);

            RSAParameters rsaParams = new RSAParameters()
            {
                Modulus = bytes.GetRange(0, 128).ToArray(), //modulus
                Exponent = bytes.GetRange(128, 128).ToArray(), //e
                D = bytes.GetRange(256, 128).ToArray(), //d
                P = bytes.GetRange(384, 64).ToArray(), //p
                Q = bytes.GetRange(448, 64).ToArray(),//q
                DP = bytes.GetRange(512, 64).ToArray(), //dmp1
                DQ = bytes.GetRange(576, 64).ToArray(), //dmq1
                InverseQ = bytes.GetRange(640, 64).ToArray() //iqmp
            };

            RSA rsa = RSA.Create(keySize);
            rsa.ImportParameters(rsaParams);

            byte[] signatureBytes = rsa.Encrypt(Encoding.UTF8.GetBytes(MD5), RSAEncryptionPadding.Pkcs1);
            return GetBOINCFuckedUpStr(signatureBytes);
        }

        public static string GetBOINCFuckedUpStr(byte[] bytes)
        {
            StringBuilder ret = new StringBuilder();
            string hex = "0123456789abcdef";
            int i;

            for(i = 0; i < bytes.Length; i++)
            {
                ret.Append(hex[bytes[i] / 16]);
                ret.Append(hex[bytes[i] % 16]);
                if (i % 32 == 31) ret.Append('\n');
            }
            if (bytes.Length % 32 != 0) ret.Append('\n');

            ret.Append(".\n");
            return ret.ToString();
        }

        private static List<byte> GetKeyData(string[] lines, out int keySize)
        {
            List<byte> ret = new List<byte>();
            int lineCount = 0;
            keySize = 1024;

            foreach (string line in lines)
            {
                if (lineCount++ == 0)
                {
                    keySize = Convert.ToInt32(line);
                    continue;
                }

                if (line.StartsWith("."))
                    break;

                int charCount = 0;
                char[] buffer = new char[2];
                foreach (char aux in line)
                {
                    buffer[charCount++] = aux;

                    if (charCount == 2)
                    {
                        byte toAdd = byte.Parse(buffer, System.Globalization.NumberStyles.HexNumber);
                        ret.Add(toAdd);

                        charCount = 0;
                    }
                }
            }
            return ret;
        }
    }
}
