using Rocket.Unturned;
using Rocket.Unturned.Commands;
using Rocket.Unturned.Player;
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

        public string Syntax
        {
            get { return "<player> [reason]"; }
        }

        public void Execute(RocketPlayer caller, params string[] command)
        {

            if (command.Length == 0 || command.Length > 2)
            {
                RocketChat.Say(caller, GlobalBan.Instance.Translate("command_generic_invalid_parameter"));
                return;
            }
            RocketPlayer playerToKick = RocketPlayer.FromName(command[0]);
            if (playerToKick == null)
            {
                RocketChat.Say(caller, GlobalBan.Instance.Translate("command_generic_player_not_found"));
                return;
            }
            if (command.Length >= 2)
            {
                RocketChat.Say(GlobalBan.Instance.Translate("command_kick_public_reason", playerToKick.SteamName, command[1]));
                Steam.kick(playerToKick.CSteamID, command[1]);
            }
            else
            {
                RocketChat.Say(GlobalBan.Instance.Translate("command_kick_public", playerToKick.SteamName));
                Steam.kick(playerToKick.CSteamID, GlobalBan.Instance.Translate("command_kick_private_default_reason"));
            }
        }
    }
}
