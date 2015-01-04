using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Steamworks;
using SDG;
using System.Reflection;
using Rocket;

namespace GlobalBan
{
    public class GlobalBan : RocketComponent
    {
        public static GlobalBanConfiguration configuration;
     
        protected override void Load()
        {
            new I18N.West.CP1250();
            configuration = Configuration.LoadConfiguration<GlobalBanConfiguration>();
            Database.CheckSchema();
        }

        protected override void onPlayerConnected(CSteamID cSteamID)
        {
            try
            {
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
