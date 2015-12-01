using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SubsonicAPI;
using SubSane.ConsoleForms;
using Un4seen.Bass;
using Utils = SubSane.ConsoleForms.Utils;

namespace SubSane
{
    class Program
    {
        public static PlayMode mode = PlayMode.Dumb;

        private static String server;
        private static String user;
        private static String pw;

        private static Random ran = new Random();

        private static Dictionary<string, string> Artists;

        private static Dictionary<MusicFolder, List<Song>> SongsByAlbum { get; set; }

        private static BassNetPlayer _thePlayer;
        private static BassNetPlayer ThePlayer
        {
            get { return _thePlayer ?? (ThePlayer = new BassNetPlayer()); }
            set
            {
                _thePlayer = value;
            }
        }


        static void Main(string[] args)
        {
            Subsonic.appName = "SubSane";
            
            string filename = "SubsonicRegfile.nfo";
            if (File.Exists(filename))
            {
                String fileContents = File.ReadAllText(filename);
                string[] tokens = fileContents.Split(' ');
                BassNet.Registration(tokens[0], tokens[1]);
            }
            

            if (args.Length != 3)
            {
                DisplayLoginForm();
            }
            else
            {
                string svr = args[0];
                string una = args[1];
                string pw = args[2];

                LoginResult loginResult = HandleLogin(svr, una, pw);
                if (!loginResult.Success)
                {
                    Console.Out.WriteLine("Could not log in. cehck connection, host/username/password and try again");
                    Console.ReadLine();
                    return;
                }
                else
                {
                    Console.Out.WriteLine("Login OK! Welcome to {0}, {1}!", server, user);
                    EnterMainLoop();
                }
            }

        }

        private static void DisplayLoginForm()
        {
            ConsoleForm loginForm = new ConsoleForm(Console.WindowWidth, Console.WindowHeight) { Name = "Login form" };
            Label lblTitle = new Label("lblTitle",
                new Point(1, 2),
                10,
                "Login",
                ConsoleColor.Green,
                ConsoleColor.Black);

            Label lblserver = new Label("lblserver",
                new Point(1, 10),
                28,
                "server",
                ConsoleColor.Green,
                ConsoleColor.Black);

            Label lblUsername = new Label("lbluname",
                new Point(1, 12),
                10,
                "Username",
                ConsoleColor.Green,
                ConsoleColor.Black);

            Label lblPass = new Label("lblpass",
                new Point(1, 14),
                10,
                "Password",
                ConsoleColor.Green,
                ConsoleColor.Black);


            Textbox txtServer = new Textbox("txtServer",
                new Point(12, 10),
                30,
                string.Empty,
                ConsoleColor.White,
                ConsoleColor.DarkGray);

            Textbox txtUser = new Textbox("txtUser",
                new Point(12, 12),
                30,
                string.Empty,
                ConsoleColor.White,
                ConsoleColor.DarkGray);

            Textbox txtPassword = new Textbox("txtPass",
                new Point(12, 14),
                30,
                string.Empty,
                ConsoleColor.White,
                ConsoleColor.DarkGray)
            {
                PasswordChar = '*'
            };


            loginForm.Add(lblTitle);

            loginForm.Add(lblTitle);
            loginForm.Add(lblserver);
            loginForm.Add(lblUsername);
            loginForm.Add(lblPass);

            loginForm.Add(txtServer);
            loginForm.Add(txtUser);
            loginForm.Add(txtPassword);

            loginForm.FormCancelled += LoginForm_FormCancelled;
            loginForm.FormComplete += LoginForm_FormComplete;

            loginForm.Render(true);
        }

        private static void LoginForm_FormCancelled(ConsoleForm sender, EventArgs e)
        {
            Console.ResetColor();
            Console.Clear();
            Utils.UOut(ConsoleColor.Yellow, "K, bye!");
        }

