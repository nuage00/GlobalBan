using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using OpenMod.API.Plugins;
using OpenMod.API.Users;
using OpenMod.Core.Commands;
using OpenMod.Core.Console;
using OpenMod.Core.Users;
using OpenMod.Unturned.Users;
using Pustalorc.GlobalBan.API.Enums;
using SDG.Unturned;
using Command = OpenMod.Core.Commands.Command;

namespace Pustalorc.GlobalBan.Commands
{
    [Command("kick")]
    [CommandSyntax("<player> [reason]")]
    [CommandDescription("Kicks a player from the server.")]
    public class CommandKick : Command
    {
        private readonly IUserManager m_UserManager;
        private readonly IStringLocalizer m_StringLocalizer;
        private readonly IPluginAccessor<GlobalBanPlugin> m_Plugin;
        private readonly ILogger<CommandKick> m_Logger;

        public CommandKick(IUserManager userManager, IStringLocalizer stringLocalizer,
            IPluginAccessor<GlobalBanPlugin> globalBanPlugin, ILogger<CommandKick> logger,
            IServiceProvider serviceProvider) : base(serviceProvider)
        {
            m_UserManager = userManager;
            m_StringLocalizer = stringLocalizer;
            m_Plugin = globalBanPlugin;
            m_Logger = logger;
        }

        protected override async Task OnExecuteAsync()
        {
            var actor = Context.Actor;

            // Parse arguments
            var target = await Context.Parameters.GetAsync<string>(0);
            var reason = Context.Parameters.GetArgumentLine(1);

            // Try find user to kick
            var user = await m_UserManager.FindUserAsync(KnownActorTypes.Player, target, UserSearchMode.NameOrId);

            if (!(user is UnturnedUser player))
            {
                await actor.PrintMessageAsync(m_StringLocalizer["commands:global:playernotfound",
                    new {Input = target}]);
                return;
            }

            // User was found, kick.
            await UniTask.SwitchToMainThread();
            Provider.kick(player.SteamId, reason);
            await UniTask.SwitchToThreadPool();

            var translation =
                m_StringLocalizer["commands:kick:kicked", new {Player = player.DisplayName, Reason = reason}];
            await m_UserManager.BroadcastAsync(translation);
            await actor.PrintMessageAsync(translation);
            if (!(actor is ConsoleActor))
                m_Logger.LogInformation(translation);

            await m_Plugin.Instance?.SendWebhookAsync(WebhookType.Kick, player.DisplayName, actor.DisplayName, reason,
                player.SteamId.ToString(), 0);
        }
    }
}