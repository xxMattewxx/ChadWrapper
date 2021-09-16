using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChadWrapper.Boinc
{
    class BoincDatabaseManager
    {
        public static long AddApp(string codename, string friendlyName)
        {
            try
            {
                using MySqlConnection connection = new MySqlConnection(Global.BoincDBConnectionBuilder.ConnectionString);
                connection.Open();

                using MySqlCommand command = connection.CreateCommand();
                command.CommandText = "INSERT INTO app (create_time, name, user_friendly_name) VALUES (@time, @name, @friendly_name);";
                command.Parameters.AddWithValue("@time", Utils.ConvertToUnixTimestamp(DateTime.Now));
                command.Parameters.AddWithValue("@name", codename);
                command.Parameters.AddWithValue("@friendly_name", friendlyName);

                if (command.ExecuteNonQuery() > 0)
                {
                    return command.LastInsertedId;
                }
            }
            catch (MySqlException e)
            {
                if (e.ErrorCode == MySqlErrorCode.DuplicateKeyEntry)
                    return -2;
            }
            catch { }

            return -1;
        }

        public static bool AddAppVersion(long appID, int versionNumber, int platformID, string xml)
        {
            using MySqlConnection connection = new MySqlConnection(Global.BoincDBConnectionBuilder.ConnectionString);
            connection.Open();

            using MySqlCommand command = connection.CreateCommand();
            command.CommandText = "INSERT INTO app_version (create_time, appid, version_num, platformid, xml_doc) VALUES (@time, @appid, @version_num, @platformid, @xml);";
            command.Parameters.AddWithValue("@time", Utils.ConvertToUnixTimestamp(DateTime.Now));
            command.Parameters.AddWithValue("@appid", appID);
            command.Parameters.AddWithValue("@version_num", versionNumber);
            command.Parameters.AddWithValue("@platformid", platformID);
            command.Parameters.AddWithValue("@xml", xml);

            return command.ExecuteNonQuery() > 0;
        }

        public static int GetPlatformID(string platformName)
        {
            using MySqlConnection connection = new MySqlConnection(Global.BoincDBConnectionBuilder.ConnectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = "SELECT id FROM platform WHERE name = @name LIMIT 1;";
            command.Parameters.AddWithValue("@name", platformName);

            using var reader = command.ExecuteReader();
            reader.Read();

            if (!reader.HasRows)
                return -1;

            return reader.GetInt32(0);
        }

        public static int GetAppID(string codename)
        {
            using MySqlConnection connection = new MySqlConnection(Global.BoincDBConnectionBuilder.ConnectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = "SELECT id FROM app WHERE name = @name LIMIT 1;";
            command.Parameters.AddWithValue("@name", codename);

            using var reader = command.ExecuteReader();
            reader.Read();

            if (!reader.HasRows)
                return -1;

            return reader.GetInt32(0);
        }
    }
}
