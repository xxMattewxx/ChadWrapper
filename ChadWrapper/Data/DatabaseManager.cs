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
            catch (PostgresException e)
            {
                if (e.SqlState == PostgresErrorCodes.InvalidPassword)
                    throw new Exception("Invalid auth provided for Postgres!");

                return false;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}
