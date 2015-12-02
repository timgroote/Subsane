using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SubsonicAPI;
using SubSane.CliCommands;
using SubSane.ConsoleForms;
using SubSane.Players;
using SubSane.SubSaneConsoleForms;
using Un4seen.Bass;

namespace SubSane
{
    class Program
    {
        public static PlayMode mode = PlayMode.Dumb;

        private static String server;
        private static String user;

        private static BassNetPlayer _thePlayer;
        private static BassNetPlayer ThePlayer
        {
            get { return _thePlayer ?? (ThePlayer = new BassNetPlayer()); }
            set
            {
                _thePlayer = value;
            }
        }

        private static DateTime? LastMsgDate;

        static void Main(string[] args)
        {
            LastMsgDate = DateTime.Now.Add(new TimeSpan(0,-4,0,0)); //retreive messages of the last four hours
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

                Console.Out.WriteLine("Login OK! Welcome to {0}, {1}!", server, user);
                EnterMainLoop();
            }

        }

        private static void DisplayLoginForm()
        {
            ConsoleForm loginForm = new LoginForm(Console.WindowWidth, Console.WindowHeight, "Login form");
            loginForm.FormCancelled += LoginForm_FormCancelled;
            loginForm.FormComplete += LoginForm_FormComplete;
            loginForm.Render(true);
        }

        private static void LoginForm_FormCancelled(ConsoleForm sender, EventArgs e)
        {
            Console.ResetColor();
            Console.Clear();
            ConsoleUtils.UOut(ConsoleColor.Yellow, "K, bye!");
        }

        private static void EnterMainLoop()
        {                                 
            string Argument = "";
            bool exitProgram = false;

            ThePlayer.SongFinished += HandleSongFinished;

            while (!exitProgram)
            {
                string ln = Console.ReadLine();

                if (ln.StartsWith("/"))
                {
                    if (ln.StartsWith("/exit"))
                    {
                        exitProgram = true;
                    }
                    CliCommandFactory.Execute(ThePlayer, ln);
                }
                else
                {
                    if (!string.IsNullOrEmpty(ln))
                    {
                        Subsonic.AddChatMessage(ln);
                    }
                }
                foreach (var msg in Subsonic.GetChatMessages(LastMsgDate).OrderBy(cm => cm.Date))
                {
                    ConsoleUtils.UOut(ConsoleColor.Cyan, msg.ToString());
                }
                LastMsgDate = DateTime.Now;
            }
            ThePlayer.Stop();
        }

        private static void HandleSongFinished(object sender, BassNetPlayer.SongFinishedEventArgs e)
        {
            if (mode == PlayMode.Party)
            {
                while (ThePlayer.PlayQueue.Count < 10)
                {
                    //todo : partymode command is now no longer there
                    CliCommandFactory.Execute(ThePlayer, "/random");     //ewww
                }
            }
            if (ThePlayer.PlayQueue.Count > 0)
            {
                ConsoleUtils.UOut(ConsoleColor.Black, "Now playing : {0}", ConsoleColor.White, ThePlayer.PlayQueue.Peek().Name);
            }
        }
          
        private static void LoginForm_FormComplete(ConsoleForm sender, FormCompleteEventArgs e)
        {
            if (e.Cancel || sender.Textboxes["txtServer"].Text == null)
            {
                ConsoleUtils.UOut(ConsoleColor.Yellow, "K, bye!");
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

        private static SubsonicAPI.LoginResult HandleLogin(string host, string username, string password)
        {    
            server = host;
            user = username;

            return Subsonic.LogIn(server, user, password);
        }     
    }
}
