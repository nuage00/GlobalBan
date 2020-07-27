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
        Task<List<PlayerBan>> FindBansAsync(string searchTerm, BanSearchMode searchMode);

        List<PlayerBan> FindBans(string searchTerm, BanSearchMode searchMode);

        Task<List<PlayerBan>> FindBansInEffectAsync(string searchTerm, BanSearchMode searchMode);

        List<PlayerBan> FindBansInEffect(string searchTerm, BanSearchMode searchMode);

        Task<BanType> CheckBanAsync(ulong steamId, uint ip = 0, string hwid = null);

        BanType CheckBan(ulong steamId, uint ip = 0, string hwid = null);

        Task<PlayerBan> BanPlayerAsync(int serverId, ulong playerId, uint ip, string hwid, uint duration, ulong adminId,
            string reason);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serverId"></param>
        /// <param name="playerId"></param>
        /// <param name="ip"></param>
        /// <param name="hwid"></param>
        /// <param name="duration"></param>
        /// <param name="adminId"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
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