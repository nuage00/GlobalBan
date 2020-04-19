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
        public string DiscordKickWebhook;
        public int DiscordKickWebhookColor;
        public string DiscordBanWebhook;
        public int DiscordBanWebhookColor;
        public string DiscordUnbanWebhook;
        public int DiscordUnbanWebhookColor;

        public void LoadDefaults()
        {
            DatabaseAddress = "127.0.0.1";
            DatabaseUsername = "unturned";
            DatabasePassword = "password";
            DatabaseName = "unturned";
            DatabaseTableName = "banlist";
            DatabasePort = 3306;
            DiscordKickWebhook = "https://discordapp.com/api/webhooks/XXXXX/YYYYYYY";
            DiscordKickWebhookColor = 16776960;
            DiscordBanWebhook = "https://discordapp.com/api/webhooks/XXXXX/YYYYYYY";
            DiscordBanWebhookColor = 16711680;
            DiscordUnbanWebhook = "https://discordapp.com/api/webhooks/XXXXX/YYYYYYY";
            DiscordUnbanWebhookColor = 65280;
        }
    }
}