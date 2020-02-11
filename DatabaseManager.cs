using System;
using System.Data;
using System.Text.RegularExpressions;
using I18N.West;
using MySql.Data.MySqlClient;
using Rocket.Core.Logging;

namespace fr34kyn01535.GlobalBan
{
    public class DatabaseManager
    {
        public DatabaseManager()
        {
            new CP1250();
            CheckSchema();
        }

        private MySqlConnection CreateConnection()
        {
            MySqlConnection connection = null;
            try
            {
                if (GlobalBan.Instance.Configuration.Instance.DatabasePort == 0)
                    GlobalBan.Instance.Configuration.Instance.DatabasePort = 3306;
                connection = new MySqlConnection(
                    $"SERVER={GlobalBan.Instance.Configuration.Instance.DatabaseAddress};DATABASE={GlobalBan.Instance.Configuration.Instance.DatabaseName};UID={GlobalBan.Instance.Configuration.Instance.DatabaseUsername};PASSWORD={GlobalBan.Instance.Configuration.Instance.DatabasePassword};PORT={GlobalBan.Instance.Configuration.Instance.DatabasePort};");
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }

            return connection;
        }

        public bool IsBanned(string steamId)
        {
            try
            {
                var connection = CreateConnection();
                var command = connection.CreateCommand();
                command.CommandText = "select 1 from `" + GlobalBan.Instance.Configuration.Instance.DatabaseTableName +
                                      "` where `steamId` = '" + steamId +
                                      "' and (banDuration is null or ((banDuration + UNIX_TIMESTAMP(banTime)) > UNIX_TIMESTAMP()));";
                connection.Open();
                var result = command.ExecuteScalar();
                if (result != null) return true;

                connection.Close();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }

            return false;
        }

        public class Ban
        {
            public int Duration;
            public DateTime Time;
            public string Admin;
            public string Reason;
        }

        public Ban GetBan(string steamId)
        {
            try
            {
                var connection = CreateConnection();
                var command = connection.CreateCommand();
                command.CommandText = "select  `banDuration`,`banTime`,`admin`, `banMessage` from `" +
                                      GlobalBan.Instance.Configuration.Instance.DatabaseTableName +
                                      "` where `steamId` = '" + steamId +
                                      "' and (banDuration is null or ((banDuration + UNIX_TIMESTAMP(banTime)) > UNIX_TIMESTAMP()));";
                connection.Open();
                var result = command.ExecuteReader(CommandBehavior.SingleRow);
                if (result?.Read() == true && result.HasRows)
                    return new Ban
                    {
                        Duration = result["banDuration"] == DBNull.Value ? -1 : result.GetInt32("banDuration"),
                        Time = (DateTime) result["banTime"],
                        Admin = result["admin"] == DBNull.Value ||
                                result["admin"].ToString() == "Rocket.API.ConsolePlayer"
                            ? "Console"
                            : (string) result["admin"],
                        Reason =
                            result["banMessage"] == DBNull.Value ? "Rule Breaking" : result.GetString("banMessage")
                    };

                connection.Close();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }

            return null;
        }

        public void CheckSchema()
        {
            try
            {
                var connection = CreateConnection();
                var command = connection.CreateCommand();
                command.CommandText = "show tables like '" +
                                      GlobalBan.Instance.Configuration.Instance.DatabaseTableName + "'";
                connection.Open();
                var test = command.ExecuteScalar();

                if (test == null)
                {
                    command.CommandText = "CREATE TABLE `" +
                                          GlobalBan.Instance.Configuration.Instance.DatabaseTableName +
                                          "` (`id` int(11) NOT NULL AUTO_INCREMENT,`steamId` varchar(32) NOT NULL,`admin` varchar(32) NOT NULL,`banMessage` varchar(512) DEFAULT NULL,`charactername` varchar(255) DEFAULT NULL,`banDuration` int NULL,`banTime` timestamp NULL ON UPDATE CURRENT_TIMESTAMP,PRIMARY KEY (`id`));";
                    command.ExecuteNonQuery();
                }

                connection.Close();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }

        public void BanPlayer(string characterName, string steamId, string admin, string banMessage, int duration)
        {
            try
            {
                characterName = Regex.Replace(characterName, @"\p{Cs}", "");
                var connection = CreateConnection();
                var command = connection.CreateCommand();
                if (banMessage == null) banMessage = "";
                command.Parameters.AddWithValue("@csteamid", steamId);
                command.Parameters.AddWithValue("@admin", admin);
                command.Parameters.AddWithValue("@charactername", characterName);
                command.Parameters.AddWithValue("@banMessage", banMessage);
                if (duration == 0)
                    command.Parameters.AddWithValue("@banDuration", DBNull.Value);
                else
                    command.Parameters.AddWithValue("@banDuration", duration);
                command.CommandText = "insert into `" + GlobalBan.Instance.Configuration.Instance.DatabaseTableName +
                                      "` (`steamId`,`admin`,`banMessage`,`charactername`,`banTime`,`banDuration`) values(@csteamid,@admin,@banMessage,@charactername,now(),@banDuration);";
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }

        public class UnbanResult
        {
            public ulong Id;
            public string Name;
        }

        public UnbanResult UnbanPlayer(string player)
        {
            UnbanResult result = null;

            try
            {
                var connection = CreateConnection();

                var command = connection.CreateCommand();
                command.Parameters.AddWithValue("@player", "%" + player + "%");
                command.CommandText = "select steamId,charactername from `" +
                                      GlobalBan.Instance.Configuration.Instance.DatabaseTableName +
                                      "` where `steamId` like @player or `charactername` like @player limit 1;";
                connection.Open();
                var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    var steamId = reader.GetUInt64(0);
                    var characterName = reader.GetString(1);
                    connection.Close();
                    command = connection.CreateCommand();
                    command.Parameters.AddWithValue("@steamId", steamId);
                    command.CommandText = "delete from `" +
                                          GlobalBan.Instance.Configuration.Instance.DatabaseTableName +
                                          "` where `steamId` = @steamId;";
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                    result = new UnbanResult {Id = steamId, Name = characterName};
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }

            return result;
        }
    }
}