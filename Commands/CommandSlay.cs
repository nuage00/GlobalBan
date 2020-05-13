using System;
using System.Collections.Generic;
using System.Globalization;
using Rocket.API;
using Rocket.Unturned.Chat;
using SDG.Unturned;
using Steamworks;

namespace fr34kyn01535.GlobalBan.Commands
{
    public class CommandSlay : IRocketCommand
    {
        public string Help => "Banns a player for a year";

        public string Name => "slay";

        public string Syntax => "<player>";

        public List<string> Aliases => new List<string>();

        public AllowedCaller AllowedCaller => AllowedCaller.Both;

        public List<string> Permissions => new List<string> {"globalban.slay"};

        public void Execute(IRocketPlayer caller, params string[] command)
        {
            if (command.Length == 0 || command.Length > 2)
            {
                UnturnedChat.Say(caller, GlobalBan.Instance.Translate("command_generic_invalid_parameter"));
                return;
            }

            if (!PlayerTool.tryGetSteamPlayer(command[0], out var otherSteamPlayer))
            {
                UnturnedChat.Say(caller, GlobalBan.Instance.Translate("command_generic_player_not_found"));
                return;
            }

            var steamId = otherSteamPlayer.playerID.steamID;
            SteamGameServerNetworking.GetP2PSessionState(steamId, out var state);
            var ip = state.m_nRemoteIP;
            var hwid = string.Join("", otherSteamPlayer.playerID.hwid);
            var characterName = otherSteamPlayer.playerID.characterName;

            if (command.Length >= 2)
            {
                GlobalBan.Instance.database.BanPlayer(characterName, steamId.ToString(), ip, hwid, caller.DisplayName,
                    command[1], 31536000);
                UnturnedChat.Say(GlobalBan.Instance.Translate("command_ban_public_reason", characterName, command[1]));
                Provider.ban(steamId, command[1], 31536000);

                Discord.SendWebhookPost(GlobalBan.Instance.Configuration.Instance.DiscordBanWebhook,
                    Discord.BuildDiscordEmbed("A player was banned from the server.",
                        $"{characterName} was banned from the server for {command[1]}!", "Global Ban",
                        "https://imperialproduction.blob.core.windows.net/shopcoreproducts/productlogos/194/13260ab6-c9b2-d350-64f3-39f360c60fe6/thumbnail.png",
                        GlobalBan.Instance.Configuration.Instance.DiscordBanWebhookColor,
                        new[]
                        {
                            Discord.BuildDiscordField("Steam64ID", steamId.ToString(), true),
                            Discord.BuildDiscordField("Banned By", caller.DisplayName, true),
                            Discord.BuildDiscordField("Time of Ban",
                                DateTime.Now.ToString(CultureInfo.InvariantCulture), false),
                            Discord.BuildDiscordField("Reason of Ban", command[1], true),
                            Discord.BuildDiscordField("Ban duration", "31536000", true)
                        }));
            }
            else
            {
                GlobalBan.Instance.database.BanPlayer(characterName, steamId.ToString(), ip, hwid, caller.DisplayName,
                    "", 31536000);
                UnturnedChat.Say(GlobalBan.Instance.Translate("command_ban_public", characterName));
                Provider.ban(steamId, GlobalBan.Instance.Translate("command_ban_private_default_reason"),
                    31536000);

                Discord.SendWebhookPost(GlobalBan.Instance.Configuration.Instance.DiscordBanWebhook,
                    Discord.BuildDiscordEmbed("A player was banned from the server.",
                        $"{characterName} was banned from the server for {GlobalBan.Instance.Translate("command_ban_private_default_reason")}!",
                        "Global Ban",
                        "https://imperialproduction.blob.core.windows.net/shopcoreproducts/productlogos/194/13260ab6-c9b2-d350-64f3-39f360c60fe6/thumbnail.png",
                        GlobalBan.Instance.Configuration.Instance.DiscordBanWebhookColor,
                        new[]
                        {
                            Discord.BuildDiscordField("Steam64ID", steamId.ToString(), true),
                            Discord.BuildDiscordField("Banned By", caller.DisplayName, true),
                            Discord.BuildDiscordField("Time of Ban",
                                DateTime.Now.ToString(CultureInfo.InvariantCulture), false),
                            Discord.BuildDiscordField("Reason of Ban",
                                GlobalBan.Instance.Translate("command_ban_private_default_reason"), true),
                            Discord.BuildDiscordField("Ban duration", "31536000", true)
                        }));
            }
        }
    }
}