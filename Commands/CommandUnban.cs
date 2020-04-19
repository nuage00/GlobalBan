using System;
using System.Collections.Generic;
using System.Globalization;
using Rocket.API;
using Rocket.Unturned.Chat;
using SDG.Unturned;
using Steamworks;

namespace fr34kyn01535.GlobalBan.Commands
{
    public class CommandUnban : IRocketCommand
    {
        public string Help => "Unbanns a player";

        public string Name => "unban";

        public string Syntax => "<player>";

        public List<string> Aliases => new List<string>();

        public AllowedCaller AllowedCaller => AllowedCaller.Both;

        public List<string> Permissions => new List<string> {"globalban.unban"};

        public void Execute(IRocketPlayer caller, params string[] command)
        {
            if (command.Length != 1)
            {
                UnturnedChat.Say(caller, GlobalBan.Instance.Translate("command_generic_invalid_parameter"));
                return;
            }

            var name = GlobalBan.Instance.database.UnbanPlayer(command[0]);
            if (!SteamBlacklist.unban(new CSteamID(name.Id)) && string.IsNullOrEmpty(name.Name))
            {
                UnturnedChat.Say(caller, GlobalBan.Instance.Translate("command_generic_player_not_found"));
            }
            else
            {
                UnturnedChat.Say("The player " + name.Name + " was unbanned");

                Discord.SendWebhookPost(GlobalBan.Instance.Configuration.Instance.DiscordUnbanWebhook,
                    Discord.BuildDiscordEmbed("A player was unbanned from the server.",
                        $"{name.Name} was unbanned from the server.", "Global Ban",
                        "https://imperialproduction.blob.core.windows.net/shopcoreproducts/productlogos/194/13260ab6-c9b2-d350-64f3-39f360c60fe6/thumbnail.png",
                        GlobalBan.Instance.Configuration.Instance.DiscordUnbanWebhookColor,
                        new[]
                        {
                            Discord.BuildDiscordField("Steam64ID", name.Id.ToString(), true),
                            Discord.BuildDiscordField("Unbanned By", caller.DisplayName, true),
                            Discord.BuildDiscordField("Time of Unban",
                                DateTime.Now.ToString(CultureInfo.InvariantCulture), false)
                        }));
            }
        }
    }
}