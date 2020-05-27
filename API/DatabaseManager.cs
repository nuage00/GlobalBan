using fr34kyn01535.GlobalBan.Config;
using I18N.West;
using Pustalorc.Libraries.MySqlConnectorWrapper;
using Pustalorc.Libraries.MySqlConnectorWrapper.Queries;
using Pustalorc.Libraries.MySqlConnectorWrapper.TableStructure;
using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using MySql.Data.MySqlClient;
using PlayerInfoLibrary;

namespace fr34kyn01535.GlobalBan.API
{
    public class DatabaseManager : ConnectorWrapper<GlobalBanConfiguration>
    {
        private Dictionary<Query, Query> _createTableQueries;
        private Query _getPlayerDataQuery;

        [NotNull]
        private Dictionary<Query, Query> CreateTableQueries => _createTableQueries ??= new Dictionary<Query, Query>
        {
            {
                new Query($"SHOW TABLES LIKE '{Configuration.DatabaseTableName}';", EQueryType.Scalar),
                new Query(
                    $"CREATE TABLE `{Configuration.DatabaseTableName}` (`id` BIGINT UNSIGNED NOT NULL AUTO_INCREMENT, `steamId` BIGINT UNSIGNED NOT NULL, `ip` INT UNSIGNED NOT NULL, `hwid` VARCHAR(255) NOT NULL DEFAULT '', `adminId` BIGINT UNSIGNED NOT NULL DEFAULT 0, `banMessage` VARCHAR(512) NOT NULL DEFAULT 'N/A', `banDuration` INT UNSIGNED, `serverId` SMALLINT UNSIGNED, `banTime` TIMESTAMP NULL ON UPDATE CURRENT_TIMESTAMP, `unbanned` BOOLEAN NOT NULL DEFAULT FALSE, PRIMARY KEY (`id`));",
                    EQueryType.NonQuery)
            }
        };

        [NotNull]
        public Query GetPlayerDataQuery => _getPlayerDataQuery ??= new Query(
            $"SELECT t1.id, t1.steamId, t1.hwid, t1.ip, t1.adminId, t1.banMessage, t1.banDuration, t1.serverId, t1.banTime, t1.unbanned FROM `{Configuration.DatabaseTableName}` as t1;",
            EQueryType.Reader, FetchedBans, true);

        private List<PlayerBan> _allBans = new List<PlayerBan>();

        public DatabaseManager(GlobalBanConfiguration config) : base(config)
        {
            // ReSharper disable once ObjectCreationAsStatement
            new CP1250();

            var output = ExecuteTransaction(CreateTableQueries.Keys.ToArray()).ToList();
            var execute = (from queryResult in output
                where queryResult.Output == null
                select CreateTableQueries[queryResult.Query]).ToArray();

            if (execute.Length > 0)
                ExecuteTransaction(execute);

            ExecuteQuery(GetPlayerDataQuery);
        }

        private void FetchedBans([NotNull] QueryOutput queryOutput)
        {
            if (queryOutput.Query.QueryType != EQueryType.Reader || !(queryOutput.Output is List<Row> rows)) return;

            lock (_allBans)
            {
                _allBans = (from row in rows select BuildBanData(row)).ToList();
            }
        }

        public bool IsBanned(uint ip)
        {
            return ip != uint.MaxValue && _allBans.Any(k => k.Ip != uint.MaxValue && k.TimeOfBan.AddSeconds(k.Duration) > DateTime.Now && k.Ip == ip);
        }

        public bool IsBanned([CanBeNull] string hwid)
        {
            return !string.IsNullOrEmpty(hwid) && _allBans.Any(k => !string.IsNullOrEmpty(k.Hwid) && k.TimeOfBan.AddSeconds(k.Duration) > DateTime.Now && k.Hwid == hwid);
        }

        [CanBeNull]
        public PlayerBan GetValidBan(ulong id)
        {
            return GetValidBans(id).FirstOrDefault();
        }

        [NotNull]
        public IEnumerable<PlayerBan> GetValidBans(ulong id)
        {
            return GetBans(id).Where(k => !k.Unbanned && k.TimeOfBan.AddSeconds(k.Duration) > DateTime.Now).ToList();
        }

        [NotNull]
        public List<PlayerBan> GetBans(ulong id)
        {
            return _allBans.Where(k => k.SteamId == id).ToList();
        }

        public void BanPlayer(ulong steamId, uint ip, string hwid, ulong admin, [CanBeNull] string banMessage, uint duration)
        {
            RequestQueryExecute(false,
                new Query(
                    $"INSERT INTO `{Configuration.DatabaseTableName}` (`steamId`,`ip,`hwid`,`adminId`,`banMessage`,`banDuration`,`serverId`,`banTime`) VALUES(@playerId,@ip,@hwid,@admin,@banMessage,@banDuration,@serverId,now())",
                    EQueryType.NonQuery, null, false, new MySqlParameter("@playerId", steamId),
                    new MySqlParameter("@ip", ip), new MySqlParameter("@hwid", hwid),
                    new MySqlParameter("@admin", admin),
                    new MySqlParameter("@banMessage", string.IsNullOrEmpty(banMessage) ? "N/A" : banMessage),
                    duration == 0
                        ? new MySqlParameter("@banDuration", DBNull.Value)
                        : new MySqlParameter("@banDuration", duration),
                    new MySqlParameter("@serverId", PlayerInfoLib.Instance.database.InstanceId)));
        }

        public bool TryUnban(ulong id)
        {
            var bans = GetBans(id).Where(k => k.TimeOfBan.AddSeconds(k.Duration) > DateTime.Now).ToList();
            if (bans.Count == 0)
                return false;

            RequestQueryExecute(true, bans.Select(ban => new Query($"UPDATE `{Configuration.DatabaseTableName}` SET `unbanned`=TRUE WHERE `id`={ban.BanEntryId}", EQueryType.NonQuery)).ToArray());
            return true;
        }

        [NotNull]
        private static PlayerBan BuildBanData([NotNull] Row row)
        {
            return new PlayerBan(ulong.Parse(row["id"].ToString()), ulong.Parse(row["steamId"].ToString()),
                row["hwid"].ToString(), uint.Parse(row["ip"].ToString()), uint.TryParse(row["banDuration"].ToString(), out var duration) ? duration : uint.MaxValue,
                (DateTime) row["banTime"], ulong.Parse(row["adminId"].ToString()), row["banMessage"].ToString(),
                ushort.Parse(row["serverId"].ToString()), bool.Parse(row["unbanned"].ToString()));
        }
    }
}