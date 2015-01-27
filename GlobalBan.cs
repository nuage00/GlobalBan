using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Steamworks;
using SDG;
using System.Reflection;
using Rocket;
using Rocket.RocketAPI;

namespace unturned.ROCKS.GlobalBan
{
    public class GlobalBan : RocketPlugin<GlobalBanConfiguration>
    {
        protected override void Load()
        {
            new I18N.West.CP1250();
            Database.CheckSchema();
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
