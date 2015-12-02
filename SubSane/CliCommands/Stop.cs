using SubSane.Players;

namespace SubSane.CliCommands
{
    public class Stop : CliCommandType
    {

        public override string Description
        {
            get { return "stops playback"; }
        }

        public override string[] CallStrings
        {
            get { return Aliases; }
        }

        public static readonly string[] Aliases = {"stop"};

        public override void Execute(IPlayer player, params string[] parameters)
        {
            player.Stop();
        }
    }
}