using System;
using Rocket.API;

namespace fr34kyn01535.GlobalBan
{
    public class GlobalBanConfiguration : IRocketPluginConfiguration
    {
        public string DatabaseAddress;
        public string DatabaseUsername;
        public string DatabasePassword;
        public string DatabaseName;
        public string DatabaseTableName;
        public int DatabasePort;
        public bool KickInsteadReject = false;

        public void LoadDefaults()
        {
            DatabaseAddress = "localhost";
            DatabaseUsername = "unturned";
            DatabasePassword = "password";
            DatabaseName = "unturned";
            DatabaseTableName = "banlist";
            DatabasePort = 3306;
            KickInsteadReject = false;
        }
    }
}
