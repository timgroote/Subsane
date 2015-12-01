using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SubSane.Players;

namespace SubSane.CliCommands
{
    public class Help : CliCommandType
    {
        public static readonly string[] Aliases = { "?", "help", "wtf" };

        public Help(string input) : base(input)
        {
        }

        public override string Description {
            get { return "shows this list"; }
        }

        public override string[] CallStrings {
            get
            {
                return Aliases;
            }
        }
        public override void Execute(IPlayer player, params string[] parameters)
        {
            Console.Out.WriteLine(@"/exit
	quit subsane

/play
	start / resume playback
	
/pause, /resume
	pause / resume playback
	
/stop
	stop playback
	
/skip, /next
	play next song
	
/queue
	display playlist
	
/random
	enqueue a random song
	
/whatsplaying
	show for each subsonic user what they are currently playing
	
/listalbums [artistname]
	allows you to enter a wildcard artist name,
	then shows all  matching albums and ids 
	
/listsongs [album id]
	allows you to enter an album id
	lists all songs and their ids for that album

/addid [song id]
	allows you to enter a song id
	enqueues song

/info
    shows current playback info

/partymode
    enter party mode! never run out of playlist! keeps 10 randomly picked songs in your playlist at all times

/dumbmode
    dumb mode : stop after playlist gets empty.

/anco
    Tries to look for 'up to my neck in you' by AC/DC and enqueues it if found

/?, /help, /wtf
	show this list");
        }
    }
}
