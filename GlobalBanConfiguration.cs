using Rocket.API;

namespace unturned.ROCKS.GlobalBan
{
    public class GlobalBanConfiguration : IRocketPluginConfiguration
    {
        public string DatabaseAddress = "localhost";
        public string DatabaseUsername = "unturned";
        public string DatabasePassword = "password";
        public string DatabaseName = "unturned";
        public string DatabaseTableName = "banlist";
        public int DatabasePort = 3306;
    }
}
