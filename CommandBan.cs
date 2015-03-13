using Rocket.RocketAPI;
using SDG;

namespace unturned.ROCKS.GlobalBan
{
    class CommandBan : Command
    {
        public CommandBan()
        {
            base.commandName = "ban";
            base.commandHelp = "Banns a player";
            base.commandInfo = commandName + " - " + commandHelp;
        }

        protected override void execute(SteamPlayerID caller, string command)
        {
            SteamPlayerID steamPlayerID = null; ;
            string[] componentsFromSerial = Parser.getComponentsFromSerial(command, '/');

            if (componentsFromSerial.Length == 0 ||componentsFromSerial.Length > 2)
            {
                RocketChatManager.Say(caller.CSteamID, GlobalBan.Instance.Translate("command_generic_invalid_parameter"));
                return;
            }
            if (!SteamPlayerlist.tryGetPlayer(componentsFromSerial[0], out steamPlayerID))
            {
                RocketChatManager.Say(caller.CSteamID, GlobalBan.Instance.Translate("command_generic_player_not_found"));
                return;
            }

            if (componentsFromSerial.Length >= 2)
            {
                GlobalBan.Instance.Database.BanPlayer(steamPlayerID.CSteamID.ToString(), caller.CSteamID.ToString(), componentsFromSerial[1]);
                RocketChatManager.Say(GlobalBan.Instance.Translate("command_ban_public_reason", steamPlayerID.SteamName, componentsFromSerial[1]));
                Steam.kick(steamPlayerID.CSteamID, componentsFromSerial[1]);
            }
            else
            {
                GlobalBan.Instance.Database.BanPlayer(steamPlayerID.CSteamID.ToString(), caller.CSteamID.ToString(), "");
                RocketChatManager.Say(GlobalBan.Instance.Translate("command_ban_public", steamPlayerID.SteamName));
                Steam.kick(steamPlayerID.CSteamID, GlobalBan.Instance.Translate("command_ban_private_default_reason"));
            }
        }
    }
}
