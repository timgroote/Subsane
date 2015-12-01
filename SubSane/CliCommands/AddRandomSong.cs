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
    public class AddRandomSong : CliCommandType
    {
        private static Random r = null;

        private static Random randomLazy
        {
            get
            {
                if (r == null)
                {
                    r = new Random();
                }
                return r;
            }
        }

        public static readonly string[] Aliases = { "random", "addrandom", "addshuffle"};

        public AddRandomSong(string input) : base(input)
        {
        }

        public override string Description
        {
            get { return "enqueues a random song"; }
        }
        public override string[] CallStrings { get; }
        public override void Execute(IPlayer player, params string[] parameters)
        {
            Song s = GetRandomSong();
            if (s != null && s.Name != null && s.Id != null)
            {
                try
                {
                    player.AddSong(GetRandomSong());
                }
                catch (Exception e)
                {
                    ConsoleUtils.UOut(ConsoleColor.Red, "cannot read song properties. \r\n{1}\r\n{2}", ConsoleColor.White, e.Message, e.StackTrace);
                }

            }
            else
            {
                ConsoleUtils.UOut(ConsoleColor.Red, "Error getting random song. nothing enqueued.");
            }
        }
        private Song GetRandomSong()
        {
            var artists = Subsonic.GetArtistIndexes();
            var artistKey = artists.Values.OrderBy(r => randomLazy.Next()).FirstOrDefault();
            var albumKey = Subsonic.GetAlbumIds(artistKey);
            var songs = GetSongIds(albumKey.OrderBy(rd => randomLazy.Next()).FirstOrDefault());
            return songs.OrderBy(r => randomLazy.Next()).FirstOrDefault(sng => sng != null);
        }
        private static IEnumerable<Song> GetSongIds(string albumKey)
        {
            MusicFolder albumContens = Subsonic.GetMusicDirectory(albumKey);
            return albumContens.Songs;
        }
    }
}
