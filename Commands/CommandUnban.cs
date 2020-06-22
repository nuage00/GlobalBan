using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using fr34kyn01535.GlobalBan.API;
using JetBrains.Annotations;
using PlayerInfoLibrary;
using Rocket.API;
using Rocket.Unturned.Chat;
using Steamworks;

namespace fr34kyn01535.GlobalBan.Commands
{
    public class CommandUnban : IRocketCommand
    {
        [NotNull] public string Help => "Unbanns a player";

        [NotNull] public string Name => "unban";

        [NotNull] public string Syntax => "<player>";

        [NotNull] public List<string> Aliases => new List<string>();

        public AllowedCaller AllowedCaller => AllowedCaller.Both;

        [NotNull] public List<string> Permissions => new List<string> {"globalban.unban"};

        public async void Execute(IRocketPlayer caller, [NotNull] params string[] command)
        {
            if (command.Length == 0)
            {
                UnturnedChat.Say(caller, GlobalBan.Instance.Translate("command_generic_invalid_parameter"));
                return;
            }

            var args = command.ToList();
            var target = args.GetIRocketPlayer(out var index);
            if (index > -1)
                args.RemoveAt(index);

            if (target == null)
            {
                UnturnedChat.Say(caller, GlobalBan.Instance.Translate("command_generic_player_not_found"));
                return;
            }

            var pData = await PlayerInfoLib.Instance.database.QueryById(new CSteamID(ulong.Parse(target.Id)));

            if (!await GlobalBan.Instance.database.TryUnban(ulong.Parse(target.Id)))
            {
                UnturnedChat.Say(caller, GlobalBan.Instance.Translate("command_generic_player_not_found"));
                return;
            }

            var characterName = pData?.CharacterName ?? target.DisplayName;

            UnturnedChat.Say("The player " + characterName + " was unbanned");
            Discord.SendWebhookPost(GlobalBan.Instance.Configuration.Instance.DiscordUnbanWebhook,
                Discord.BuildDiscordEmbed("A player was unbanned from the server.",
                    $"{characterName} was unbanned from the server.",
                    GlobalBan.Instance.Configuration.Instance.WebhookDisplayName,
                    GlobalBan.Instance.Configuration.Instance.WebhookImageURL,
                    GlobalBan.Instance.Configuration.Instance.DiscordUnbanWebhookColor,
                    new[]
                    {
                        Discord.BuildDiscordField("Steam64ID", target.Id, true),
                        Discord.BuildDiscordField("Unbanned By", caller.DisplayName, true),
                        Discord.BuildDiscordField("Time of Unban", DateTime.Now.ToString(CultureInfo.InvariantCulture),
                            false)
                    }));
        }
    }
}