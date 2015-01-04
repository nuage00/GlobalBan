using MySql.Data.MySqlClient;
using Rocket;
using System;

namespace GlobalBan
{
    class Database
    {
        private static MySqlConnection createConnection()
        {
            MySqlConnection connection = null;
            try
            {
                connection = new MySqlConnection(String.Format("SERVER={0};DATABASE={1};UID={2};PASSWORD={3};", GlobalBan.configuration.DatabaseAddress, GlobalBan.configuration.DatabaseName, GlobalBan.configuration.DatabaseUsername, GlobalBan.configuration.DatabasePassword));
                return connection;
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
            return connection;
        }

        public static string IsBanned(string steamId)
        {
            string output = null;
            try
            {
                MySqlConnection connection = createConnection();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "select `banMessage` from `" + GlobalBan.configuration.DatabaseTableName + "` where `steamId` = '" + steamId + "';";
                connection.Open();
                object result = command.ExecuteScalar();
                if (result != null) output = result.ToString();
                connection.Close();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
            return output;
        }

        public static void CheckSchema()
        {
            try
            {
                MySqlConnection connection = createConnection();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "show tables like '" + GlobalBan.configuration.DatabaseTableName + "'";
                connection.Open();
                object test = command.ExecuteScalar();

                if (test == null)
                {
                    command.CommandText = "CREATE TABLE `" + GlobalBan.configuration.DatabaseTableName + "` (`id` int(11) NOT NULL AUTO_INCREMENT,`steamId` varchar(32) NOT NULL,`admin` varchar(32) NOT NULL,`banMessage` varchar(255) DEFAULT NULL,`banTime` timestamp NOT NULL DEFAULT `0000-00-00 00:00:00` ON UPDATE CURRENT_TIMESTAMP,PRIMARY KEY (`id`));";
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }

        public static void BanPlayer(string steamId,string admin, string banMessage)
        {
            try
            {
                MySqlConnection connection = createConnection();
                MySqlCommand command = connection.CreateCommand();
                if (banMessage == null) banMessage = "";
                command.CommandText = "insert into `" + GlobalBan.configuration.DatabaseTableName + "` (`steamId`,`admin`,`banMessage`,`banTime`) values('" + steamId + "','" + admin + "','" + banMessage + "',now());";
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }

        public static void UnbanPlayer(string steamId)
        {
            try
            {
                MySqlConnection connection = createConnection();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "delete from `" + GlobalBan.configuration.DatabaseTableName + "` where `steamId` = '" + steamId + "';";
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }
    }
}
