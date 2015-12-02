using System;
using System.Collections.Generic;
using System.Linq;
using SubsonicAPI;
using SubSane.Players;

namespace SubSane.CliCommands
{
    public class Partymode : CliCommandType
    {
        private static Random r;

        private static Random randomLazy
        {
            get
            {
                if (r == null)
                {
                    r= new Random();
                }
                return r;
            }
        }

        public override string Description
        {
            get { return "enqueues random songs (maintains a list of at least 10 songs)"; }
        }

        public override string[] CallStrings
        {
            get { return Aliases; }
        }

        public static readonly string[] Aliases = {"partymode"};

        public override void Execute(IPlayer player, params string[] parameters)
        {
            Program.mode = PlayMode.Party;

            while (player.GetQueue().Count < 10)
            {
                player.AddSong(GetRandomSong());
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