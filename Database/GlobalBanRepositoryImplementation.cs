// ReSharper disable AnnotateNotNullParameter
// ReSharper disable AnnotateNotNullTypeMember
// ReSharper disable AnnotateCanBeNullParameter

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Ioc;
using OpenMod.API.Prioritization;
using Pustalorc.GlobalBan.API.Classes;
using Pustalorc.GlobalBan.API.Enums;
using Pustalorc.GlobalBan.API.Services;

namespace Pustalorc.GlobalBan.Database
{
    [ServiceImplementation(Lifetime = ServiceLifetime.Scoped, Priority = Priority.Normal)]
    public class GlobalBanRepositoryImplementation : IGlobalBanRepository
    {
        private readonly GlobalBanDbContext m_DbContext;

        public GlobalBanRepositoryImplementation(GlobalBanDbContext dbContext)
        {
            m_DbContext = dbContext;
        }

        public async Task<List<PlayerBan>> FindBansAsync(string searchTerm, BanSearchMode searchMode)
        {
            return await FindBansInternal(searchTerm, searchMode).ToListAsync();
        }

        public List<PlayerBan> FindBans(string searchTerm, BanSearchMode searchMode)
        {
            return FindBansInternal(searchTerm, searchMode).ToList();
        }

        private IQueryable<PlayerBan> FindBansInternal(string searchTerm, BanSearchMode searchMode)
        {
            switch (searchMode)
            {
                case BanSearchMode.Id:
                    return GetBansById(searchTerm);
                case BanSearchMode.Ip:
                    return GetBansByIp(searchTerm);
                case BanSearchMode.Hwid:
                    return GetBansByHwid(searchTerm);
                case BanSearchMode.IdOrIp:
                    return GetBansById(searchTerm).Concat(GetBansByIp(searchTerm));
                case BanSearchMode.IdOrHwid:
                    return GetBansById(searchTerm).Concat(GetBansByHwid(searchTerm));
                case BanSearchMode.IpOrHwid:
                    return GetBansByIp(searchTerm).Concat(GetBansByHwid(searchTerm));
                case BanSearchMode.All:
                    return GetBansById(searchTerm).Concat(GetBansByIp(searchTerm)).Concat(GetBansByHwid(searchTerm));
                default:
                    return m_DbContext.PlayerBans.Take(0);
            }
        }

        private IQueryable<PlayerBan> GetBansById(string searchTerm)
        {
            if (!ulong.TryParse(searchTerm, out var id) || id < 76561197960265728 || id > 103582791429521408)
                return m_DbContext.PlayerBans.Take(0);

            return m_DbContext.PlayerBans.Where(k => k.PlayerId == id);
        }

        private IQueryable<PlayerBan> GetBansByIp(string searchTerm)
        {
            if (!uint.TryParse(searchTerm, out var ip) || ip == 0 || ip == uint.MaxValue)
                return m_DbContext.PlayerBans.Take(0);

            return m_DbContext.PlayerBans.Where(k => k.Ip != 0 && k.Ip == ip);
        }

        private IQueryable<PlayerBan> GetBansByHwid(string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm) || searchTerm.Equals("00000000000000000000") || searchTerm.Length < 20)
                return m_DbContext.PlayerBans.Take(0);

