using Rocket.RocketAPI;
using SDG;

namespace unturned.ROCKS.GlobalBan
{
    class CommandBan : IRocketCommand
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
            SteamPlayer steamPlayer = null;
            SteamPlayerID steamPlayerID = null;
            string[] componentsFromSerial = Parser.getComponentsFromSerial(command, '/');

            if (componentsFromSerial.Length == 0 ||componentsFromSerial.Length > 2)
            {
                RocketChatManager.Say(caller, GlobalBan.Instance.Translate("command_generic_invalid_parameter"));
                return;
            }
            if (!PlayerTool.tryGetSteamPlayer(componentsFromSerial[0], out steamPlayer))
            {
                RocketChatManager.Say(caller, GlobalBan.Instance.Translate("command_generic_player_not_found"));
                return;
            }
            steamPlayerID = steamPlayer.SteamPlayerID;
            if (componentsFromSerial.Length >= 2)
            {
                GlobalBan.Instance.Database.BanPlayer(steamPlayerID.CSteamID.ToString(), caller.ToString(), componentsFromSerial[1]);
                RocketChatManager.Say(GlobalBan.Instance.Translate("command_ban_public_reason", steamPlayerID.SteamName, componentsFromSerial[1]));
                Steam.kick(steamPlayerID.CSteamID, componentsFromSerial[1]);
            }
            else
            {
                GlobalBan.Instance.Database.BanPlayer(steamPlayerID.CSteamID.ToString(), caller.ToString(), "");
                RocketChatManager.Say(GlobalBan.Instance.Translate("command_ban_public", steamPlayerID.SteamName));
                Steam.kick(steamPlayerID.CSteamID, GlobalBan.Instance.Translate("command_ban_private_default_reason"));
            }
        }
    }
}
