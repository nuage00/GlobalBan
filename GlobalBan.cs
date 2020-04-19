using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Rocket.API.Collections;
using Rocket.Core.Plugins;
using Rocket.Unturned;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using UnityEngine.Serialization;

namespace fr34kyn01535.GlobalBan
{
    public class GlobalBan : RocketPlugin<GlobalBanConfiguration>
    {
        public static GlobalBan Instance;
        [FormerlySerializedAs("Database")] public DatabaseManager database;

        protected override void Load()
        {
            Instance = this;
            database = new DatabaseManager();
            Provider.onCheckValid += Events_OnJoinRequested;
            U.Events.OnPlayerConnected += RocketServerEvents_OnPlayerConnected;
        }

        protected override void Unload()
        {
            Provider.onCheckValid -= Events_OnJoinRequested;
            U.Events.OnPlayerConnected -= RocketServerEvents_OnPlayerConnected;
        }

        public override TranslationList DefaultTranslations =>
            new TranslationList
            {
                {
                    "default_banmessage",
                    "you were banned by {0} on {1} for {2} seconds, contact the staff if you feel this is a mistake."
                },
                {"command_generic_invalid_parameter", "Invalid parameter"},
                {"command_generic_player_not_found", "Player not found"},
                {"command_ban_public_reason", "The player {0} was banned for: {1}"},
                {"command_ban_public", "The player {0} was banned"},
                {"command_ban_private_default_reason", "you were banned from the server"},
                {"command_kick_public_reason", "The player {0} was kicked for: {1}"},
                {"command_kick_public", "The player {0} was kicked"},
                {"command_kick_private_default_reason", "you were kicked from the server"}
            };

        private void RocketServerEvents_OnPlayerConnected(UnturnedPlayer player)
        {
            var playerId = player.CSteamID;
            SteamGameServerNetworking.GetP2PSessionState(playerId, out var state);
            var playerIp = state.m_nRemoteIP;
            var playerHwid = string.Join("", player.SteamPlayer().playerID.hwid);
            var idBan = database.GetBan(playerId.ToString());
            var ipBan = database.GetBan(playerIp);
            var hwidBan = database.GetHwidBan(playerHwid);

            if (idBan == null && ipBan == null && hwidBan == null) return;

            if (ipBan != null)
            {
                const string banReason = "Ban Evading (IP)";

                database.BanPlayer(player.CharacterName, playerId.ToString(), playerIp, playerHwid, "Global Ban",
                    banReason, 0);
                UnturnedChat.Say(Translate("command_ban_public", player.CharacterName));

                var bytes = new List<byte> {9, (byte) banReason.Length};
                bytes.AddRange(Encoding.UTF8.GetBytes(banReason));
                bytes.AddRange(BitConverter.GetBytes(uint.MaxValue));
                Provider.send(playerId, ESteamPacket.BANNED, bytes.ToArray(), bytes.Count, 0);

                Discord.SendWebhookPost(Configuration.Instance.DiscordBanWebhook,
                    Discord.BuildDiscordEmbed("A player was banned from the server.",
                        $"{player.CharacterName} was banned from the server for {banReason}!", "Global Ban",
                        "https://imperialproduction.blob.core.windows.net/shopcoreproducts/productlogos/194/13260ab6-c9b2-d350-64f3-39f360c60fe6/thumbnail.png",
                        Configuration.Instance.DiscordBanWebhookColor,
                        new[]
                        {
                            Discord.BuildDiscordField("Steam64ID", playerId.ToString(), true),
                            Discord.BuildDiscordField("Time of Ban",
                                DateTime.Now.ToString(CultureInfo.InvariantCulture), true),
                            Discord.BuildDiscordField("Reason of Ban", banReason, false),
                            Discord.BuildDiscordField("Ban duration", uint.MaxValue.ToString(), true)
                        }));

                SteamGameServer.EndAuthSession(playerId);
            }
            else if (hwidBan != null)
            {
                const string banReason = "Ban Evading (HWID)";

                database.BanPlayer(player.CharacterName, playerId.ToString(), playerIp, playerHwid, "Global Ban",
                    banReason, 0);
                UnturnedChat.Say(Translate("command_ban_public", player.CharacterName));

                var bytes = new List<byte> {9, (byte) banReason.Length};
                bytes.AddRange(Encoding.UTF8.GetBytes(banReason));
                bytes.AddRange(BitConverter.GetBytes(uint.MaxValue));
                Provider.send(playerId, ESteamPacket.BANNED, bytes.ToArray(), bytes.Count, 0);

                Discord.SendWebhookPost(Configuration.Instance.DiscordBanWebhook,
                    Discord.BuildDiscordEmbed("A player was banned from the server.",
                        $"{player.CharacterName} was banned from the server for {banReason}!", "Global Ban",
                        "https://imperialproduction.blob.core.windows.net/shopcoreproducts/productlogos/194/13260ab6-c9b2-d350-64f3-39f360c60fe6/thumbnail.png",
                        Configuration.Instance.DiscordBanWebhookColor,
                        new[]
                        {
                            Discord.BuildDiscordField("Steam64ID", playerId.ToString(), true),
                            Discord.BuildDiscordField("Time of Ban",
                                DateTime.Now.ToString(CultureInfo.InvariantCulture), true),
                            Discord.BuildDiscordField("Reason of Ban", banReason, false),
                            Discord.BuildDiscordField("Ban duration", uint.MaxValue.ToString(), true)
                        }));

                SteamGameServer.EndAuthSession(playerId);
            }
            else if (idBan != null)
            {
                var time = idBan.Duration == -1
                    ? uint.MaxValue
                    : (uint) idBan.Time.AddSeconds(idBan.Duration).Subtract(DateTime.Now).TotalSeconds;
                var bytes = new List<byte> {9, (byte) idBan.Reason.Length};
                bytes.AddRange(Encoding.UTF8.GetBytes(idBan.Reason));
                bytes.AddRange(BitConverter.GetBytes(time));
                Provider.send(playerId, ESteamPacket.BANNED, bytes.ToArray(), bytes.Count, 0);
                SteamGameServer.EndAuthSession(playerId);
            }
        }

