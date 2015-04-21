using Rocket.RocketAPI;
using SDG;

namespace unturned.ROCKS.GlobalBan
{
    public class CommandKick : IRocketCommand
    {
        public string Help
        {
            get { return "Kicks a player"; }
        }

        public string Name
        {
            get { return "kick"; }
        }

        public bool RunFromConsole
        {
            get { return true; }
        }

        public void Execute(RocketPlayer caller, string command)
        {
            string[] componentsFromSerial = Parser.getComponentsFromSerial(command, '/');

            if (componentsFromSerial.Length == 0 ||componentsFromSerial.Length > 2)
            {
                RocketChatManager.Say(caller, GlobalBan.Instance.Translate("command_generic_invalid_parameter"));
                return;
            }
            RocketPlayer playerToKick = RocketPlayer.FromName(componentsFromSerial[0]);
            if (playerToKick == null)
            {
                RocketChatManager.Say(caller, GlobalBan.Instance.Translate("command_generic_player_not_found"));
                return;
            }
            if (componentsFromSerial.Length >= 2)
            {
                RocketChatManager.Say(GlobalBan.Instance.Translate("command_kick_public_reason", playerToKick.SteamName, componentsFromSerial[1]));
                Steam.kick(playerToKick.CSteamID, componentsFromSerial[1]);
            }
            else
            {
                RocketChatManager.Say(GlobalBan.Instance.Translate("command_kick_public", playerToKick.SteamName));
                Steam.kick(playerToKick.CSteamID, GlobalBan.Instance.Translate("command_kick_private_default_reason"));
            }
        }
    }
}
