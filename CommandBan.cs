using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SDG;
using UnityEngine;
using System.Web.Script.Serialization;
using Rocket.RocketAPI;

namespace unturned.ROCKS.GlobalBan
{
    class CommandBan : Command
    {
        public CommandBan()
        {
            base.commandName = "ban";
            base.commandHelp = "Banns a player";
            base.commandInfo = commandName + " - " + commandHelp;
        }

        protected override void execute(SteamPlayerID caller, string command)
        {
            SteamPlayerID steamPlayerID = null; ;
            string[] componentsFromSerial = Parser.getComponentsFromSerial(command, '/');
            if (componentsFromSerial.Length == 0 ||componentsFromSerial.Length > 2)
            {
                RocketChatManager.Say(caller.CSteamID,"Invalid parameter");
                return;
            }
            if (!SteamPlayerlist.tryGetPlayer(componentsFromSerial[0], out steamPlayerID))
            {
                RocketChatManager.Say(caller.CSteamID, "Player not found");
                return;
            }
            if ((int)componentsFromSerial.Length == 1)
            {
                Database.BanPlayer(steamPlayerID.CSteamID.ToString(),caller.CSteamID.ToString(),"");
                RocketChatManager.Say("The player " + steamPlayerID.SteamName + " was banned");
            }
            else
            {
                Database.BanPlayer(steamPlayerID.CSteamID.ToString(), caller.CSteamID.ToString(), componentsFromSerial[1]);
                RocketChatManager.Say("The player " + steamPlayerID.SteamName + " was banned for: " + componentsFromSerial[2]);
            }
        }
    }
}
