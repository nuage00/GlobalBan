using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rocket.API.Collections;
using Rocket.Core.Plugins;
using Rocket.Unturned;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using UnityEngine;
using UnityEngine.Serialization;

namespace fr34kyn01535.GlobalBan
{
    public class GlobalBan : RocketPlugin<GlobalBanConfiguration>
    {
        public static GlobalBan Instance;
        [FormerlySerializedAs("Database")] public DatabaseManager database;

        public static Dictionary<CSteamID, string> Players = new Dictionary<CSteamID, string>();

        protected override void Load()
        {
            Instance = this;
            database = new DatabaseManager();
            Provider.onCheckValid += Events_OnJoinRequested;
            U.Events.OnPlayerConnected += RocketServerEvents_OnPlayerConnected;
        }

        protected override void Unload()
        {
            Provider.onCheckValid -= Events_OnJoinRequested;
            U.Events.OnPlayerConnected -= RocketServerEvents_OnPlayerConnected;
        }

        public override TranslationList DefaultTranslations =>
            new TranslationList
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
                {"command_kick_private_default_reason", "you were kicked from the server"}
            };

        public static KeyValuePair<CSteamID, string> GetPlayer(string search)
        {
            foreach (var pair in Players.Where(pair => pair.Key.ToString().ToLower().Contains(search.ToLower()) ||
                                                       pair.Value.ToLower().Contains(search.ToLower())))
                return pair;

            return new KeyValuePair<CSteamID, string>(new CSteamID(0), null);
        }

        private void RocketServerEvents_OnPlayerConnected(UnturnedPlayer player)
        {
            if (!Players.ContainsKey(player.CSteamID))
                Players.Add(player.CSteamID, player.CharacterName);

            if (!Configuration.Instance.KickInsteadReject) return;

            var ban = database.GetBan(player.Id);
            if (ban != null && (ban.Duration == -1 || ban.Time.AddSeconds(ban.Duration) > DateTime.Now))
                StartCoroutine(KickPlayer(player, ban));
        }

        private IEnumerator KickPlayer(UnturnedPlayer player, DatabaseManager.Ban ban)
        {
            yield return new WaitForSeconds(Instance.Configuration.Instance.KickInterval);

            player.Kick(Translate("default_banmessage", ban.Admin, ban.Time.ToString(),
                ban.Duration == -1 ? "" : ban.Duration.ToString()));
        }

        public static bool TryConvertTimeToSeconds(string time, out int duration)
        {
            duration = 0;
            if (string.IsNullOrEmpty(time)) return false;

            var splitVar = time.Split('w');
            if (splitVar.Length >= 2)
                if (int.TryParse(splitVar[0], out var tm))
                    duration += tm * 604800;
            splitVar = splitVar[splitVar.Length >= 2 ? 1 : 0].Split('d');
            if (splitVar.Length >= 2)
                if (int.TryParse(splitVar[0], out var tm))
                    duration += tm * 86400;
            splitVar = splitVar[splitVar.Length >= 2 ? 1 : 0].Split('h');
            if (splitVar.Length >= 2)
                if (int.TryParse(splitVar[0], out var tm))
                    duration += tm * 3600;
            splitVar = splitVar[splitVar.Length >= 2 ? 1 : 0].Split('m');
            if (splitVar.Length >= 2)
                if (int.TryParse(splitVar[0], out var tm))
                    duration += tm * 60;
            splitVar = splitVar[splitVar.Length >= 2 ? 1 : 0].Split('s');
            if (splitVar.Length >= 2)
                if (int.TryParse(splitVar[0], out var tm))
                    duration += tm;
            if (int.TryParse(splitVar[splitVar.Length >= 2 ? 1 : 0], out var tm1))
                duration += tm1;
            return duration > 0;
        }

        private void Events_OnJoinRequested(ValidateAuthTicketResponse_t callback, ref bool isValid)
        {
            if (Configuration.Instance.KickInsteadReject) return;

            var ban = database.GetBan(callback.m_SteamID.ToString());

            if (ban == null) return;

            isValid = false;
            var time = ban.Duration == -1
                ? uint.MaxValue
                : (uint) ban.Time.AddSeconds(ban.Duration).Subtract(DateTime.Now).TotalSeconds;
            var bytes = new List<byte> {9, (byte) ban.Reason.Length};
            bytes.AddRange(Encoding.UTF8.GetBytes(ban.Reason));
            bytes.AddRange(BitConverter.GetBytes(time));
            Provider.send(callback.m_SteamID, ESteamPacket.BANNED, bytes.ToArray(), bytes.Count, 0);
        }
    }
}