            return m_DbContext.PlayerBans.Where(k => !string.IsNullOrEmpty(k.Hwid) && k.Hwid.Equals(searchTerm));
        }


        public async Task<List<PlayerBan>> FindBansInEffectAsync(string searchTerm, BanSearchMode searchMode)
        {
            var bans = await FindBansInEffectInternal(searchTerm, searchMode).ToListAsync();
            return bans.Where(k => DateTime.Now.Subtract(k.TimeOfBan).TotalSeconds <= k.Duration).ToList();
        }

        public List<PlayerBan> FindBansInEffect(string searchTerm, BanSearchMode searchMode)
        {
            return FindBansInEffectInternal(searchTerm, searchMode).ToList()
                .Where(k => DateTime.Now.Subtract(k.TimeOfBan).TotalSeconds <= k.Duration).ToList();
        }

        private IQueryable<PlayerBan> FindBansInEffectInternal(string searchTerm, BanSearchMode searchMode)
        {
            switch (searchMode)
            {
                case BanSearchMode.Id:
                    return GetBansInEffectById(searchTerm);
                case BanSearchMode.Ip:
                    return GetBansInEffectByIp(searchTerm);
                case BanSearchMode.Hwid:
                    return GetBansInEffectByHwid(searchTerm);
                case BanSearchMode.IdOrIp:
                    return GetBansInEffectById(searchTerm).Concat(GetBansInEffectByIp(searchTerm));
                case BanSearchMode.IdOrHwid:
                    return GetBansInEffectById(searchTerm).Concat(GetBansInEffectByHwid(searchTerm));
                case BanSearchMode.IpOrHwid:
                    return GetBansInEffectByIp(searchTerm).Concat(GetBansInEffectByHwid(searchTerm));
                case BanSearchMode.All:
                    return GetBansInEffectById(searchTerm).Concat(GetBansInEffectByIp(searchTerm))
                        .Concat(GetBansInEffectByHwid(searchTerm));
                default:
                    return m_DbContext.PlayerBans.Take(0);
            }
        }

        private IQueryable<PlayerBan> GetBansInEffectById(string searchTerm)
        {
            return GetBansById(searchTerm).Where(k => !k.IsUnbanned);
        }

        private IQueryable<PlayerBan> GetBansInEffectByIp(string searchTerm)
        {
            return GetBansByIp(searchTerm).Where(k => !k.IsUnbanned);
        }

        private IQueryable<PlayerBan> GetBansInEffectByHwid(string searchTerm)
        {
            return GetBansByHwid(searchTerm).Where(k => !k.IsUnbanned);
        }

        public async Task<BanType> CheckBanAsync(ulong steamId, uint ip = 0, string hwid = null)
        {
            var validIdBans = await GetBansInEffectById(steamId.ToString()).ToListAsync();
            var validIpBans = await GetBansInEffectByIp(ip.ToString()).ToListAsync();
            var validHwidBans = await GetBansInEffectByHwid(hwid).ToListAsync();

            return CheckBanInternal(validIdBans, validIpBans, validHwidBans);
        }

        public BanType CheckBan(ulong steamId, uint ip = 0, string hwid = null)
        {
            var validIdBans = GetBansInEffectById(steamId.ToString()).ToList();
            var validIpBans = GetBansInEffectByIp(ip.ToString()).ToList();
            var validHwidBans = GetBansInEffectByHwid(hwid).ToList();

            return CheckBanInternal(validIdBans, validIpBans, validHwidBans);
        }

        private static BanType CheckBanInternal(IReadOnlyCollection<PlayerBan> idBans,
            IReadOnlyCollection<PlayerBan> ipBans, IReadOnlyCollection<PlayerBan> hwidBans)
        {
            if (idBans.Count > 0)
                return BanType.Id;
            if (ipBans.Count > 0)
                return BanType.Ip;

            return hwidBans.Count > 0 ? BanType.Hwid : BanType.None;
        }

        public async Task<PlayerBan> BanPlayerAsync(int serverId, ulong playerId, uint ip, string hwid, uint duration,
            ulong adminId, string reason)
        {
            var ban = CreatePlayerBanInternal(serverId, playerId, ip, hwid, duration, adminId, reason);

            await m_DbContext.PlayerBans.AddAsync(ban);
            await m_DbContext.SaveChangesAsync();

            return ban;
        }

        public PlayerBan BanPlayer(int serverId, ulong playerId, uint ip, string hwid, uint duration, ulong adminId,
            string reason)
        {
            var ban = CreatePlayerBanInternal(serverId, playerId, ip, hwid, duration, adminId, reason);

            m_DbContext.PlayerBans.Add(ban);
            m_DbContext.SaveChanges();

            return ban;
        }

        private static PlayerBan CreatePlayerBanInternal(int serverId, ulong playerId, uint ip, string hwid,
            uint duration, ulong adminId, string reason)
        {
            return new PlayerBan
            {
                Duration = duration,
                AdminId = adminId,
                Hwid = hwid,
                Ip = ip,
                IsUnbanned = false,
                PlayerId = playerId,
                Reason = reason,
                ServerId = serverId,
                TimeOfBan = DateTime.Now
            };
        }


        public async Task<List<PlayerBan>> UnbanAutoFindAsync(string target, BanSearchMode searchMode)
        {
            var bans = await FindBansInEffectAsync(target, searchMode);

            UnbanInternal(bans);

            await SaveChangesAsync();

            return bans;
        }

        public List<PlayerBan> UnbanAutoFind(string target, BanSearchMode searchMode)
        {
            var bans = FindBansInEffect(target, searchMode);

            UnbanInternal(bans);

            SaveChanges();

            return bans;
        }

        private static void UnbanInternal(IReadOnlyCollection<PlayerBan> bans)
        {
            if (bans.Count == 0) return;

            foreach (var ban in bans)
                ban.IsUnbanned = true;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await m_DbContext.SaveChangesAsync();
        }

        public int SaveChanges()
        {
            return m_DbContext.SaveChanges();
        }
    }
}