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

        public void Execute(Steamworks.CSteamID caller, string command)
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
            Logger.Log("1");
            if (!PlayerTool.tryGetSteamPlayer(componentsFromSerial[0], out otherSteamPlayer))
            {
                Logger.Log("2");
                KeyValuePair<string, string> player = GlobalBan.GetPlayer(command);
                Logger.Log("3");
                if (player.Key != null)
                {
                    Logger.Log("4");
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
                Logger.Log("5");
                GlobalBan.Instance.Database.BanPlayer(charactername, steamid, caller.ToString(), componentsFromSerial[1]);
                Logger.Log("6");
                RocketChatManager.Say(GlobalBan.Instance.Translate("command_ban_public_reason", charactername, componentsFromSerial[1]));
                if (isOnline)
                    Steam.kick(steamPlayerID.CSteamID, componentsFromSerial[1]);
            }
            else
            {
                Logger.Log("5");
                GlobalBan.Instance.Database.BanPlayer(charactername, steamid, caller.ToString(), "");
                Logger.Log("6");
                RocketChatManager.Say(GlobalBan.Instance.Translate("command_ban_public", charactername));
                if (isOnline)
                    Steam.kick(steamPlayerID.CSteamID, GlobalBan.Instance.Translate("command_ban_private_default_reason"));
            }
        }
    }
}
