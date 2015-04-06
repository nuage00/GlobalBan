using Rocket.Logging;
using Rocket.RocketAPI;
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

        public void Execute(RocketPlayer caller, string command)
        {
            SteamPlayer otherSteamPlayer = null;
            SteamPlayerID steamPlayerID = null;
            string[] componentsFromSerial = Parser.getComponentsFromSerial(command, '/');

            if (componentsFromSerial.Length == 0 ||componentsFromSerial.Length > 2)
            {
                RocketChatManager.Say(caller, GlobalBan.Instance.Translate("command_generic_invalid_parameter"));
                return;
            }

            bool isOnline = false;
            string steamid = null;
            string charactername = null;
            if (!PlayerTool.tryGetSteamPlayer(componentsFromSerial[0], out otherSteamPlayer))
            {
                KeyValuePair<string, string> player = GlobalBan.GetPlayer(command);
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

            if (componentsFromSerial.Length >= 2)
            {
                GlobalBan.Instance.Database.BanPlayer(charactername, steamid, caller.ToString(), componentsFromSerial[1],31536000);
                RocketChatManager.Say(GlobalBan.Instance.Translate("command_ban_public_reason", charactername, componentsFromSerial[1]));
                if (isOnline)
                    Steam.kick(steamPlayerID.CSteamID, componentsFromSerial[1]);
            }
            else
            {
                GlobalBan.Instance.Database.BanPlayer(charactername, steamid, caller.ToString(), "", 31536000);
                RocketChatManager.Say(GlobalBan.Instance.Translate("command_ban_public", charactername));
                if (isOnline)
                    Steam.kick(steamPlayerID.CSteamID, GlobalBan.Instance.Translate("command_ban_private_default_reason"));
            }
        }
    }
}