        private static void EnterMainLoop()
        {
            GetArtists();

            string Argument = "";
            bool exitProgram = false;

            ThePlayer.SongFinished += HandleSongFinished;

            while (exitProgram == false)
            {
                Argument = Console.ReadLine();

                switch (Argument)
                {
                    case "play":
                        if (ThePlayer.PlayList.Count > 0)
                        {
                            Utils.UOut(ConsoleColor.Black, "Now playing : {0}", ConsoleColor.White, ThePlayer.PlayList.Peek());
                            Play();
                        }
                        else
                        {
                            Utils.UOut(ConsoleColor.Red, "Nothing in playlist... enter '?' for command list", ConsoleColor.White);
                        }
                        break;
                    case "skip":
                        Skip();
                        break;
                    case "stop":
                        Stop();
                        break;
                    case "pause":
                        Pause();
                        break;

                    case "random":

                        Song s = GetRandomSong();
                        if (s != null && s.Name != null && s.Id != null)
                        {
                            try
                            {
                                ThePlayer.AddSong(GetRandomSong());
                            }
                            catch (Exception e)
                            {
                                Utils.UOut(ConsoleColor.Red, "cannot read song properties. \r\n{1}\r\n{2}", ConsoleColor.White, e.Message, e.StackTrace);
                            }

                        }
                        else
                        {
                            Utils.UOut(ConsoleColor.Red, "Error getting random song. nothing enqueued.");
                        }
                        break;


                    case "queue":
                        foreach (var queueditem in ThePlayer.PlayList)
                        {
                            if (queueditem != null)
                            {
                                Utils.UOut(ConsoleColor.Yellow, queueditem.ToString());
                            }
                            else
                            {
                                Utils.UOut(ConsoleColor.Red, "NULL <-- this should not happen. fix please");
                            }
                        }
                        break;

                    case "whatsplaying":
                        foreach (KeyValuePair<String, Song> p in WhatsPlaying())
                        {
                            Utils.UOut(ConsoleColor.Yellow, "{0} => {1} {2}", ConsoleColor.Black, p.Key, p.Value.Name, p.Value.Id);
                        }
                        break;

                    case "listalbums":
                        Utils.UOut(ConsoleColor.Yellow, "Enter artist");
                        string artistName = Console.ReadLine();
                        artistName = artistName.ToLowerInvariant();
                        Dictionary<String, string> matchingArtists = new Dictionary<string, string>();

                        Dictionary<string, string> matchingalbums = new Dictionary<string, string>();
                        foreach (KeyValuePair<string, string> artist in Artists)
                        {
                            if (artist.Key.ToLowerInvariant().Contains(artistName))
                            {
                                matchingArtists.Add(artist.Key, artist.Value);
                                matchingalbums = ListAlbums(artist.Value);
                                foreach (var album in matchingalbums)
                                {
                                    Utils.UOut(ConsoleColor.Yellow, "({0}) {1} - {2}", ConsoleColor.Black, artist.Key, album.Key, album.Value);
                                }
                            }
                        }
                        break;

                    case "listsongs":
                        string albumId = Console.ReadLine();
                        foreach (Song songadong in ListSongsByAlbumId(albumId))
                        {
                            Utils.UOut(ConsoleColor.Yellow, "{0} ({1})", ConsoleColor.Black, songadong.Name, songadong.Id);
                        }
                        break;

                    case "info":
                        Utils.UOut(ConsoleColor.Black, "Now playing {0}", ConsoleColor.White, ThePlayer.CurrentSong);
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Out.Write("[");
                        Console.BackgroundColor = ConsoleColor.DarkBlue;

                        int numberOfBlocks = (int)Math.Floor((double)(ThePlayer.ProgressPercentage / 2));
                        for (int i = 0; i < 50; i++)
                        {
                            if (i < numberOfBlocks)
                            {
                                Console.Out.Write("@");
                            }
                            else
                            {
                                Console.ResetColor();
                                Console.Out.Write("-");
                            }
                        }
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Out.Write("]");
                        Console.ResetColor();
                        Console.Out.WriteLine();
                        break;


                    case "addid":
                        Utils.UOut(ConsoleColor.Green, "enter id and press [enter].");
                        string id = Console.ReadLine();
                        Song sg = GetSongById(id);
                        if (sg?.Name == null || sg.Id == null)
                        {
                            Utils.UOut(ConsoleColor.Red, "no song found.");
                        }
                        else
                        {
                            ThePlayer.AddSong(sg);
                            Utils.UOut(ConsoleColor.Yellow, "Enqueued {0}", ConsoleColor.Black, sg.Name);
                        }
                        break;

                    case "partymode":
                        mode = PlayMode.Party;
                        while (ThePlayer.PlayList.Count < 10)
                        {
                            Song rsong = GetRandomSong();
                            if (rsong == null)
                            {
                                Utils.UOut(ConsoleColor.Red, "attempted to add a null song. song not enqueued");
                                continue;
                            }

                            if (rsong?.Name != null && rsong.Id != null)
                            {
                                ThePlayer.PlayList.Enqueue(rsong);
                            }

                        }
                        break;

                    case "dumbmode":
                        mode = PlayMode.Dumb;
                        break;

                    case "?":
                        Console.Out.WriteLine(@"exit
	quit subsane

play
	start / resume playback
	
pause
	pause / resume playback
	
stop
	stop playback
	
skip
	play next song
	
queue
	display playlist
	
random
	enqueue a random song
	
whatsplaying
	show for each subsonic user what they are currently playing
	
listalbums
	allows you to enter a wildcard artist name,
	then shows all  matching albums and ids 
	
listsongs
	allows you to enter an album ids
	lists all songs and their ids for that album

addid
	allows you to enter a song ids
	enqueues song

info
    shows current playback info

partymode
    enter party mode! never run out of playlist! keeps 10 randomly picked songs in your playlist at all times

dumbmode
    dumb mode : stop after playlist gets empty.

anco
    Tries to look for 'up to my neck in you' by AC/DC and enqueues it if found

?
	show this list");
                        break;

                    case "exit":
                        Utils.UOut(ConsoleColor.Green, "ok, bye :)");
                        exitProgram = true;
                        break;

                    default:
                        Utils.UOut(ConsoleColor.Red, "not recognised. enter '?' for help.");
                        break;
                }
            }

            ThePlayer.Stop();
        }

        private static void HandleSongFinished(object sender, BassNetPlayer.SongFinishedEventArgs e)
        {
            if (mode == PlayMode.Party)
            {
                while (ThePlayer.PlayList.Count < 10)
                {
                    Song randomSong = GetRandomSong();
                    if (randomSong == null || randomSong.Id == null || randomSong.Name == null)
                    {
                        Utils.UOut(ConsoleColor.Red,"attempted to add a null song. song not added");
                        continue;
                    }
                    ThePlayer.PlayList.Enqueue(randomSong);
                }
            }
            if (ThePlayer.PlayList.Count > 0)
            {
                Utils.UOut(ConsoleColor.Black, "Now playing : {0}", ConsoleColor.White, ThePlayer.PlayList.Peek().Name);
            }
        }

        



        private static IEnumerable<Song> ListSongsByAlbumId(string albumId)
        {
            MusicFolder albumContens = Subsonic.GetMusicDirectory(albumId);
            return albumContens.Songs;
        }

        private static void LoginForm_FormComplete(ConsoleForm sender, FormCompleteEventArgs e)
        {
            if (e.Cancel || sender.Textboxes["txtServer"].Text == null)
            {
                Utils.UOut(ConsoleColor.Yellow, "K, bye!");
                return;
            }

            LoginResult loginResult = HandleLogin(sender.Textboxes["txtServer"].Text, sender.Textboxes["txtUser"].Text, sender.Textboxes["txtPass"].Text);
            if (loginResult.Success == false)
            {
                DisplayLoginForm();
            }
            else
            {
                Console.ResetColor();
                Console.Clear();
                EnterMainLoop();

            }
        }

        private static Dictionary<string, Song> WhatsPlaying()
        {
            return Subsonic.GetNowPlaying();
        }

        private static LoginResult HandleLogin(string host, string username, string password)
        {
            try
            {
                server = host;
                user = username;
                pw = password;

                return new LoginResult(Subsonic.LogIn(server, user, password));
            }
            catch (Exception e)
            {
                return new LoginResult("FAIL");
            }

        }

        private static Song GetSongById(string id)
        {
            return Subsonic.GetSong(id);
        }

        private static Song GetRandomSong()
        {
            var artistKey = Artists.Values.OrderBy(r => ran.Next()).FirstOrDefault();
            var albumKey = GetAlbumIds(artistKey);
            var songs = GetSongIds(albumKey.OrderBy(rd => ran.Next()).FirstOrDefault());
            return songs.OrderBy(r => ran.Next()).FirstOrDefault(sng => sng != null);
        }

        private static IEnumerable<Song> GetSongIds(string albumKey)
        {
            MusicFolder albumContens = Subsonic.GetMusicDirectory(albumKey);
            return albumContens.Songs;
        }

        private static void GetArtists()
        {
            Utils.UOut(ConsoleColor.Yellow, "Loading (artists)");
            Artists = Subsonic.GetIndexes();
            Utils.UOut(ConsoleColor.Yellow, "Done Loading (artists)");
        }


        private static Dictionary<string, string> ListAlbums(string artistId)
        {
            return Subsonic.GetMusicDirectory(artistId).Folders.Where(f => f.ItemType == SubsonicItemType.Folder).Select(fd => new KeyValuePair<string, string>(fd.Name, fd.Id)).ToDictionary(r => r.Key, r => r.Value);
        }


        private static IEnumerable<String> GetAlbumIds(string artistId)
        {
            MusicFolder folderContents = Subsonic.GetMusicDirectory(artistId);
            return folderContents.Folders.Select(mf => mf.Id);
        }


        private static void Pause()
        {
            ThePlayer.Pause();
        }

        private static void Skip()
        {
            ThePlayer.Skip();
        }

        private static void Stop()
        {
            ThePlayer.Stop();
        }

        private static void Play()
        {
            ThePlayer.Play();
        }


        private static string GetFirstLetter(string name)
        {
            name = name.ToUpper();
            string theFirstLetter = "";
            if (name.StartsWith("THE "))
                theFirstLetter = name.Substring(4, 1);
            else if (name.StartsWith("LOS "))
                theFirstLetter = name.Substring(4, 1);
            else if (name.StartsWith("LAS "))
                theFirstLetter = name.Substring(4, 1);
            else if (name.StartsWith("LES "))
                theFirstLetter = name.Substring(4, 1);
            else if (name.StartsWith("LA "))
                theFirstLetter = name.Substring(3, 1);
            else
                theFirstLetter = name.Substring(0, 1); // No leading word

            return theFirstLetter.ToUpper();
        }
    }
}
