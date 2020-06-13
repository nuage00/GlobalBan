using System.Collections.Generic;
using System.Linq;
using fr34kyn01535.GlobalBan.API;
using JetBrains.Annotations;
using PlayerInfoLibrary;
using Rocket.API;
using Rocket.Unturned.Chat;
using Steamworks;

namespace fr34kyn01535.GlobalBan.Commands
{
    public class CommandBanHistory : IRocketCommand
    {
        [NotNull] public string Help => "Displays the ban history of a player";

        [NotNull] public string Name => "banhistory";

        [NotNull] public string Syntax => "<player>";

        [NotNull] public List<string> Aliases => new List<string>();

        public AllowedCaller AllowedCaller => AllowedCaller.Both;

        [NotNull] public List<string> Permissions => new List<string> {"globalban.banhistory"};

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
            var allBans = await GlobalBan.Instance.database.GetBans(ulong.Parse(target.Id));

            UnturnedChat.Say(caller,
                GlobalBan.Instance.Translate("ban_history", pData?.CharacterName ?? target.DisplayName, allBans.Count,
                    string.Join(";",
                        allBans.OrderByDescending(k => k.TimeOfBan).Take(3).Select(k =>
                            $"Date: {k.TimeOfBan}, Reason: {k.Reason}, For: {k.Duration}, By: {k.Admin}"))));
        }
    }
}