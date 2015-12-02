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
    public class AddSong : CliCommandType
    {

        public static readonly string[] Aliases = { "addsong", "addid", "enqueuesong", "enqueueid" };

        public override string Description {
            get { return "adds a song specified by id to the play queue / playlist : (/addsong 213)"; }
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
                ConsoleUtils.UOut(ConsoleColor.Red, "You didn't enter a song id. use like this : (/listsongs 213)");
                return;
            }
            string songid = parameters[1];
            try
            {
                Song sg = Subsonic.GetSong(songid);
                if (sg == null)
                {
                    ConsoleUtils.UOut(ConsoleColor.Red, "unable to add song -> song result was null");
                }
                else
                {
                    player.AddSong(sg);
                }
            }
            catch (Exception e)
            {
                //todo log
                ConsoleUtils.UOut(ConsoleColor.Red, "unable to add song");
            }   
        }
    }
}
