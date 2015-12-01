using System.Collections.Generic;
using SubsonicAPI;

namespace SubSane.Players
{
    public interface IPlayer
    {
        void Play();
        void Pause();
        void Stop();
        void Skip();

        List<Song> GetQueue();
        void AddSong(Song newSong);
        Song GetCurrentSong();
        int ProgressPercentage { get;}
    }
}
