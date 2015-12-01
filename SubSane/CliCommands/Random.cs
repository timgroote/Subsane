using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubSane.CliCommands
{
    public class Random : CliCommandType
    {
        public static readonly string[] Aliases = { "random", "addrandom", "addshuffle"};

        public Random(string input) : base(input)
        {
        }

        public override string[] CallStrings { get; }
        public override void Execute(IPlayer player, params string[] parameters)
        {
            throw new NotImplementedException();
        }
    }
}
