using Rocket.Logging;
using Rocket.RocketAPI;
using SDG;
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

        public void Execute(RocketPlayer caller, params string[] command)
        {
            SteamPlayer otherSteamPlayer = null;
            SteamPlayerID steamPlayerID = null;

            if (command.Length == 0 || command.Length > 3)
            {
                RocketChatManager.Say(caller, GlobalBan.Instance.Translate("command_generic_invalid_parameter"));
                return;
            }

            bool isOnline = false;
            string steamid = null;
            string charactername = null;
            if (!PlayerTool.tryGetSteamPlayer(command[0], out otherSteamPlayer))
            {
                KeyValuePair<string, string> player = GlobalBan.GetPlayer(command[0]);
                if (player.Key != null)
                {
                    steamid = player.Key;
                    charactername = player.Value;
                }
                else
                {
                    RocketChatManager.Say(caller, GlobalBan.Instance.Translate("command_generic_player_not_found"));
                    return;
                }
            }
            else
            {
                isOnline = true;
                steamid = otherSteamPlayer.SteamPlayerID.CSteamID.ToString();
                charactername = otherSteamPlayer.SteamPlayerID.CharacterName;
            }

            if (command.Length == 3)
            {
                int duration = 0;
                if (int.TryParse(command[2], out duration))
                {
                    GlobalBan.Instance.Database.BanPlayer(charactername, steamid, caller.ToString(), command[1], duration);
                    RocketChatManager.Say(GlobalBan.Instance.Translate("command_ban_public_reason", charactername, command[1]));
                    if (isOnline)
                        Steam.kick(steamPlayerID.CSteamID, command[1]);
                }
                else
                {
                    RocketChatManager.Say(caller, GlobalBan.Instance.Translate("command_generic_invalid_parameter"));
                    return;
                }
            }
            else if (command.Length == 2)
            {

                GlobalBan.Instance.Database.BanPlayer(charactername, steamid, caller.ToString(), command[1], 0);
                RocketChatManager.Say(GlobalBan.Instance.Translate("command_ban_public_reason", charactername, command[1]));
                if (isOnline)
                    Steam.kick(steamPlayerID.CSteamID, command[1]);
            }
            else
            {
                GlobalBan.Instance.Database.BanPlayer(charactername, steamid, caller.ToString(), "", 0);
                RocketChatManager.Say(GlobalBan.Instance.Translate("command_ban_public", charactername));
                if (isOnline)
                    Steam.kick(steamPlayerID.CSteamID, GlobalBan.Instance.Translate("command_ban_private_default_reason"));
            }
        }
    }
}
