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
    public class ListSongs : CliCommandType
    {
        public ListSongs(string input) : base(input)
        {
        }

        public static readonly string[] Aliases = { "listsongs", "findsongs", "searchsongs" };

        public override string Description {
            get { return "Lists the ListSongs of a specified album by id. (/listsongs 40960) (/listsongs 211"; }
        }

        public override string[] CallStrings {
            get
            {
                return Aliases;
            }
        }
        public override void Execute(IPlayer player, params string[] parameters)
        {
            if (parameters.Length == 1)
            {
                ConsoleUtils.UOut(ConsoleColor.Red, "You didn't enter an album id. use like this : (/listsongs 211)");
                return;
            }
            string albumid = parameters[1];

            foreach (Song songadong in Subsonic.ListSongsByAlbumId(albumid))
            {
                ConsoleUtils.UOut(ConsoleColor.Yellow, "{0} ({1})", ConsoleColor.Black, songadong.Name, songadong.Id);
            }
        }
    }
}
