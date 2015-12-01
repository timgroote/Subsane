using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using SubsonicAPI;
using Un4seen.Bass;

namespace SubSane.Players
{    
    public class BassNetPlayer : IPlayer
    {
        private int _currentSongChannel;

        public class SongFinishedEventArgs : EventArgs
        {
            
        }

        public event EventHandler<SongFinishedEventArgs> SongFinished;

        private void OnSongfinished()
        {
            EventHandler<SongFinishedEventArgs> handler = SongFinished;
            if (handler != null)
            {
                handler(this, new SongFinishedEventArgs());
            }
        }

        public PlayState State { get; private set; }
        
        private Thread _playThread;

        GCHandle _hgcFile;

        private Queue<Song> PlayedSongs { get; set; }
        public Song CurrentSong { get; private set; }
        public Queue<Song> PlayQueue { get; private set; }

        public BassNetPlayer()
        {
            PlayQueue = new Queue<Song>();
            Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero);
        }

        public void AddSong(Song song)
        {
            if (song == null) return; //do not accept null.

            PlayQueue.Enqueue(song);
        }

        public Song GetCurrentSong()
        {
            return CurrentSong;
        }

        
        public int ProgressPercentage
        {
            get
            {
                if (State == PlayState.Playing)
                {
                    float percentage = ((float)Bass.BASS_ChannelGetPosition(_currentSongChannel) / (float)Bass.BASS_ChannelGetLength(_currentSongChannel)) * 100f;

                    return (int) Math.Round(percentage);
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

            if (_playThread == null)
            {
                  _playThread = new Thread(new ThreadStart(ProgressThread));
            }

            if (State == PlayState.Paused)
            {
                Pause();
            }
            else if (PlayQueue.Count > 0)
            {
                State = PlayState.Playing;
                if (CurrentSong == null)
                {
                    CurrentSong = PlayQueue.Dequeue();
                    PlayedSongs.Enqueue(CurrentSong);
                }
            }
            else
            {
                //nothing to play
                Stop();
            }
            
            if (State == PlayState.Playing)
            {
                if (CurrentSong == null)
                {
                    CurrentSong = PlayQueue.Peek();
                }
                if (!CurrentSong.IsPlayable || (!Config.Instance.PlayWavs && CurrentSong.FileType.ToLowerInvariant() == "wav"))
                {
                    Console.ResetColor();
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.Out.WriteLine("cannot play song. skipping.");
                    Console.ResetColor();
                    Skip();
                    return;
                }
                
                State = PlayState.Playing;
                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.Out.WriteLine("downloading...");
                //todo : this should happen in a background thread.
                byte[] songBytes = Subsonic.PreloadSong(CurrentSong.Id);
                _hgcFile = GCHandle.Alloc(songBytes, GCHandleType.Pinned);
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.Out.WriteLine("download complete. playing");
                Console.ForegroundColor = ConsoleColor.White;
                _currentSongChannel = Bass.BASS_StreamCreateFile(_hgcFile.AddrOfPinnedObject(), 0, songBytes.Length, BASSFlag.BASS_SAMPLE_FLOAT);
                Bass.BASS_ChannelPlay(_currentSongChannel, false);
                Console.ResetColor();
                if (!_playThread.IsAlive)
                {
                    if (_playThread.ThreadState == ThreadState.Running)
                    {
                        _playThread.Join();
                    }
                    _playThread = new Thread(new ThreadStart(ProgressThread));
                    _playThread.Start();
                }
            }

        }

        public void Pause()
        {
            if (State == PlayState.Playing)
            {
                Bass.BASS_ChannelPause(_currentSongChannel);
                State = PlayState.Paused;
            }
            else if (State == PlayState.Paused)
            {
                Bass.BASS_ChannelPlay(_currentSongChannel, false);
                State = PlayState.Playing;
            }

        }
                    
        public void Stop()
        {
            Bass.BASS_ChannelStop(_currentSongChannel);

            if(_hgcFile.IsAllocated)
                _hgcFile.Free();

            State = PlayState.Stopped;
        }  
                                         
        private void ProgressThread()
        {
            int progress = 0;

            while (Bass.BASS_ChannelIsActive(_currentSongChannel) == BASSActive.BASS_ACTIVE_PLAYING || Bass.BASS_ChannelIsActive(_currentSongChannel) == BASSActive.BASS_ACTIVE_PAUSED || State == PlayState.Playing)
            {
                progress = ProgressPercentage;   
                Thread.Sleep(1000);
                if (progress >= 100)
                {
                    Skip();
                }
            }
        }

        public void Skip()
        {
            if (PlayQueue.Count == 0)
            {
                Stop(); //no more songs
                return;
            }

            Stop();
            OnSongfinished();
            CurrentSong = PlayQueue.Dequeue();
            PlayedSongs.Enqueue(CurrentSong);
            Play();
        }

        public List<Song> GetPlayedSongs()
        {
            return PlayedSongs.ToList();
        } 

        public List<Song> GetQueue()
        {
            return PlayQueue.ToList();
        }
    }
}
