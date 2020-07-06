using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fr34kyn01535.GlobalBan.API;
using fr34kyn01535.GlobalBan.Config;
using JetBrains.Annotations;
using PlayerInfoLibrary;
using Rocket.API.Collections;
using Rocket.Core.Logging;
using Rocket.Core.Plugins;
using Rocket.Unturned;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;

namespace fr34kyn01535.GlobalBan
{
    public class GlobalBan : RocketPlugin<GlobalBanConfiguration>
    {
        public static GlobalBan Instance;
        public DatabaseManager database;

        protected override void Load()
        {
            Instance = this;
            database = new DatabaseManager(Configuration.Instance);
            U.Events.OnPlayerConnected += RocketServerEvents_OnPlayerConnected;
        }

        protected override void Unload()
        {
            U.Events.OnPlayerConnected -= RocketServerEvents_OnPlayerConnected;
        }

        [NotNull]
        public override TranslationList DefaultTranslations =>
            new TranslationList
            {
                {"command_generic_invalid_parameter", "Invalid parameter"},
                {"command_generic_player_not_found", "Player not found"},
                {"command_ban_public_reason", "The player {0} was banned for: {1}"},
                {"command_ban_public", "The player {0} was banned"},
                {"command_ban_private_default_reason", "you were banned from the server"},
                {"command_kick_public_reason", "The player {0} was kicked for: {1}"},
                {"command_kick_public", "The player {0} was kicked"},
                {"command_kick_private_default_reason", "you were kicked from the server"},
                {"ban_history", "The player {0} has had {1} bans. Latest bans: {2}"}
            };

        private void RocketServerEvents_OnPlayerConnected([NotNull] UnturnedPlayer player)
        {
            OnJoin(player);
        }

        private async Task OnJoin([NotNull] UnturnedPlayer player)
        {
            var playerId = player.CSteamID;
            var playerIp = playerId.GetIp();
            var playerHwid = string.Join("", player.SteamPlayer().playerID.hwid);

            var idBan = await database.GetValidBan(playerId.m_SteamID);
            var ipBan = await database.IsBanned(playerIp);
            var hwidBan = await database.IsBanned(playerHwid);

            if (idBan == null && !ipBan && !hwidBan) return;

            if (idBan != null)
                RemovePlayerWithBan(playerId,
                    idBan.Duration == uint.MaxValue
                        ? uint.MaxValue
                        : (uint) idBan.TimeOfBan.AddSeconds(idBan.Duration).Subtract(DateTime.Now).TotalSeconds,
                    idBan.Reason);
            else
                BanEvading(player.CharacterName, playerId, playerIp, playerHwid);
        }

        private void BanEvading(string playerName, CSteamID playerId, uint playerIp, string playerHwid)
        {
            const string banReason = "Ban Evading";

            database.BanPlayer(playerId.m_SteamID, playerIp, playerHwid, 0, banReason, 0);

            UnturnedChat.Say(Translate("command_ban_public", playerName));

            SendBanWebhook(playerName, banReason, playerId.ToString(), uint.MaxValue);
            RemovePlayerWithBan(playerId, uint.MaxValue, banReason);
        }

        public void SendBanWebhook(string playerName, string reason, string playerId, uint duration)
        {
            Discord.SendWebhookPost(Configuration.Instance.DiscordBanWebhook,
                Discord.BuildDiscordEmbed("A player was banned from the server.",
                    $"{playerName} was banned from the server for {reason}!", Configuration.Instance.WebhookDisplayName,
                    Configuration.Instance.WebhookImageUrl, Configuration.Instance.DiscordBanWebhookColor,
                    new[]
                    {
                        Discord.BuildDiscordField("Steam64ID", playerId, true),
                        Discord.BuildDiscordField("Time of Ban", DateTime.Now.ToString(CultureInfo.InvariantCulture),
                            true),
                        Discord.BuildDiscordField("Reason of Ban", reason, false),
                        Discord.BuildDiscordField("Ban duration", duration.ToString(), true)
                    }));
        }

        public static void RemovePlayerWithBan(CSteamID playerId, uint timeRemaining, [NotNull] string reason)
        {
            var bytes = new List<byte> {9, (byte) reason.Length};
            bytes.AddRange(Encoding.UTF8.GetBytes(reason));
            bytes.AddRange(BitConverter.GetBytes(timeRemaining));
            Provider.send(playerId, ESteamPacket.BANNED, bytes.ToArray(), bytes.Count, 0);
            SteamGameServer.EndAuthSession(playerId);
        }
    }
}