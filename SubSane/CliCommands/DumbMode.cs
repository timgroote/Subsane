using SubSane.Players;

namespace SubSane.CliCommands
{
    public class DumbMode : CliCommandType
    {

        public override string Description
        {
            get { return "stops party mode and resumes playback via normal enqueuing"; }
        }

        public override string[] CallStrings
        {
            get { return Aliases; }
        }

        public static readonly string[] Aliases = {"dumbmode"};

        public override void Execute(IPlayer player, params string[] parameters)
        {
            Program.mode = PlayMode.Dumb;
        }
    }
}