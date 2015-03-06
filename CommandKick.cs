using Rocket.RocketAPI;
using SDG;

namespace unturned.ROCKS.GlobalBan
{
    class CommandKick : Command
    {
        public CommandKick()
        {
            base.commandName = "kick";
            base.commandHelp = "Kicks a player";
            base.commandInfo = commandName + " - " + commandHelp;
        }

        protected override void execute(SteamPlayerID caller, string command)
        {
            SteamPlayerID steamPlayerID = null; ;
            string[] componentsFromSerial = Parser.getComponentsFromSerial(command, '/');

            if (componentsFromSerial.Length == 0 ||componentsFromSerial.Length > 2)
            {
                RocketChatManager.Say(caller.CSteamID,"Invalid parameter");
                return;
            }
            if (!SteamPlayerlist.tryGetPlayer(componentsFromSerial[0], out steamPlayerID))
            {
                RocketChatManager.Say(caller.CSteamID, "Player not found");
                return;
            }

            if (componentsFromSerial.Length >= 2)
            {
                GlobalBan.Instance.Database.BanPlayer(steamPlayerID.CSteamID.ToString(), caller.CSteamID.ToString(), componentsFromSerial[1]);
                RocketChatManager.Say("The player " + steamPlayerID.SteamName + " was kicked for: " + componentsFromSerial[1]);
                Steam.kick(steamPlayerID.CSteamID, componentsFromSerial[1]);
            }
            else
            {
                GlobalBan.Instance.Database.BanPlayer(steamPlayerID.CSteamID.ToString(), caller.CSteamID.ToString(), "");
                RocketChatManager.Say("The player " + steamPlayerID.SteamName + " was kicked");
                Steam.kick(steamPlayerID.CSteamID, "you were kicked from the server");
            }
        }
    }
}
