using System;
using System.IO;

namespace ChadWrapper
{
    public class DotEnv
    {
        public static void Load(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Environment file not found.");

            foreach (var line in File.ReadAllLines(filePath))
            {
                int firstEqualSymbol = line.IndexOf("=");
                if (firstEqualSymbol == -1)
                    continue;

                string key = line.Substring(0, firstEqualSymbol);
                string value = line.Substring(firstEqualSymbol + 1);

                Environment.SetEnvironmentVariable(key.Trim(), value.Trim());
            }
        }
    }
}