using System;
using Npgsql;
using System.Data.Common;
using System.IO;
using System.Reflection;

namespace ChadWrapperMake.Data
{
    class DatabaseManager
    {
        public static bool IsDBRunning()
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

                return true;
            }
            catch (PostgresException e)
            {
                if (e.SqlState == PostgresErrorCodes.InvalidPassword)
                    throw new Exception("Invalid auth provided for Postgres.");

                return false;
            }
            catch {
                return false;
            }
        }

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

                cmd.CommandText = Utils.GetFromResources("ChadWrapperMake.Resources.tables.sql");
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
