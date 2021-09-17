using System;
using System.IO;
using System.Data.Common;
using MySqlConnector;
using Npgsql;

namespace ChadWrapperMake
{
    class Global
    {
        public static bool Debug;

        public static NpgsqlConnectionStringBuilder ChadDBConnectionBuilder;
        public static MySqlConnectionStringBuilder BoincDBConnectionBuilder;

        public static void Load()
        {
            if (Debug)
            {
                Console.WriteLine("Loading debug environment file...");
                DotEnv.Load("debug.env");
            }

            if(!Utils.IsStringSafe(Environment.GetEnvironmentVariable("CHADWRAPPER_DB_NAME")))
                throw new Exception("Choose a valid database name!");

            BoincDBConnectionBuilder = new MySqlConnectionStringBuilder()
            {
                Server = Environment.GetEnvironmentVariable("BOINC_DB_HOST"),
                UserID = Environment.GetEnvironmentVariable("BOINC_DB_USER"),
                Password = Environment.GetEnvironmentVariable("BOINC_DB_PASS"),
                Database = Environment.GetEnvironmentVariable("BOINC_DB_NAME"),
                AllowUserVariables = true
            };

            ChadDBConnectionBuilder = new NpgsqlConnectionStringBuilder()
            {
                Host = Environment.GetEnvironmentVariable("CHADWRAPPER_DB_HOST"),
                Username = Environment.GetEnvironmentVariable("CHADWRAPPER_DB_USER"),
                Password = Environment.GetEnvironmentVariable("CHADWRAPPER_DB_PASS"),
                Database = Environment.GetEnvironmentVariable("CHADWRAPPER_DB_NAME")
            };
        }
    }
}
