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
            RocketChat.Say("test");
            Logger.Log(command);
            SteamPlayerID steamPlayerID = null; ;
            string[] componentsFromSerial = Parser.getComponentsFromSerial(command, '/');
            if (componentsFromSerial.Length == 0 ||componentsFromSerial.Length > 2)
            {
                Logger.Log("1");
                RocketChat.Say(caller.CSteamID,this.Local.format("InvalidParameterErrorText"));
                return;
            }
            if (!SteamPlayerlist.tryGetPlayer(componentsFromSerial[0], out steamPlayerID))
            {
                Logger.Log("2");
                RocketChat.Say(caller.CSteamID, this.Local.format("NoPlayerErrorText", new object[] { componentsFromSerial[0] }));
                return;
            }
            if ((int)componentsFromSerial.Length == 1)
            {
                Logger.Log("3");
                Database.BanPlayer(steamPlayerID.CSteamID.ToString(),caller.CSteamID.ToString(),"");
                RocketChat.Say(this.Local.format("BanTextPermanent", new object[] { steamPlayerID.SteamName }));
            }
            else
            {
                Database.BanPlayer(steamPlayerID.CSteamID.ToString(),caller.CSteamID.ToString(),componentsFromSerial[1]);
                RocketChat.Say(this.Local.format("BanTextPermanent", new object[] { steamPlayerID.SteamName }));
            }
        }
    }
}
