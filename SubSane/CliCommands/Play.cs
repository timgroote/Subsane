namespace SubSane.CliCommands
{
    public class Play : CliCommandType
    {

        public static readonly string[] Aliases = { "play", "resume" };

        public Play(string input) : base(input)
        {
        }

        public override string[] CallStrings
        {
            get { return Aliases; }
        }
        public override void Execute(IPlayer player, params string[] parameters)
        {
            player.Play();
        }
    }
}