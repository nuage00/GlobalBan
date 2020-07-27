using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using OpenMod.API.Plugins;
using OpenMod.API.Users;
using OpenMod.Core.Commands;
using Pustalorc.GlobalBan.API.Enums;
using Pustalorc.GlobalBan.API.Services;
using Pustalorc.PlayerInfoLib.Unturned.Database;

namespace Pustalorc.GlobalBan.Commands
{
    [Command("unban")]
    [CommandSyntax("<player>")]
    [CommandDescription("Unbans a banned player globally from the network.")]
    public class CommandUnban : Command
    {
        private readonly IStringLocalizer m_StringLocalizer;
        private readonly IUserManager m_UserManager;
        private readonly IPluginAccessor<GlobalBanPlugin> m_Plugin;
        private readonly IPlayerInfoRepository m_PlayerInfoRepository;
        private readonly IGlobalBanRepository m_GlobalBanRepository;

        public CommandUnban(IStringLocalizer stringLocalizer, IUserManager userManager,
            IPluginAccessor<GlobalBanPlugin> globalBanPlugin, IPlayerInfoRepository playerInfoRepository,
            IGlobalBanRepository globalBanRepository, IServiceProvider serviceProvider) :
            base(serviceProvider)
        {
            m_UserManager = userManager;
            m_StringLocalizer = stringLocalizer;
            m_Plugin = globalBanPlugin;
            m_PlayerInfoRepository = playerInfoRepository;
            m_GlobalBanRepository = globalBanRepository;
        }

        protected override async Task OnExecuteAsync()
        {
            var actor = Context.Actor;

            var target = await Context.Parameters.GetAsync<string>(0);
            var unbans = await m_GlobalBanRepository.UnbanAutoFindAsync(target, BanSearchMode.All);
            var pData = await m_PlayerInfoRepository.FindPlayerAsync(target, UserSearchMode.NameOrId);

            if (unbans.Count == 0)
            {
                if (pData == null)
                {
                    await actor.PrintMessageAsync(m_StringLocalizer["commands:global:playernotfound",
                        new {Input = target}]);
                    return;
                }

                unbans = await m_GlobalBanRepository.UnbanAutoFindAsync(pData.Id.ToString(), BanSearchMode.Id);

                if (unbans.Count == 0)
                {
                    await actor.PrintMessageAsync(m_StringLocalizer["commands:unban:no_bans", new {Target = target}]);
                    return;
                }
            }

            var playerId = unbans.First().PlayerId;
            if (unbans.Any(x => x.PlayerId != playerId))
            {
                await actor.PrintMessageAsync(m_StringLocalizer["commands:unban:unbanned",
                    new {Target = target, BanCount = unbans.Count}]);
                return;
            }

            var data = await m_PlayerInfoRepository.FindPlayerAsync(playerId.ToString(), UserSearchMode.Id);
            var charName = data?.CharacterName ?? playerId.ToString();
            var translated =
                m_StringLocalizer["commands:unban:unbanned", new {Target = charName, BanCount = unbans.Count}];

            await m_UserManager.BroadcastAsync(translated);
            await actor.PrintMessageAsync(translated);
            await m_Plugin.Instance?.SendWebhookAsync(WebhookType.Unban, charName, actor.DisplayName, "",
                playerId.ToString(), 0);
        }
    }
}