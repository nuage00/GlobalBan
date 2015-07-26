using Rocket.API;
using Rocket.Unturned;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Commands;
using Rocket.Unturned.Player;
using SDG;
using SDG.Unturned;
using Steamworks;
using System.Collections.Generic;

namespace unturned.ROCKS.GlobalBan
{
    public class CommandSlay : IRocketCommand
    {
        public string Help
        {
            get { return  "Banns a player for a year"; }
        }

        public string Name
        {
            get { return "slay"; }
        }

        public string Syntax
        {
            get { return "<player>"; }
        }

        public List<string> Aliases
        {
            get { return new List<string>(); }
        }

        public bool AllowFromConsole
        {
            get { return true; }
        }

        public List<string> Permissions
        {
            get
            {
                return new List<string>() { "globalban.slay" };
            }
        }

        public void Execute(IRocketPlayer caller, params string[] command)
        {
            SteamPlayer otherSteamPlayer = null;
            SteamPlayerID steamPlayerID = null;

            if (command.Length == 0 || command.Length > 2)
            {
                UnturnedChat.Say(caller, GlobalBan.Instance.Translate("command_generic_invalid_parameter"));
                return;
            }

            bool isOnline = false;
            CSteamID steamid;
            string charactername = null;
            if (!PlayerTool.tryGetSteamPlayer(command[0], out otherSteamPlayer))
            {
                KeyValuePair<CSteamID, string> player = GlobalBan.GetPlayer(command[0]);
                if (player.Key != null)
                {
                    steamid = player.Key;
                    charactername = player.Value;
                }
                else
                {
                    UnturnedChat.Say(caller, GlobalBan.Instance.Translate("command_generic_player_not_found"));
                    return;
                }
            }
            else
            {
                isOnline = true;
                steamid = otherSteamPlayer.SteamPlayerID.CSteamID;
                charactername = otherSteamPlayer.SteamPlayerID.CharacterName;
            }

            if (command.Length >= 2)
            {
                GlobalBan.Instance.Database.BanPlayer(charactername, steamid.ToString(), caller.ToString(), command[1], 31536000);
                UnturnedChat.Say(GlobalBan.Instance.Translate("command_ban_public_reason", charactername, command[1]));
                if (isOnline)
                    Steam.kick(steamPlayerID.CSteamID, command[1]);
            }
            else
            {
                GlobalBan.Instance.Database.BanPlayer(charactername, steamid.ToString(), caller.ToString(), "", 31536000);
                UnturnedChat.Say(GlobalBan.Instance.Translate("command_ban_public", charactername));
                if (isOnline)
                    Steam.kick(steamPlayerID.CSteamID, GlobalBan.Instance.Translate("command_ban_private_default_reason"));
            }
        }
    }
}
