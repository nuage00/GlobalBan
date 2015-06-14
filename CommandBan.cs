using Rocket.Unturned;
using Rocket.Unturned.Commands;
using Rocket.Unturned.Logging;
using Rocket.Unturned.Player;
using SDG;
using Steamworks;
using System.Collections.Generic;

namespace unturned.ROCKS.GlobalBan
{
    public class CommandBan : IRocketCommand
    {
        public string Help
        {
            get { return  "Banns a player"; }
        }

        public string Name
        {
            get { return "ban"; }
        }

        public bool RunFromConsole
        {
            get { return true; }
        }

        public string Syntax
        {
            get { return "<player> [reason] [duration]"; }
        }

        public List<string> Aliases {
            get { return new List<string>(); }
        }

        public void Execute(RocketPlayer caller, params string[] command)
        {
            try
            {
                if (command.Length == 0 || command.Length > 3)
                {
                    RocketChat.Say(caller, GlobalBan.Instance.Translate("command_generic_invalid_parameter"));
                    return;
                }

                bool isOnline = false;

                CSteamID steamid;
                string charactername = null;

                RocketPlayer otherPlayer = RocketPlayer.FromName(command[0]);
                if (otherPlayer == null || otherPlayer.CSteamID.ToString() == "0" || otherPlayer.CSteamID == caller.CSteamID)
                {
                    KeyValuePair<CSteamID, string> player = GlobalBan.GetPlayer(command[0]);
                    if (player.Key.ToString() != "0")
                    {
                        steamid = player.Key;
                        charactername = player.Value;
                    }
                    else
                    {
                        RocketChat.Say(caller, GlobalBan.Instance.Translate("command_generic_player_not_found"));
                        return;
                    }
                }
                else
                {
                    isOnline = true;
                    steamid = otherPlayer.CSteamID;
                    charactername = otherPlayer.CharacterName;
                }

                if (command.Length == 3)
                {
                    int duration = 0;
                    if (int.TryParse(command[2], out duration))
                    {
                        GlobalBan.Instance.Database.BanPlayer(charactername, steamid.ToString(), caller.ToString(), command[1], duration);
                        RocketChat.Say(GlobalBan.Instance.Translate("command_ban_public_reason", charactername, command[1]));
                        if (isOnline)
                            Steam.kick(steamid, command[1]);
                    }
                    else
                    {
                        RocketChat.Say(caller, GlobalBan.Instance.Translate("command_generic_invalid_parameter"));
                        return;
                    }
                }
                else if (command.Length == 2)
                {

                    GlobalBan.Instance.Database.BanPlayer(charactername, steamid.ToString(), caller.ToString(), command[1], 0);
                    RocketChat.Say(GlobalBan.Instance.Translate("command_ban_public_reason", charactername, command[1]));
                    if (isOnline)
                        Steam.kick(steamid, command[1]);
                }
                else
                {
                    GlobalBan.Instance.Database.BanPlayer(charactername, steamid.ToString(), caller.ToString(), "", 0);
                    RocketChat.Say(GlobalBan.Instance.Translate("command_ban_public", charactername));
                    if (isOnline)
                        Steam.kick(steamid, GlobalBan.Instance.Translate("command_ban_private_default_reason"));
                }

            }
            catch (System.Exception ex)
            {
                Logger.LogException(ex);
            }
        }
    }
}
