using System.Collections.Generic;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;

namespace fr34kyn01535.GlobalBan.Commands
{
    public class CommandKick : IRocketCommand
    {
        public string Help => "Kicks a player";

        public string Name => "kick";

        public string Syntax => "<player> [reason]";

        public List<string> Aliases => new List<string>();

        public AllowedCaller AllowedCaller => AllowedCaller.Both;

        public List<string> Permissions => new List<string> {"globalban.kick"};

        public void Execute(IRocketPlayer caller, params string[] command)
        {
            if (command.Length == 0 || command.Length > 2)
            {
                UnturnedChat.Say(caller, GlobalBan.Instance.Translate("command_generic_invalid_parameter"));
                return;
            }

            var playerToKick = UnturnedPlayer.FromName(command[0]);
            if (playerToKick == null)
            {
                UnturnedChat.Say(caller, GlobalBan.Instance.Translate("command_generic_player_not_found"));
                return;
            }

            if (command.Length >= 2)
            {
                UnturnedChat.Say(GlobalBan.Instance.Translate("command_kick_public_reason", playerToKick.SteamName,
                    command[1]));
                Provider.kick(playerToKick.CSteamID, command[1]);
            }
            else
            {
                UnturnedChat.Say(GlobalBan.Instance.Translate("command_kick_public", playerToKick.SteamName));
                Provider.kick(playerToKick.CSteamID,
                    GlobalBan.Instance.Translate("command_kick_private_default_reason"));
            }
        }
    }
}