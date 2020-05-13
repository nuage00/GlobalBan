using System;
using System.Collections.Generic;
using System.Globalization;
using Rocket.API;
using Rocket.Core.Logging;
using Rocket.Core.Steam;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Commands;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;

namespace fr34kyn01535.GlobalBan.Commands
{
    public class CommandBan : IRocketCommand
    {
        public string Help => "Bans a player";

        public string Name => "ban";

        public string Syntax => "<player> [reason] [duration]";

        public List<string> Aliases => new List<string>();

        public AllowedCaller AllowedCaller => AllowedCaller.Both;

        public List<string> Permissions => new List<string> {"globalban.ban"};

        public void Execute(IRocketPlayer caller, params string[] command)
        {
            try
            {
                if (command.Length == 0 || command.Length > 3)
                {
                    UnturnedChat.Say(caller, GlobalBan.Instance.Translate("command_generic_invalid_parameter"));
                    return;
                }

                var isOnline = false;

                CSteamID steamId;
                var ip = uint.MinValue;
                string hwid = null;
                string characterName = null;


                var otherPlayer = UnturnedPlayer.FromName(command[0]);
                var otherPlayerId = command.GetCSteamIDParameter(0);
                if (otherPlayer == null || otherPlayer.CSteamID.ToString() == "0" ||
                    caller != null && otherPlayer.CSteamID.ToString() == caller.Id)
                {
                    if (otherPlayerId != null)
                    {
                        steamId = new CSteamID(otherPlayerId.Value);
                        var playerProfile = new Profile(otherPlayerId.Value);
                        characterName = playerProfile.SteamID;
                    }
                    else
                    {
                        UnturnedChat.Say(caller, GlobalBan.Instance.Translate("command_generic_player_not_found"));
                        return;
                    }
                }
                else
                {
                    isOnline = true;
                    steamId = otherPlayer.CSteamID;
                    SteamGameServerNetworking.GetP2PSessionState(steamId, out var state);
                    ip = state.m_nRemoteIP;
                    hwid = string.Join("", otherPlayer.SteamPlayer().playerID.hwid);
                    characterName = otherPlayer.CharacterName;
                }

                var adminName = "Console";
                if (caller != null) adminName = caller.ToString();

                // ReSharper disable once ConvertIfStatementToSwitchStatement
                if (command.Length == 3)
                {
                    var duration = 0;
                    if (GlobalBan.TryConvertTimeToSeconds(command[2], out duration))
                    {
                        GlobalBan.Instance.database.BanPlayer(characterName, steamId.ToString(), ip, hwid, adminName,
                            command[1], duration);
                        UnturnedChat.Say(GlobalBan.Instance.Translate("command_ban_public_reason", characterName,
                            command[1]));
                        if (isOnline)
                            Provider.ban(steamId, command[1], (uint) duration);

                        Discord.SendWebhookPost(GlobalBan.Instance.Configuration.Instance.DiscordBanWebhook,
                            Discord.BuildDiscordEmbed("A player was banned from the server.",
                                $"{characterName} was banned from the server for {command[1]}!",
                                "Global Ban",
                                "https://imperialproduction.blob.core.windows.net/shopcoreproducts/productlogos/194/13260ab6-c9b2-d350-64f3-39f360c60fe6/thumbnail.png",
                                GlobalBan.Instance.Configuration.Instance.DiscordBanWebhookColor,
                                new[]
                                {
                                    Discord.BuildDiscordField("Steam64ID", steamId.ToString(), true),
                                    Discord.BuildDiscordField("Banned By", caller.DisplayName, true),
                                    Discord.BuildDiscordField("Time of Ban",
                                        DateTime.Now.ToString(CultureInfo.InvariantCulture), false),
                                    Discord.BuildDiscordField("Reason of Ban", command[1], true),
                                    Discord.BuildDiscordField("Ban duration", duration.ToString(), true)
                                }));
                    }
                    else
                    {
                        UnturnedChat.Say(caller, GlobalBan.Instance.Translate("command_generic_invalid_parameter"));
                    }
                }
                else if (command.Length == 2)
                {
                    GlobalBan.Instance.database.BanPlayer(characterName, steamId.ToString(), ip, hwid, adminName,
                        command[1], 0);
                    UnturnedChat.Say(GlobalBan.Instance.Translate("command_ban_public_reason", characterName,
                        command[1]));
                    if (isOnline)
                        Provider.ban(steamId, command[1], uint.MaxValue);

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
                                Discord.BuildDiscordField("Ban duration", uint.MaxValue.ToString(), true)
                            }));
                }
                else
                {
                    GlobalBan.Instance.database.BanPlayer(characterName, steamId.ToString(), ip, hwid, adminName, "",
                        0);
                    UnturnedChat.Say(GlobalBan.Instance.Translate("command_ban_public", characterName));
                    if (isOnline)
                        Provider.ban(steamId, GlobalBan.Instance.Translate("command_ban_private_default_reason"),
                            uint.MaxValue);

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
                                Discord.BuildDiscordField("Ban duration", uint.MaxValue.ToString(), true)
                            }));
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }
    }
}