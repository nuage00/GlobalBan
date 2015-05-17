using Rocket.Unturned;
using Rocket.Unturned.Commands;
using Rocket.Unturned.Player;
using SDG;
using System.Collections.Generic;

namespace unturned.ROCKS.GlobalBan
{
    public class CommandSlay : IRocketCommand
    {
        public string Help
        {
            get { return  "Banns a player for a year"; }
        }

        public string Name
        {
            get { return "slay"; }
        }

        public bool RunFromConsole
        {
            get { return true; }
        }

        public void Execute(RocketPlayer caller, params string[] command)
        {
            SteamPlayer otherSteamPlayer = null;
            SteamPlayerID steamPlayerID = null;

            if (command.Length == 0 || command.Length > 2)
            {
                RocketChat.Say(caller, GlobalBan.Instance.Translate("command_generic_invalid_parameter"));
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
                    RocketChat.Say(caller, GlobalBan.Instance.Translate("command_generic_player_not_found"));
                    return;
                }
            }
            else
            {
                isOnline = true;
                steamid = otherSteamPlayer.SteamPlayerID.CSteamID.ToString();
                charactername = otherSteamPlayer.SteamPlayerID.CharacterName;
            }

            if (command.Length >= 2)
            {
                GlobalBan.Instance.Database.BanPlayer(charactername, steamid, caller.ToString(), command[1], 31536000);
                RocketChat.Say(GlobalBan.Instance.Translate("command_ban_public_reason", charactername, command[1]));
                if (isOnline)
                    Steam.kick(steamPlayerID.CSteamID, command[1]);
            }
            else
            {
                GlobalBan.Instance.Database.BanPlayer(charactername, steamid, caller.ToString(), "", 31536000);
                RocketChat.Say(GlobalBan.Instance.Translate("command_ban_public", charactername));
                if (isOnline)
                    Steam.kick(steamPlayerID.CSteamID, GlobalBan.Instance.Translate("command_ban_private_default_reason"));
            }
        }
    }
}
