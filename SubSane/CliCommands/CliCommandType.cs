using System;

namespace SubSane.CliCommands
{
    public abstract class CliCommandType
    {
        protected string[] tokens;

        public abstract string[] CallStrings { get; }

        public abstract void Execute(IPlayer player, params string[] parameters);

        public CliCommandType(string input)
        {
            tokens = input.Split(new char[] {' '}, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}