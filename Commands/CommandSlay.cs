using System.Collections.Generic;
using Rocket.API;
using Rocket.Unturned.Chat;
using SDG.Unturned;
using Steamworks;

namespace fr34kyn01535.GlobalBan.Commands
{
    public class CommandSlay : IRocketCommand
    {
        public string Help => "Banns a player for a year";

        public string Name => "slay";

        public string Syntax => "<player>";

        public List<string> Aliases => new List<string>();

        public AllowedCaller AllowedCaller => AllowedCaller.Both;

        public List<string> Permissions => new List<string> {"globalban.slay"};

        public void Execute(IRocketPlayer caller, params string[] command)
        {
            SteamPlayer otherSteamPlayer = null;
            SteamPlayerID steamPlayerId = null;

            if (command.Length == 0 || command.Length > 2)
            {
                UnturnedChat.Say(caller, GlobalBan.Instance.Translate("command_generic_invalid_parameter"));
                return;
            }

            var isOnline = false;
            CSteamID steamId;
            string characterName = null;
            if (!PlayerTool.tryGetSteamPlayer(command[0], out otherSteamPlayer))
            {
                var player = GlobalBan.GetPlayer(command[0]);
                if (player.Key != null)
                {
                    steamId = player.Key;
                    characterName = player.Value;
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
                steamId = otherSteamPlayer.playerID.steamID;
                characterName = otherSteamPlayer.playerID.characterName;
            }

            if (command.Length >= 2)
            {
                GlobalBan.Instance.database.BanPlayer(characterName, steamId.ToString(), caller.DisplayName, command[1],
                    31536000);
                UnturnedChat.Say(GlobalBan.Instance.Translate("command_ban_public_reason", characterName, command[1]));
                if (isOnline)
                    Provider.kick(steamPlayerId.steamID, command[1]);
            }
            else
            {
                GlobalBan.Instance.database.BanPlayer(characterName, steamId.ToString(), caller.DisplayName, "",
                    31536000);
                UnturnedChat.Say(GlobalBan.Instance.Translate("command_ban_public", characterName));
                if (isOnline)
                    Provider.kick(steamPlayerId.steamID,
                        GlobalBan.Instance.Translate("command_ban_private_default_reason"));
            }
        }
    }
}