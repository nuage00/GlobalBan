using System;
using System.Collections.Generic;
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
                string characterName = null;


                var otherPlayer = UnturnedPlayer.FromName(command[0]);
                var otherPlayerId = command.GetCSteamIDParameter(0);
                if (otherPlayer == null || otherPlayer.CSteamID.ToString() == "0" ||
                    caller != null && otherPlayer.CSteamID.ToString() == caller.Id)
                {
                    var player = GlobalBan.GetPlayer(command[0]);
                    if (player.Key.ToString() != "0")
                    {
                        steamId = player.Key;
                        characterName = player.Value;
                    }
                    else
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
                }
                else
                {
                    isOnline = true;
                    steamId = otherPlayer.CSteamID;
                    characterName = otherPlayer.CharacterName;
                }

                var adminName = "Console";
                if (caller != null) adminName = caller.ToString();

                if (command.Length == 3)
                {
                    var duration = 0;
                    if (GlobalBan.TryConvertTimeToSeconds(command[2], out duration))
                    {
                        GlobalBan.Instance.database.BanPlayer(characterName, steamId.ToString(), adminName, command[1],
                            duration);
                        UnturnedChat.Say(GlobalBan.Instance.Translate("command_ban_public_reason", characterName,
                            command[1]));
                        if (isOnline)
                            Provider.ban(steamId, command[1], (uint) duration);
                    }
                    else
                    {
                        UnturnedChat.Say(caller, GlobalBan.Instance.Translate("command_generic_invalid_parameter"));
                    }
                }
                else if (command.Length == 2)
                {
                    GlobalBan.Instance.database.BanPlayer(characterName, steamId.ToString(), adminName, command[1], 0);
                    UnturnedChat.Say(GlobalBan.Instance.Translate("command_ban_public_reason", characterName,
                        command[1]));
                    if (isOnline)
                        Provider.ban(steamId, command[1], uint.MaxValue);
                }
                else
                {
                    GlobalBan.Instance.database.BanPlayer(characterName, steamId.ToString(), adminName, "", 0);
                    UnturnedChat.Say(GlobalBan.Instance.Translate("command_ban_public", characterName));
                    if (isOnline)
                        Provider.ban(steamId, GlobalBan.Instance.Translate("command_ban_private_default_reason"),
                            uint.MaxValue);
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }
    }
}