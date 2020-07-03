using Pustalorc.Libraries.MySqlConnectorWrapper.Configuration;
using Rocket.API;

namespace fr34kyn01535.GlobalBan.Config
{
    public class GlobalBanConfiguration : IConnectorConfiguration, IRocketPluginConfiguration
    {
        public string DatabaseAddress { get; set; }
        public string DatabaseUsername { get; set; }
        public string DatabasePassword { get; set; }
        public string DatabaseName { get; set; }
        public string DatabaseTableName;
        public ushort DatabasePort { get; set; }
        public bool UseCache { get; set; }
        public ulong CacheRefreshIntervalMilliseconds { get; set; }
        public byte CacheSize { get; set; }

        public string DiscordKickWebhook;
        public int DiscordKickWebhookColor;
        public string DiscordBanWebhook;
        public int DiscordBanWebhookColor;
        public string DiscordUnbanWebhook;
        public int DiscordUnbanWebhookColor;
        public bool BanCommandAlsoIpAndHwidBans;
        public string WebhookDisplayName;
        public string WebhookImageUrl;

        public void LoadDefaults()
        {
            DatabaseAddress = "127.0.0.1";
            DatabaseUsername = "unturned";
            DatabasePassword = "password";
            DatabaseName = "unturned";
            DatabaseTableName = "banlist";
            DatabasePort = 3306;
            UseCache = true;
            CacheRefreshIntervalMilliseconds = 30000;
            CacheSize = 48;
            DiscordKickWebhook = "https://discordapp.com/api/webhooks/XXXXX/YYYYYYY";
            DiscordKickWebhookColor = 16776960;
            DiscordBanWebhook = "https://discordapp.com/api/webhooks/XXXXX/YYYYYYY";
            DiscordBanWebhookColor = 16711680;
            DiscordUnbanWebhook = "https://discordapp.com/api/webhooks/XXXXX/YYYYYYY";
            DiscordUnbanWebhookColor = 65280;
            BanCommandAlsoIpAndHwidBans = true;
            WebhookDisplayName = "Global Ban";
            WebhookImageUrl = "https://imperialproduction.blob.core.windows.net/shopcoreproducts/productlogos/194/13260ab6-c9b2-d350-64f3-39f360c60fe6/thumbnail.png";
        }
    }
}