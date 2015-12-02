using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SubSane.ConsoleForms;
using SubSane.Players;

namespace SubSane.CliCommands
{
    public static class CliCommandFactory
    {
        public static readonly List<CliCommandType> KnownTypes = new List<CliCommandType>()
        {
            new AddRandomSong(),
            new AddSong(),
            new Help(),
            new Info(),
            new ListAlbums(),
            new ListSongs(),
            new Pause(),
            new Play(),
            new PlayList(),
            new Skip(),
            new Stop(),
            new Partymode(),
            new DumbMode()
        };

        public static void Execute(IPlayer player, string input)
        {
            string[] tokens = input.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (!tokens.Any()) return;

            if (!(tokens[0].StartsWith("/")))
            {
                //is chat message?   
                Console.Out.WriteLine("TODO : CHAT?");
            }

            tokens[0] = tokens[0].Remove(0, 1); //remove pre slash

            foreach (CliCommandType tp in KnownTypes.Where(tp => tp.CallStrings.Contains(tokens[0])))
            {
                tp.Execute(player, tokens);
                return;
            }

            ConsoleUtils.UOut(ConsoleColor.White, "command not found. try /help for a list of available commands", ConsoleColor.Red);
        }          
    }
}
