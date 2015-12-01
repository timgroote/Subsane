using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using SubsonicAPI;
using Un4seen.Bass;

namespace SubSane
{    
    public class BassNetPlayer : IPlayer
    {
        private int BassNetChannel;

        private int CurrentSongChannel;

        public class SongFinishedEventArgs : EventArgs
        {
            
        }

        public event EventHandler<SongFinishedEventArgs> SongFinished;

        private void onSongfinished()
        {
            EventHandler<SongFinishedEventArgs> handler = SongFinished;
            if (handler != null)
            {
                handler(this, new SongFinishedEventArgs());
            }
        }

        public PlayState State { get; private set; }
        public Song CurrentSong { get; private set; }
        private Thread playThread;

        GCHandle _hgcFile;

        public Queue<Song> PlayList { get; private set; }

        public BassNetPlayer()
        {
            PlayList = new Queue<Song>();
            Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero);
        }

        public void AddSong(Song song)
        {
            if (song == null) return; //do not accept null.

            PlayList.Enqueue(song);
        }

        public int ProgressPercentage
        {
            get
            {
                if (State == PlayState.Playing)
                {
                    float percentage = ((float)Bass.BASS_ChannelGetPosition(CurrentSongChannel) / (float)Bass.BASS_ChannelGetLength(CurrentSongChannel)) * 100f;

                    return (Int32) Math.Round(percentage);
                }
                return 0;
            }
        }

        public void Play()
        {
            if (State == PlayState.Playing)
            {
                Stop();
            }

            if (playThread == null)
            {
                  playThread = new Thread(new ThreadStart(ProgressThread));
            }

            if (State == PlayState.Paused)
            {
                Pause();
            }
            else if (PlayList.Count > 0)
            {
                State = PlayState.Playing;
                if (CurrentSong == null)
                {
                    CurrentSong = PlayList.Dequeue();
                }
            }
            else
            {
                //nothing to play
                Stop();
            }
            
            if (this.State == PlayState.Playing)
            {
                if (CurrentSong == null)
                {
                    CurrentSong = PlayList.Peek();
                }

                
                State = PlayState.Playing;
                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.Out.WriteLine("downloading...");
                byte[] songBytes = Subsonic.PreloadSong(CurrentSong.Id);
                _hgcFile = GCHandle.Alloc(songBytes, GCHandleType.Pinned);
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.Out.WriteLine("download complete. playing");
                Console.ForegroundColor = ConsoleColor.White;
                CurrentSongChannel = Bass.BASS_StreamCreateFile(_hgcFile.AddrOfPinnedObject(), 0, songBytes.Length, BASSFlag.BASS_SAMPLE_FLOAT);
                Bass.BASS_ChannelPlay(CurrentSongChannel, false);
                Console.ResetColor();
                if (!playThread.IsAlive)
                {
                    if (playThread.ThreadState == ThreadState.Running)
                    {
                        playThread.Join();
                    }
                    playThread = new Thread(new ThreadStart(ProgressThread));
                    playThread.Start();
                }
            }

        }

        public void Pause()
        {
            if (State == PlayState.Playing)
            {
                Bass.BASS_ChannelPause(CurrentSongChannel);
                State = PlayState.Paused;
            }
            else if (State == PlayState.Paused)
            {
                Bass.BASS_ChannelPlay(CurrentSongChannel, false);
                State = PlayState.Playing;
            }

        }
                    
        public void Stop()
        {
            Bass.BASS_ChannelStop(CurrentSongChannel);

            if(_hgcFile.IsAllocated)
                _hgcFile.Free();

            State = PlayState.Stopped;
        }  
                                         
        private void ProgressThread()
        {                                                     
            while (Bass.BASS_ChannelIsActive(CurrentSongChannel) == BASSActive.BASS_ACTIVE_PLAYING || Bass.BASS_ChannelIsActive(CurrentSongChannel) == BASSActive.BASS_ACTIVE_PAUSED)
            {
                int progress = ProgressPercentage;
                
                Thread.Sleep(1000);
                if (progress >= 100)
                {
                    Skip();
                }
            }
        }

        public void Skip()
        {
            if (PlayList.Count == 0)
            {
                Stop(); //no more songs
                return;
            }

            Stop();
            onSongfinished();
            CurrentSong = PlayList.Dequeue();
            Play();
        }

        public List<Song> GetQueue()
        {
            return PlayList.ToList();
        }
    }
}
