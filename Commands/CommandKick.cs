using System;
using System.Collections.Generic;
using System.Globalization;
using fr34kyn01535.GlobalBan.API;
using JetBrains.Annotations;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;

namespace fr34kyn01535.GlobalBan.Commands
{
    public class CommandKick : IRocketCommand
    {
        [NotNull] public string Help => "Kicks a player";

        [NotNull] public string Name => "kick";

        [NotNull] public string Syntax => "<player> [reason]";

        [NotNull] public List<string> Aliases => new List<string>();

        public AllowedCaller AllowedCaller => AllowedCaller.Both;

        [NotNull] public List<string> Permissions => new List<string> {"globalban.kick"};

        public void Execute(IRocketPlayer caller, [NotNull] params string[] command)
        {
            if (command.Length == 0 || command.Length > 2)
            {
                UnturnedChat.Say(caller, GlobalBan.Instance.Translate("command_generic_invalid_parameter"));
                return;
            }

            var playerToKick = UnturnedPlayer.FromName(command[0]);
            if (playerToKick == null)
            {
                UnturnedChat.Say(caller, GlobalBan.Instance.Translate("command_generic_player_not_found"));
                return;
            }

            if (command.Length >= 2)
            {
                UnturnedChat.Say(GlobalBan.Instance.Translate("command_kick_public_reason", playerToKick.CharacterName,
                    command[1]));
                Provider.kick(playerToKick.CSteamID, command[1]);

                Discord.SendWebhookPost(GlobalBan.Instance.Configuration.Instance.DiscordKickWebhook,
                    Discord.BuildDiscordEmbed("A player was kicked from the server.",
                        $"{playerToKick.CharacterName} was kicked from the server for {command[1]}!",
                        GlobalBan.Instance.Configuration.Instance.WebhookDisplayName,
                        GlobalBan.Instance.Configuration.Instance.WebhookImageURL,
                        GlobalBan.Instance.Configuration.Instance.DiscordKickWebhookColor,
                        new[]
                        {
                            Discord.BuildDiscordField("Steam64ID", playerToKick.CSteamID.ToString(), true),
                            Discord.BuildDiscordField("Kicked By", caller.DisplayName, true),
                            Discord.BuildDiscordField("Time of Kick",
                                DateTime.Now.ToString(CultureInfo.InvariantCulture), false),
                            Discord.BuildDiscordField("Reason of Kick", command[1], true)
                        }));
            }
            else
            {
                UnturnedChat.Say(GlobalBan.Instance.Translate("command_kick_public", playerToKick.CharacterName));
                Provider.kick(playerToKick.CSteamID,
                    GlobalBan.Instance.Translate("command_kick_private_default_reason"));

                Discord.SendWebhookPost(GlobalBan.Instance.Configuration.Instance.DiscordKickWebhook,
                    Discord.BuildDiscordEmbed("A player was kicked from the server.",
                        $"{playerToKick.CharacterName} was kicked from the server for {GlobalBan.Instance.Translate("command_kick_private_default_reason")}!",
                        GlobalBan.Instance.Configuration.Instance.WebhookDisplayName,
                        GlobalBan.Instance.Configuration.Instance.WebhookImageURL,
                        GlobalBan.Instance.Configuration.Instance.DiscordKickWebhookColor,
                        new[]
                        {
                            Discord.BuildDiscordField("Steam64ID", playerToKick.CSteamID.ToString(), true),
                            Discord.BuildDiscordField("Kicked By", caller.DisplayName, true),
                            Discord.BuildDiscordField("Time of Kick",
                                DateTime.Now.ToString(CultureInfo.InvariantCulture), false),
                            Discord.BuildDiscordField("Reason of Kick",
                                GlobalBan.Instance.Translate("command_kick_private_default_reason"), true)
                        }));
            }
        }
    }
}