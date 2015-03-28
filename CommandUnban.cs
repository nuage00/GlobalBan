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
            SteamPlayer steamPlayer = null;
            if (String.IsNullOrEmpty(command) || !PlayerTool.tryGetSteamPlayer(command, out steamPlayer))
            {
                RocketChatManager.Say(caller.CSteamID, GlobalBan.Instance.Translate("command_generic_player_not_found"));
                return;
            }

            GlobalBan.Instance.Database.UnbanPlayer(steamPlayer.SteamPlayerID.CSteamID.ToString());
            RocketChatManager.Say("The player " + steamPlayer.SteamPlayerID.SteamName + " was unbanned");
        }

    }
}
