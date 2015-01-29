using Rocket.RocketAPI;
using SDG;
using Steamworks;
using System;

namespace unturned.ROCKS.GlobalBan
{
    public class GlobalBan : RocketPlugin<GlobalBanConfiguration>
    {
        public static GlobalBan Instance;
        public DatabaseManager Database;
        protected override void Load()
        {
            Instance = this;
            Database = new DatabaseManager();
            RocketEvents.OnPlayerConnected += Events_OnPlayerConnected;
        }

        public void Events_OnPlayerConnected(Player player)
        {
            try
            {
                CSteamID cSteamID = player.SteamChannel.SteamPlayer.SteamPlayerID.CSteamID;
                string banned = Database.IsBanned(cSteamID.ToString());
                if (banned != null)
                {
                    if (banned == "") banned = "you are banned, contact the staff if you feel this is a mistake.";
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
