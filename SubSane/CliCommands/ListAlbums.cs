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
    public class ListAlbums : CliCommandType
    {
        public ListAlbums(string input) : base(input)
        {
        }

        public static readonly string[] Aliases = { "listalbums", "findalbums", "searchalbums" };

        public override string Description {
            get { return "Lists the albums of a specified band. (/listalbums rammstein) (/listalbums the cure)"; }
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
                ConsoleUtils.UOut(ConsoleColor.Red, "You didn't enter an artist/band name. use like this : (/listalbums rammstein) (/listalbums the cure)");
                return;
            }
            string artistName = String.Join(" ", parameters.ToList().Skip(1)).ToLowerInvariant();

            foreach (KeyValuePair<string, string> artist in Subsonic.GetArtistIndexes())
            {
                if (artist.Key.ToLowerInvariant().Contains(artistName))
                {
                    foreach (var album in Subsonic.ListAlbums(artist.Value))
                    {
                        ConsoleUtils.UOut(ConsoleColor.Yellow, "({0}) {1} - {2}", ConsoleColor.Black, artist.Key, album.Key, album.Value);
                    }
                }
            }

        }
    }
}
