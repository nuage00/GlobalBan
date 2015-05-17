using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using Rocket.Unturned.Plugins;
using SDG;
using Steamworks;
using System;
using System.Collections.Generic;

namespace unturned.ROCKS.GlobalBan
{
    public class GlobalBan : RocketPlugin<GlobalBanConfiguration>
    {
        public static GlobalBan Instance;
        public DatabaseManager Database;

        public static Dictionary<string, string> Players = new Dictionary<string,string>();

        protected override void Load()
        {
            Instance = this;
            Database = new DatabaseManager();
            RocketServerEvents.OnPlayerConnected += Events_OnPlayerConnected;
        }

        public override System.Collections.Generic.Dictionary<string, string> DefaultTranslations
        {
            get
            {
                return new System.Collections.Generic.Dictionary<string, string>() {
                    {"default_banmessage","you are banned, contact the staff if you feel this is a mistake."},
                    {"command_generic_invalid_parameter","Invalid parameter"},
                    {"command_generic_player_not_found","Player not found"},
                    {"command_ban_public_reason", "The player {0} was banned for: {1}"},
                    {"command_ban_public","The player {0} was banned"},
                    {"command_ban_private_default_reason","you were banned from the server"},
                    {"command_kick_public_reason", "The player {0} was kicked for: {1}"},
                    {"command_kick_public","The player {0} was kicked"},
                    {"command_kick_private_default_reason","you were kicked from the server"},
                };
            }
        }

        public static KeyValuePair<string, string> GetPlayer(string search) {
            foreach (KeyValuePair<string, string> pair in Players) {
                if (pair.Key.ToLower().Contains(search.ToLower()) || pair.Value.ToLower().Contains(search.ToLower()))
                {
                    return pair;
                }
            }
            return new KeyValuePair<string, string>(null,null);
        }

        public void Events_OnPlayerConnected(RocketPlayer player)
        {
            if (!Players.ContainsKey(player.CSteamID.ToString()))
                Players.Add(player.CSteamID.ToString(), player.CharacterName);

            try
            {
                CSteamID cSteamID = player.CSteamID;
                string banned = Database.IsBanned(cSteamID.ToString());
                if (banned != null)
                {
                    if (banned == "") banned = Translate("default_banmessage");
                    Steam.kick(cSteamID, banned);
                }
            }
            catch (Exception)
            {
                //Nelson has to fix that....
            }
        }
    }
}
