using System;
using Npgsql;
using System.Data.Common;
using System.IO;

namespace ChadWrapperMake.Data
{
    class DatabaseManager
    {
        public static bool CreateDB()
        {
            try
            {
                using NpgsqlConnection connection = new NpgsqlConnection(new NpgsqlConnectionStringBuilder()
                {
                    Host = Global.ChadDBConnectionBuilder.Host,
                    Username = Global.ChadDBConnectionBuilder.Username,
                    Password = Global.ChadDBConnectionBuilder.Password
                }.ConnectionString);
                connection.Open();

                using var cmd = connection.CreateCommand();
                cmd.CommandText = "CREATE DATABASE " + Global.ChadDBConnectionBuilder.Database;
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }

            return true;
        }

        public static bool CreateTables()
        {
            try
            {
                using NpgsqlConnection connection = new NpgsqlConnection(Global.ChadDBConnectionBuilder.ConnectionString);
                connection.Open();

                using var cmd = connection.CreateCommand();
                cmd.CommandText = File.ReadAllText("tables.sql");

                cmd.ExecuteNonQuery();
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
            return true;
        }

        public static bool IsSetup()
        {
            try
            {
                return ConfigEntry.Get("IS_SETUP") != null;
            }
            catch
            {
                return false;
            }
        }
    }
}
