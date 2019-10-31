using Rocket.API;
using Rocket.API.Collections;
using Rocket.Core.Commands;
using Rocket.Core.Logging;
using Rocket.Core.Plugins;
using Rocket.Unturned;
using Rocket.Unturned.Events;
using Rocket.Unturned.Permissions;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace fr34kyn01535.GlobalBan
{
    public class GlobalBan : RocketPlugin<GlobalBanConfiguration>
    {
        public static GlobalBan Instance;
        public DatabaseManager Database;

        public static Dictionary<CSteamID, string> Players = new Dictionary<CSteamID, string>();

        protected override void Load()
        {
            Instance = this;
            Database = new DatabaseManager();
            UnturnedPermissions.OnJoinRequested += Events_OnJoinRequested;
            U.Events.OnPlayerConnected += RocketServerEvents_OnPlayerConnected;
        }

        protected override void Unload()
        {
            UnturnedPermissions.OnJoinRequested -= Events_OnJoinRequested;
            U.Events.OnPlayerConnected -= RocketServerEvents_OnPlayerConnected;
        }

        public override TranslationList DefaultTranslations =>
            new TranslationList()
            {
                {
                    "default_banmessage",
                    "you were banned by {0} on {1} for {2} seconds, contact the staff if you feel this is a mistake."
                },
                {"command_generic_invalid_parameter", "Invalid parameter"},
                {"command_generic_player_not_found", "Player not found"},
                {"command_ban_public_reason", "The player {0} was banned for: {1}"},
                {"command_ban_public", "The player {0} was banned"},
                {"command_ban_private_default_reason", "you were banned from the server"},
                {"command_kick_public_reason", "The player {0} was kicked for: {1}"},
                {"command_kick_public", "The player {0} was kicked"},
                {"command_kick_private_default_reason", "you were kicked from the server"},
            };

        public static KeyValuePair<CSteamID, string> GetPlayer(string search)
        {
            foreach (var pair in Players)
                if (pair.Key.ToString().ToLower().Contains(search.ToLower()) ||
                    pair.Value.ToLower().Contains(search.ToLower()))
                    return pair;
            return new KeyValuePair<CSteamID, string>(new CSteamID(0), null);
        }

        private void RocketServerEvents_OnPlayerConnected(UnturnedPlayer player)
        {
            if (!Players.ContainsKey(player.CSteamID))
                Players.Add(player.CSteamID, player.CharacterName);

            if (Configuration.Instance.KickInsteadReject)
            {
                var ban = Database.GetBan(player.Id);
                if (ban != null && (ban.Duration == -1 || ban.Time.AddSeconds(ban.Duration) > DateTime.Now))
                    StartCoroutine(KickPlayer(player, ban));
            }
        }

        private IEnumerator KickPlayer(UnturnedPlayer player, DatabaseManager.Ban ban)
        {
            yield return new WaitForSeconds(Instance.Configuration.Instance.KickInterval);
            player.Kick(Translate("default_banmessage", ban.Admin, ban.Time.ToString(),
                ban.Duration == -1 ? "" : ban.Duration.ToString()));
        }

        public void Events_OnJoinRequested(CSteamID player, ref ESteamRejection? rejection)
        {
            try
            {
                if (!Configuration.Instance.KickInsteadReject && Database.IsBanned(player.ToString()))
                    rejection = ESteamRejection.AUTH_PUB_BAN;
            }
            catch (Exception)
            {
            }
        }
    }
}