        public static bool TryConvertTimeToSeconds(string time, out int duration)
        {
            duration = 0;
            if (string.IsNullOrEmpty(time)) return false;

            var splitVar = time.Split('w');
            if (splitVar.Length >= 2)
                if (int.TryParse(splitVar[0], out var tm))
                    duration += tm * 604800;
            splitVar = splitVar[splitVar.Length >= 2 ? 1 : 0].Split('d');
            if (splitVar.Length >= 2)
                if (int.TryParse(splitVar[0], out var tm))
                    duration += tm * 86400;
            splitVar = splitVar[splitVar.Length >= 2 ? 1 : 0].Split('h');
            if (splitVar.Length >= 2)
                if (int.TryParse(splitVar[0], out var tm))
                    duration += tm * 3600;
            splitVar = splitVar[splitVar.Length >= 2 ? 1 : 0].Split('m');
            if (splitVar.Length >= 2)
                if (int.TryParse(splitVar[0], out var tm))
                    duration += tm * 60;
            splitVar = splitVar[splitVar.Length >= 2 ? 1 : 0].Split('s');
            if (splitVar.Length >= 2)
                if (int.TryParse(splitVar[0], out var tm))
                    duration += tm;
            if (int.TryParse(splitVar[splitVar.Length >= 2 ? 1 : 0], out var tm1))
                duration += tm1;
            return duration > 0;
        }

        private void Events_OnJoinRequested(ValidateAuthTicketResponse_t callback, ref bool isValid)
        {
            var ban = database.GetBan(callback.m_SteamID.ToString());

            if (ban == null) return;

            isValid = false;
            var time = ban.Duration == -1
                ? uint.MaxValue
                : (uint) ban.Time.AddSeconds(ban.Duration).Subtract(DateTime.Now).TotalSeconds;
            var bytes = new List<byte> {9, (byte) ban.Reason.Length};
            bytes.AddRange(Encoding.UTF8.GetBytes(ban.Reason));
            bytes.AddRange(BitConverter.GetBytes(time));
            Provider.send(callback.m_SteamID, ESteamPacket.BANNED, bytes.ToArray(), bytes.Count, 0);
        }
    }
}