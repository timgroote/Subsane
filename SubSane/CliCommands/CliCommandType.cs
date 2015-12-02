using System;
using SubSane.Players;

namespace SubSane.CliCommands
{
    public abstract class CliCommandType
    {
        protected string[] tokens;

        public abstract string Description { get; }

        public abstract string[] CallStrings { get; }

        public abstract void Execute(IPlayer player, params string[] parameters);

        protected CliCommandType()
        {
            
        }
    }
}