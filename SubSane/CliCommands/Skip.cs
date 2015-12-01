namespace SubSane.CliCommands
{
    public class Skip : CliCommandType
    {
        private static readonly string[] Aliases = {
            "next",
            "skip"
        };

        public Skip(string input) : base(input)
        {
        }

        public override string[] CallStrings
        {
            get { return Aliases; }
        }
        public override void Execute(IPlayer player, params string[] parameters)
        {
            player.Skip();
        }
    }
}