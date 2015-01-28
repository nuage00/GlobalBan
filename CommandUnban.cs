using Rocket.RocketAPI;
using SDG;
using System;

namespace unturned.ROCKS.GlobalBan
{
    class CommandUnban : Command
    {
        public CommandUnban()
        {
            base.commandName = "unban";
            base.commandHelp = "Unbanns a player";
            base.commandInfo = commandName + " - " + commandHelp;
        }

        protected override void execute(SteamPlayerID caller, string command)
        {
            SteamPlayerID steamPlayerID = null;
            if (String.IsNullOrEmpty(command) || !SteamPlayerlist.tryGetPlayer(command, out steamPlayerID))
            {
                RocketChatManager.Say(caller.CSteamID, "Player not found");
                return;
            }

            GlobalBan.Database.UnbanPlayer(steamPlayerID.CSteamID.ToString());
            RocketChatManager.Say("The player " + steamPlayerID.SteamName + " was unbanned");
        }

    }
}
