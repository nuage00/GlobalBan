using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SDG;
using UnityEngine;
using System.Web.Script.Serialization;

namespace GlobalBan
{
    class CommandBan : Command
    {
        public CommandBan()
        {
            base.commandName = "ban";
            base.commandInfo = base.commandHelp = "Banns a player";
        }

        public override void execute(SteamPlayerID caller, string command)
        {
            string[] commandArray = command.Split(' ');

            if (commandArray.Length < 2)
            {
                ChatManager.say(caller.CSteamId, "Missing arguments");
                return;
            }

            string message = "";
            if (commandArray.Length > 2) {
                for (int i = 2; i < commandArray.Length; i++)
                {
                    if (i != 2) message += " ";
                    message+=commandArray[i];
                }
            }

            SteamPlayer steamPlayer;
            if (SteamPlayerlist.tryGetSteamPlayer(commandArray[1], out steamPlayer))
            {
                Database.BanPlayer(steamPlayer.SteamPlayerId.CSteamId.ToString(),caller.CSteamId.ToString(),message);
                ChatManager.say("Banned " + steamPlayer.SteamPlayerId.IngameName + (message == ""?"":(" for \"" + message+"\"")));
                Steam.kick(steamPlayer.SteamPlayerId.CSteamId,message);
            }
            else
            {
                ChatManager.say(caller.CSteamId, "Failed to find player");
            }
        }

    }
}
