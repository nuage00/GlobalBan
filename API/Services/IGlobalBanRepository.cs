using System.Collections.Generic;
using System.Threading.Tasks;
using OpenMod.API.Ioc;
using Pustalorc.GlobalBan.API.Classes;
using Pustalorc.GlobalBan.API.Enums;

namespace Pustalorc.GlobalBan.API.Services
{
    [Service]
    public interface IGlobalBanRepository
    {
        /// <summary>
        /// Finds any bans that match the input search term under the specified search mode.
        /// </summary>
        /// <param name="searchTerm">The term to match the bans with.</param>
        /// <param name="searchMode">The mode to search for bans with.</param>
        /// <returns>A list of all the bans that matched the search term.</returns>
        Task<List<PlayerBan>> FindBansAsync(string searchTerm, BanSearchMode searchMode);

        /// <summary>
        /// Finds any bans that match the input search term under the specified search mode.
        /// </summary>
        /// <param name="searchTerm">The term to match the bans with.</param>
        /// <param name="searchMode">The mode to search for bans with.</param>
        /// <returns>A list of all the bans that matched the search term.</returns>
        List<PlayerBan> FindBans(string searchTerm, BanSearchMode searchMode);

        /// <summary>
        /// Finds any bans that are in effect (not expired) that matches the input search term under the specified search mode.
        /// </summary>
        /// <param name="searchTerm">The term to match the bans with.</param>
        /// <param name="searchMode">The mode to search for bans with.</param>
        /// <returns>A list of all the bans in effect that matched the search term.</returns>
        Task<List<PlayerBan>> FindBansInEffectAsync(string searchTerm, BanSearchMode searchMode);

        /// <summary>
        /// Finds any bans that are in effect (not expired) that matches the input search term under the specified search mode.
        /// </summary>
        /// <param name="searchTerm">The term to match the bans with.</param>
        /// <param name="searchMode">The mode to search for bans with.</param>
        /// <returns>A list of all the bans in effect that matched the search term.</returns>
        List<PlayerBan> FindBansInEffect(string searchTerm, BanSearchMode searchMode);

        /// <summary>
        /// Checks for a ban with the provided parameters.
        /// </summary>
        /// <param name="steamId">The steam64Id of the player.</param>
        /// <param name="ip">The IP of the player.</param>
        /// <param name="hwid">The HWID of the player, joined with an empty string.</param>
        /// <returns>A result from the enum BanType.
        /// BanType.Id means that the steam64Id has a ban.
        /// BanType.Ip means that the IP has a ban, but not the steam64Id.
        /// BanType.Hwid means that the HWID has a ban, but not the IP nor the steam64Id.
        /// BanType.None that means that there's no ban for the provided inputs.</returns>
        Task<BanType> CheckBanAsync(ulong steamId, uint ip = 0, string hwid = null);

        /// <summary>
        /// Checks for a ban with the provided parameters.
        /// </summary>
        /// <param name="steamId">The steam64Id of the player.</param>
        /// <param name="ip">The IP of the player.</param>
        /// <param name="hwid">The HWID of the player, joined with an empty string.</param>
        /// <returns>A result from the enum BanType.
        /// BanType.Id means that the steam64Id has a ban.
        /// BanType.Ip means that the IP has a ban, but not the steam64Id.
        /// BanType.Hwid means that the HWID has a ban, but not the IP nor the steam64Id.
        /// BanType.None that means that there's no ban for the provided inputs.</returns>
        BanType CheckBan(ulong steamId, uint ip = 0, string hwid = null);

        /// <summary>
        /// Bans a player from the server.
        /// </summary>
        /// <param name="serverId">The serverID related to this ban.</param>
        /// <param name="playerId">The steam64ID of the player to ban.</param>
        /// <param name="ip">The IP of the player to ban.</param>
        /// <param name="hwid">The HWID of the player to ban.</param>
        /// <param name="duration">The amount of time that the ban should last for.</param>
        /// <param name="adminId">The steam64ID of the player that did the ban.</param>
        /// <param name="reason">The reason for the ban.</param>
        /// <returns>Constructed PlayerBan that represents the ban in the DB.</returns>
        Task<PlayerBan> BanPlayerAsync(int serverId, ulong playerId, uint ip, string hwid, uint duration, ulong adminId,
            string reason);

        /// <summary>
        /// Bans a player from the server.
        /// </summary>
        /// <param name="serverId">The serverID related to this ban.</param>
        /// <param name="playerId">The steam64ID of the player to ban.</param>
        /// <param name="ip">The IP of the player to ban.</param>
        /// <param name="hwid">The HWID of the player to ban.</param>
        /// <param name="duration">The amount of time that the ban should last for.</param>
        /// <param name="adminId">The steam64ID of the player that did the ban.</param>
        /// <param name="reason">The reason for the ban.</param>
        /// <returns>Constructed PlayerBan that represents the ban in the DB.</returns>
        PlayerBan BanPlayer(int serverId, ulong playerId, uint ip, string hwid, uint duration, ulong adminId,
            string reason);

        /// <summary>
        /// Unbans all bans that match the searchTerm under the searchMode.
        /// </summary>
        /// <param name="searchTerm">The term to search for an unban, this can be an ID, an IP or a HWID.</param>
        /// <param name="searchMode">The mode to search with, either one specific type, or multiple.</param>
        /// <returns>A List of all the PlayerBans that were modified so that the searchTerm is no longer banned.</returns>
        Task<List<PlayerBan>> UnbanAutoFindAsync(string searchTerm, BanSearchMode searchMode);

        /// <summary>
        /// Unbans all bans that match the searchTerm under the searchMode.
        /// </summary>
        /// <param name="searchTerm">The term to search for an unban, this can be an ID, an IP or a HWID.</param>
        /// <param name="searchMode">The mode to search with, either one specific type, or multiple.</param>
        /// <returns>A List of all the PlayerBans that were modified so that the searchTerm is no longer banned.</returns>
        List<PlayerBan> UnbanAutoFind(string searchTerm, BanSearchMode searchMode);

        /// <summary>
        /// Saves all changes made in the context to the database.
        /// </summary>
        /// <returns>A task that represents the asynchronous save operation. The task result contains the number of state entries written to the database.</returns>
        Task<int> SaveChangesAsync();

        /// <summary>
        /// Saves all changes made in this context to the database.
        /// </summary>
        /// <returns>The number of state entries written to the database.</returns>
        int SaveChanges();
    }
}