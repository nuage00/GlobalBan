using Rocket.RocketAPI;
using SDG;
using Steamworks;
using System;

namespace unturned.ROCKS.GlobalBan
{
    public class CommandUnban : IRocketCommand
    {
        public string Help
        {
            get { return "Unbanns a player"; }
        }

        public string Name
        {
            get { return "unban"; }
        }

        public bool RunFromConsole
        {
            get { return true; }
        }

        public void Execute(RocketPlayer caller, params string[] command)
        {
            if (command.Length != 1)
            {
                RocketChatManager.Say(caller, GlobalBan.Instance.Translate("command_generic_invalid_parameter"));
                return;
            }

            unturned.ROCKS.GlobalBan.DatabaseManager.UnbanResult name = GlobalBan.Instance.Database.UnbanPlayer(command[0]);
            if (!SteamBlacklist.unban(new CSteamID(name.Id)) && String.IsNullOrEmpty(name.Name))
            {
                RocketChatManager.Say(caller, GlobalBan.Instance.Translate("command_generic_player_not_found"));
                return;
            }
            else
            {
                RocketChatManager.Say("The player " + name + " was unbanned");
            }
        }

    }
}
