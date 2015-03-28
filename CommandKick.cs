using Rocket.RocketAPI;
using SDG;

namespace unturned.ROCKS.GlobalBan
{
    public class CommandKick : IRocketCommand
    {
        string IRocketCommand.Help
        {
            get { return "Kicks a player"; }
        }

        string IRocketCommand.Name
        {
            get { return "kick"; }
        }

        bool IRocketCommand.RunFromConsole
        {
            get { return false; }
        }

        void IRocketCommand.Execute(Steamworks.CSteamID caller, string command)
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
                RocketChatManager.Say(GlobalBan.Instance.Translate("command_kick_public_reason", steamPlayerID.SteamName, componentsFromSerial[1]));
                Steam.kick(steamPlayerID.CSteamID, componentsFromSerial[1]);
            }
            else
            {
                RocketChatManager.Say(GlobalBan.Instance.Translate("command_kick_public", steamPlayerID.SteamName));
                Steam.kick(steamPlayerID.CSteamID, GlobalBan.Instance.Translate("command_kick_private_default_reason"));
            }
        }
    }
}
