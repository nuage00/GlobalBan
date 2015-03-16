using Rocket.RocketAPI;

namespace unturned.ROCKS.GlobalBan
{
    public class GlobalBanConfiguration : RocketConfiguration
    {
        public string DatabaseAddress;
        public string DatabaseUsername;
        public string DatabasePassword;
        public string DatabaseName;
        public string DatabaseTableName;
        public int DatabasePort;

        public RocketConfiguration DefaultConfiguration
        {
            get {
                GlobalBanConfiguration config = new GlobalBanConfiguration();
                config.DatabaseAddress = "localhost";
                config.DatabaseUsername = "unturned";
                config.DatabasePassword = "password";
                config.DatabaseName = "unturned";
                config.DatabaseTableName = "banlist";
                config.DatabasePort = 3306;
                return config;
            }
        }
    }
}
