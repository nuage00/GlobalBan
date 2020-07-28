// ReSharper disable AnnotateNotNullParameter
// ReSharper disable AnnotateNotNullTypeMember

using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using OpenMod.API.Plugins;
using OpenMod.API.Users;
using OpenMod.EntityFrameworkCore.Extensions;
using OpenMod.Unturned.Plugins;
using Pustalorc.GlobalBan.API.Enums;
using Pustalorc.GlobalBan.API.External;
using Pustalorc.GlobalBan.API.Services;
using Pustalorc.GlobalBan.Database;
using Pustalorc.PlayerInfoLib.Unturned.API.Services;
using SDG.Unturned;
using Math = System.Math;

[assembly:
    PluginMetadata("Pustalorc.GlobalBan", Author = "Pustalorc", DisplayName = "Global Ban",
        Website = "https://github.com/Pustalorc/GlobalBan/")]

namespace Pustalorc.GlobalBan
{
    public class GlobalBanPlugin : OpenModUnturnedPlugin
    {
        private readonly IConfiguration m_Configuration;
        private readonly IStringLocalizer m_StringLocalizer;
        private readonly ILogger<GlobalBanPlugin> m_Logger;
        private readonly IUserManager m_UserManager;
        private readonly GlobalBanDbContext m_GlobalBanDbContext;
        private readonly IGlobalBanRepository m_GlobalBanRepository;
        private readonly IPlayerInfoRepository m_PlayerInfoRepository;

        public GlobalBanPlugin(
            IConfiguration configuration,
            IStringLocalizer stringLocalizer,
            ILogger<GlobalBanPlugin> logger,
            IUserManager userManager,
            IPlayerInfoRepository playerInfoRepository,
            IGlobalBanRepository globalBanRepository,
            GlobalBanDbContext globalBanDbContext,
            IServiceProvider serviceProvider) : base(serviceProvider)
        {
            m_Configuration = configuration;
            m_StringLocalizer = stringLocalizer;
            m_Logger = logger;
            m_UserManager = userManager;
            m_GlobalBanDbContext = globalBanDbContext;
            m_GlobalBanRepository = globalBanRepository;
            m_PlayerInfoRepository = playerInfoRepository;
        }

        protected override async UniTask OnLoadAsync()
        {
            // Event nelson added to check if someone is banned yourself. Uses correct Banned removal if isBanned is returned to true.
            Provider.onCheckBanStatusWithHWID += CheckBanned;

            await m_GlobalBanDbContext.OpenModMigrateAsync();

            m_Logger.LogInformation("Global Ban for Unturned by Pustalorc was loaded correctly.");
        }

        protected override UniTask OnUnloadAsync()
        {
            m_Logger.LogInformation("Global Ban for Unturned by Pustalorc was unloaded correctly.");

            return UniTask.CompletedTask;
        }

        private void CheckBanned(SteamPlayerID playerId, uint remoteIp, ref bool isBanned, ref string banReason,
            ref uint banRemainingDuration)
        {
            var steamId = playerId.steamID.m_SteamID;
            var hwid = string.Join("", playerId.hwid);
            var server = m_PlayerInfoRepository.GetCurrentServer();

            switch (m_GlobalBanRepository.CheckBan(steamId, remoteIp, hwid))
            {
                case BanType.Id:
                    var now = DateTime.Now;

                    var bans = m_GlobalBanRepository.FindBansInEffect(steamId.ToString(), BanSearchMode.Id).OrderByDescending(k => k.TimeOfBan.AddSeconds(k.Duration).Subtract(now).TotalSeconds);
                    var ban = bans.FirstOrDefault();
                    if (ban == null) return;

                    isBanned = true;
                    banReason = ban.Reason;
                    var remainingDuration = ban.TimeOfBan.AddSeconds(ban.Duration).Subtract(DateTime.Now).TotalSeconds;
                    banRemainingDuration =
                        (uint)Math.Min(uint.MaxValue,
                            remainingDuration); // Makes sure that the maximum number sent back is uint.MaxValue, even if remaining duration is higher (it would overflow otherwise and give the wrong duration)

                    if (!bans.Any(k => k.Hwid.Equals(hwid) && k.Ip == remoteIp))
                    {
                        m_GlobalBanRepository.BanPlayer(server?.Id ?? 0, steamId, remoteIp, hwid, banRemainingDuration,
                            0, m_StringLocalizer["internal:new_ip_or_hwid_ban_reason"]);

                        var translation = m_StringLocalizer["commands:ban:banned",
                            new {Player = playerId.characterName, Reason = banReason}];
                        m_UserManager.BroadcastAsync(translation);
                        m_Logger.LogInformation(translation);
                        SendWebhook(WebhookType.BanEvading, playerId.characterName,
                            m_StringLocalizer["internal:ban_evading_admin_name"], banReason, steamId.ToString(),
                            banRemainingDuration);
                    }

                    break;
                case BanType.Ip:
                case BanType.Hwid:
                    isBanned = true;
                    banReason = m_StringLocalizer["internal:ban_evading_reason"];
                    banRemainingDuration = uint.MaxValue;

                    m_GlobalBanRepository.BanPlayer(server?.Id ?? 0, steamId, remoteIp, hwid, uint.MaxValue, 0,
                        banReason);

                    var translated = m_StringLocalizer["commands:ban:banned",
                        new {Player = playerId.characterName, Reason = banReason}];
                    m_UserManager.BroadcastAsync(translated);
                    m_Logger.LogInformation(translated);
                    SendWebhook(WebhookType.BanEvading, playerId.characterName,
                        m_StringLocalizer["internal:ban_evading_admin_name"], banReason, steamId.ToString(),
                        uint.MaxValue);
                    break;
                default:
                    return;
            }
        }

