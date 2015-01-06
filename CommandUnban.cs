using Rocket.RocketAPI;
using SDG;

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
            if (!SteamPlayerlist.tryGetPlayer(command, out steamPlayerID))
            {
                RocketChat.Say(caller.CSteamID, this.Local.format("NoPlayerErrorText", new object[] { command }));
                return;
            }

            Database.UnbanPlayer(steamPlayerID.CSteamID.ToString());
            RocketChat.Say(this.Local.format("UnbanText", new object[] { steamPlayerID.SteamName }));
        }

    }
}
