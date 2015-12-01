using SubSane.Players;

namespace SubSane.CliCommands
{
    public class Pause : CliCommandType
    {
        public static readonly string[] Aliases = { "pause" };

        public Pause(string input) : base(input)
        {
        }

        public override string Description
        {
            get { return "pause or resume playback"; }
        }

        public override string[] CallStrings
        {
            get { return Aliases; }
        }

        public override void Execute(IPlayer player, params string[] parameters)
        {
            player.Pause();
        }
    }
}