        public async Task SendWebhookAsync(WebhookType webhookType, string playerName, string adminName, string reason,
            string playerId, uint duration)
        {
            await Discord.SendWebhookPostAsync(m_Configuration[$"webhooks:{webhookType.ToString().ToLower()}:url"],
                Discord.BuildDiscordEmbed(m_StringLocalizer[$"webhooks:{webhookType.ToString().ToLower()}:title"],
                    m_StringLocalizer[$"webhooks:{webhookType.ToString().ToLower()}:description",
                        new {Player = playerName, Reason = reason}], m_StringLocalizer["webhooks:global:displayname"],
                    m_Configuration["webhooks:image_url"],
                    m_Configuration.GetSection($"webhooks:{webhookType.ToString().ToLower()}:color").Get<int>(),
                    BuildFields(webhookType, adminName, reason, playerId, duration)));
        }

        public void SendWebhook(WebhookType webhookType, string playerName, string adminName, string reason,
            string playerId, uint duration)
        {
            Discord.SendWebhookPost(m_Configuration[$"webhooks:{webhookType.ToString().ToLower()}:url"],
                Discord.BuildDiscordEmbed(m_StringLocalizer[$"webhooks:{webhookType.ToString().ToLower()}:title"],
                    m_StringLocalizer[$"webhooks:{webhookType.ToString().ToLower()}:description",
                        new {Player = playerName, Reason = reason}], m_StringLocalizer["webhooks:global:displayname"],
                    m_Configuration["webhooks:image_url"],
                    m_Configuration.GetSection($"webhooks:{webhookType.ToString().ToLower()}:color").Get<int>(),
                    BuildFields(webhookType, adminName, reason, playerId, duration)));
        }

        private object[] BuildFields(WebhookType webhookType, string adminName, string reason,
            string playerId, uint duration)
        {
            var fields = Array.Empty<object>();

            switch (webhookType)
            {
                case WebhookType.BanEvading:
                    fields = new[]
                    {
                        Discord.BuildDiscordField(m_StringLocalizer["webhooks:global:steam64id"], playerId, true),
                        Discord.BuildDiscordField(m_StringLocalizer["webhooks:global:time"],
                            DateTime.Now.ToString(CultureInfo.InvariantCulture), true),
                        Discord.BuildDiscordField(m_StringLocalizer["webhooks:global:reason"], reason, false),
                        Discord.BuildDiscordField(m_StringLocalizer["webhooks:global:duration"], duration.ToString(),
                            true)
                    };
                    break;
                case WebhookType.Ban:
                    fields = new[]
                    {
                        Discord.BuildDiscordField(m_StringLocalizer["webhooks:global:steam64id"], playerId, true),
                        Discord.BuildDiscordField(m_StringLocalizer["webhooks:ban:banned_by"], adminName, true),
                        Discord.BuildDiscordField(m_StringLocalizer["webhooks:global:time"],
                            DateTime.Now.ToString(CultureInfo.InvariantCulture), false),
                        Discord.BuildDiscordField(m_StringLocalizer["webhooks:global:reason"], reason, true),
                        Discord.BuildDiscordField(m_StringLocalizer["webhooks:global:duration"], duration.ToString(),
                            true)
                    };
                    break;
                case WebhookType.Kick:
                    fields = new[]
                    {
                        Discord.BuildDiscordField(m_StringLocalizer["webhooks:global:steam64id"], playerId, true),
                        Discord.BuildDiscordField(m_StringLocalizer["webhooks:kick:kicked_by"], adminName, true),
                        Discord.BuildDiscordField(m_StringLocalizer["webhooks:global:time"],
                            DateTime.Now.ToString(CultureInfo.InvariantCulture), false),
                        Discord.BuildDiscordField(m_StringLocalizer["webhooks:global:reason"], reason, true)
                    };
                    break;
                case WebhookType.Unban:
                    fields = new[]
                    {
                        Discord.BuildDiscordField(m_StringLocalizer["webhooks:global:steam64id"], playerId, true),
                        Discord.BuildDiscordField(m_StringLocalizer["webhooks:unban:unbanned_by"], adminName, true),
                        Discord.BuildDiscordField(m_StringLocalizer["webhooks:global:time"],
                            DateTime.Now.ToString(CultureInfo.InvariantCulture), false)
                    };
                    break;
            }

            return fields;
        }
    }
}