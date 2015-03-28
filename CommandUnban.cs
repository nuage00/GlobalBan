using Rocket.RocketAPI;
using SDG;
using System;

namespace unturned.ROCKS.GlobalBan
{
    class CommandUnban : Command
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
            SteamPlayer steamPlayer = null;
            if (String.IsNullOrEmpty(command) || !PlayerTool.tryGetSteamPlayer(command, out steamPlayer))
            {
                RocketChatManager.Say(caller, GlobalBan.Instance.Translate("command_generic_player_not_found"));
                return;
            }

            GlobalBan.Instance.Database.UnbanPlayer(steamPlayer.SteamPlayerID.CSteamID.ToString());
            RocketChatManager.Say("The player " + steamPlayer.SteamPlayerID.SteamName + " was unbanned");
        }

    }
}
