using Rocket.RocketAPI;
using SDG;
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

        public void Execute(Steamworks.CSteamID caller, string command)
        {
            string name = GlobalBan.Instance.Database.UnbanPlayer(command);
            if (String.IsNullOrEmpty(name))
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
