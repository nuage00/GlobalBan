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

        public void Execute(RocketPlayer caller, string command)
        {
            unturned.ROCKS.GlobalBan.DatabaseManager.UnbanResult name = GlobalBan.Instance.Database.UnbanPlayer(command);
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
