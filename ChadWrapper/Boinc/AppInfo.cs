using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChadWrapper.Boinc
{
    class AppInfo
    {
        public int ID;
        public string Codename;
        public string FriendlyName;

        public static AppInfo GetFromID(int ID)
        {
            try
            {
                using MySqlConnection connection = new MySqlConnection(Global.BoincDBConnectionBuilder.ConnectionString);
                connection.Open();

                using var command = connection.CreateCommand();
                command.CommandText = "SELECT name, user_friendly_name FROM app WHERE id = @id LIMIT 1;";
                command.Parameters.AddWithValue("@id", ID);

                using var reader = command.ExecuteReader();
                reader.Read();

                if (!reader.HasRows)
                    return null;

                AppInfo ret = new AppInfo()
                {
                    ID = ID,
                    Codename = reader.GetString(0),
                    FriendlyName = reader.GetString(1)
                };
                return ret;
            }
            catch
            {
                return null;
            }
        }

        public static AppInfo GetFromCodename(string codename)
        {
            try
            {
                using MySqlConnection connection = new MySqlConnection(Global.BoincDBConnectionBuilder.ConnectionString);
                connection.Open();

                using var command = connection.CreateCommand();
                command.CommandText = "SELECT id, user_friendly_name FROM app WHERE name = @codename LIMIT 1;";
                command.Parameters.AddWithValue("@codename", codename);

                using var reader = command.ExecuteReader();
                reader.Read();

                if (!reader.HasRows)
                    return null;

                AppInfo ret = new AppInfo()
                {
                    ID = reader.GetInt32(0),
                    Codename = codename,
                    FriendlyName = reader.GetString(1)
                };
                return ret;
            }
            catch
            {
                return null;
            }
        }
    }
}
