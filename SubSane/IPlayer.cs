using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SubsonicAPI;

namespace SubSane
{
    public interface IPlayer
    {
        void Play();
        void Pause();
        void Stop();
        void Skip();

        List<Song> GetQueue();
    }
}
