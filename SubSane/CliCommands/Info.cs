using System;
using SubSane.ConsoleForms;
using SubSane.Players;

namespace SubSane.CliCommands
{
    public class Info : CliCommandType
    {

        public override string Description
        {
            get { return "shows info on the currently playing song"; }
        }

        public override string[] CallStrings
        {
            get { return Aliases; }
        }

        public static readonly string[] Aliases = {"info"};

        public override void Execute(IPlayer player, params string[] parameters)
        {
            ConsoleUtils.UOut(ConsoleColor.Black, "Now playing {0}", ConsoleColor.White, player.GetCurrentSong());
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Out.Write("[");
            Console.BackgroundColor = ConsoleColor.DarkBlue;

            int numberOfBlocks = (int)Math.Floor((double)(player.ProgressPercentage / 2));
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
        }
    }
}