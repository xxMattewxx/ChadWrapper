using ChadWrapper.Data.Types;
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
                Console.WriteLine(e);
                if (e.ErrorCode == MySqlErrorCode.DuplicateKeyEntry)
                    return -2;
            }
            catch (Exception e) { Console.WriteLine(e); }

            return -1;
        }

        public static bool AddAppVersion(long appID, int versionNumber, int platformID, string xml, string plan_class)
        {
            using MySqlConnection connection = new MySqlConnection(Global.BoincDBConnectionBuilder.ConnectionString);
            connection.Open();

            using MySqlCommand command = connection.CreateCommand();
            command.CommandText = "INSERT INTO app_version (create_time, appid, version_num, platformid, xml_doc, plan_class) VALUES (@time, @appid, @version_num, @platformid, @xml, @plan_class);";
            command.Parameters.AddWithValue("@time", Utils.ConvertToUnixTimestamp(DateTime.Now));
            command.Parameters.AddWithValue("@appid", appID);
            command.Parameters.AddWithValue("@version_num", versionNumber);
            command.Parameters.AddWithValue("@platformid", platformID);
            command.Parameters.AddWithValue("@xml", xml);
            command.Parameters.AddWithValue("@plan_class", plan_class);

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

        public static string GetPlatformName(int platformID)
        {
            using MySqlConnection connection = new MySqlConnection(Global.BoincDBConnectionBuilder.ConnectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = "SELECT name FROM platform WHERE id = @id LIMIT 1;";
            command.Parameters.AddWithValue("@id", platformID);

            using var reader = command.ExecuteReader();
            reader.Read();

            if (!reader.HasRows)
                return null;

            return reader.GetString(0);
        }

        public static List<BinaryInfo> GetAppBinaries(int appID)
        {
            try
            {
                using MySqlConnection connection = new MySqlConnection(Global.BoincDBConnectionBuilder.ConnectionString);
                connection.Open();

                using var command = connection.CreateCommand();
                command.CommandText = "SELECT LENGTH(xml_doc), xml_doc, platformid, plan_class FROM app_version WHERE appid = @appID;";
                command.Parameters.AddWithValue("@appID", appID);

                using var reader = command.ExecuteReader();
                reader.Read();

                if (!reader.HasRows)
                    return null;

                List<BinaryInfo> ret = new List<BinaryInfo>();
                do
                {
                    int length = reader.GetInt32(0);
                    byte[] buffer = new byte[length];

                    if (reader.GetBytes(1, 0, buffer, 0, length) != length)
                        return null;

                    string xml = Encoding.UTF8.GetString(buffer);
                    BinaryInfo binaryInfo = BinaryInfo.FromBoincXML(xml);
                    binaryInfo.Platform = GetPlatformName(reader.GetInt32(2));
                    binaryInfo.HardwareAccelerator = reader.GetString(3);

                    if (binaryInfo.HardwareAccelerator == "")
                        binaryInfo.HardwareAccelerator = "None";

                    ret.Add(binaryInfo);
                }
                while (reader.Read());

                return ret;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }
    }
}
