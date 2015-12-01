using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SubsonicAPI;
using SubSane.ConsoleForms;
using SubSane.Players;

namespace SubSane.CliCommands
{
    class PlayList : CliCommandType
    {
        private static readonly string[] Aliases = {
            "playlist",
            "getplaylist",
            "queue",
            "getqueue"
        };

        public override string Description
        {
            get { return "shows the current playlist"; }
        }

        public override string[] CallStrings {
            get
            {
                return Aliases;
            }
        }
        public override void Execute(IPlayer player, params string[] parameters)
        {
            foreach (Song plSong in player.GetQueue())
            {
                if (plSong != null)
                {
                    ConsoleUtils.UOut(ConsoleColor.Yellow, plSong.ToString());
                }
                else
                {
                    ConsoleUtils.UOut(ConsoleColor.Red, "NULL <-- this should not happen. fix please");
                }
            }
        }

        public PlayList(string input) : base(input)
        {
        }
    }
}
