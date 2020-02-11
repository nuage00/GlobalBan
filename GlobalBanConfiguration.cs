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
        public int KickInterval;
        public bool KickInsteadReject;

        public void LoadDefaults()
        {
            DatabaseAddress = "127.0.0.1";
            DatabaseUsername = "unturned";
            DatabasePassword = "password";
            DatabaseName = "unturned";
            DatabaseTableName = "banlist";
            DatabasePort = 3306;
            KickInterval = 10;
            KickInsteadReject = false;
        }
    }
}