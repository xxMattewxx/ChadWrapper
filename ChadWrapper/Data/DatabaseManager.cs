using System;
using Npgsql;
using System.Data.Common;
using System.IO;
using System.Reflection;

namespace ChadWrapper.Data
{
    class DatabaseManager
    {
        public static bool IsSetup()
        {
            try
            {
                return ConfigEntry.Get("IS_SETUP") != null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }
    }
}
