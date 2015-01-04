using SDG;

namespace GlobalBan
{
    class CommandUnban : Command
    {
        public CommandUnban()
        {
            base.commandName = "unban";
            base.commandInfo = base.commandHelp = "Unbanns a player";
        }

        public override void execute(SteamPlayerID caller, string command)
        {
            string[] commandArray = command.Split(' ');

            if (commandArray.Length < 2)
            {
                ChatManager.say(caller.CSteamId, "Missing arguments");
                return;
            }

            SteamPlayerID steamPlayerID;
            if (SteamPlayerlist.tryGetPlayer(commandArray[1], out steamPlayerID))
            {
                Database.UnbanPlayer(steamPlayerID.CSteamId.ToString());
                ChatManager.say("Unbanned " + steamPlayerID.IngameName);
            }else{
                ChatManager.say(caller.CSteamId,"Failed to find player");
            }
        }

    }
}
