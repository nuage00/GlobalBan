using System;

namespace fr34kyn01535.GlobalBan.API
{
    public class PlayerBan
    {
        public ulong BanEntryId;
        public ulong SteamId;
        public string Hwid;
        public uint Ip;
        public ulong Admin;
        public string Reason;
        public uint Duration;
        public ushort ServerId;
        public DateTime TimeOfBan;
        public bool Unbanned;

        public PlayerBan()
        {
        }

        public PlayerBan(ulong entryId, ulong steamId, string hwid, uint ip, uint duration, DateTime banTime, ulong admin, string reason, ushort serverId, bool unban)
        {
            BanEntryId = entryId;
            SteamId = steamId;
            Hwid = hwid;
            Ip = ip;
            Admin = admin;
            Reason = reason;
            Duration = duration;
            ServerId = serverId;
            TimeOfBan = banTime;
            Unbanned = unban;
        }
    }
}
