using System;
using System.Collections.Generic;
using SubsonicAPI;
using SubSane.ConsoleForms;
using SubSane.Players;

namespace SubSane.CliCommands
{
    public class WhatsPlaying : CliCommandType
    {

        public override string Description
        {
            get { return "shows what everyone is playing right now"; }
        }

        public override string[] CallStrings
        {
            get { return Aliases; }
        }

        public static readonly string[] Aliases = {"whatsplaying", "everybody"};

        public override void Execute(IPlayer player, params string[] parameters)
        {
            foreach (KeyValuePair<String, Song> p in Subsonic.GetNowPlaying())
            {
                ConsoleUtils.UOut(ConsoleColor.Yellow, "{0} => {1} {2}", p.Key, p.Value.Name, p.Value.Id);
            }
        }
    }
}