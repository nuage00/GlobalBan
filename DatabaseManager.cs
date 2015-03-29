using MySql.Data.MySqlClient;
using Rocket.Logging;
using Rocket.RocketAPI;
using SDG;
using System;

namespace unturned.ROCKS.GlobalBan
{
    public class DatabaseManager
    {
        public DatabaseManager()
        {
            new I18N.West.CP1250();
            CheckSchema();
        }

        private MySqlConnection createConnection()
        {
            MySqlConnection connection = null;
            try
            {
                if (GlobalBan.Instance.Configuration.DatabasePort == 0) GlobalBan.Instance.Configuration.DatabasePort = 3306;
                connection = new MySqlConnection(String.Format("SERVER={0};DATABASE={1};UID={2};PASSWORD={3};PORT={4};", GlobalBan.Instance.Configuration.DatabaseAddress, GlobalBan.Instance.Configuration.DatabaseName, GlobalBan.Instance.Configuration.DatabaseUsername, GlobalBan.Instance.Configuration.DatabasePassword, GlobalBan.Instance.Configuration.DatabasePort));
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
            return connection;
        }

        public string IsBanned(string steamId)
        {
            string output = null;
            try
            {
                MySqlConnection connection = createConnection();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "select `banMessage` from `" + GlobalBan.Instance.Configuration.DatabaseTableName + "` where `steamId` = '" + steamId + "';";
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

        public void CheckSchema()
        {
            try
            {
                MySqlConnection connection = createConnection();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "show tables like '" + GlobalBan.Instance.Configuration.DatabaseTableName + "'";
                connection.Open();
                object test = command.ExecuteScalar();

                if (test == null)
                {
                    command.CommandText = "CREATE TABLE `" + GlobalBan.Instance.Configuration.DatabaseTableName + "` (`id` int(11) NOT NULL AUTO_INCREMENT,`steamId` varchar(32) NOT NULL,`admin` varchar(32) NOT NULL,`banMessage` varchar(512) DEFAULT NULL,`steamname` varchar(512) DEFAULT NULL,`charactername` varchar(255) DEFAULT NULL,`banTime` timestamp NULL ON UPDATE CURRENT_TIMESTAMP,PRIMARY KEY (`id`));";
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }

        public void BanPlayer(SteamPlayerID player,string admin, string banMessage)
        {
            try
            {
                MySqlConnection connection = createConnection();
                MySqlCommand command = connection.CreateCommand();
                if (banMessage == null) banMessage = "";
                command.Parameters.AddWithValue("@csteamid", player.CSteamID);
                command.Parameters.AddWithValue("@admin", admin);
                command.Parameters.AddWithValue("@charactername", player.CharacterName);
                command.Parameters.AddWithValue("@steamname", player.SteamName);
                command.Parameters.AddWithValue("@banMessage", banMessage);
                command.CommandText = "insert into `" + GlobalBan.Instance.Configuration.DatabaseTableName + "` (`steamId`,`admin`,`banMessage`,`charactername`,`steamname`,`banTime`) values(@csteamid,@admin,@charactername,@steamname,@banMessage,now());";
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }

        public void UnbanPlayer(string player)
        {
            try
            {
                MySqlConnection connection = createConnection();
                MySqlCommand command = connection.CreateCommand();
                command.Parameters.AddWithValue("@player", "%"+player+"%");
                command.CommandText = "delete from `" + GlobalBan.Instance.Configuration.DatabaseTableName + "` where `steamId` like @player or `charactername` like = '%@player%' or `steamname` like = '%@player%' limit 1;";
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
