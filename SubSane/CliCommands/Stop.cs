namespace SubSane.CliCommands
{
    public class Stop : CliCommandType
    {
        public Stop(string input) : base(input)
        {
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