using System;
using System.Collections.Generic;
using System.Text;

namespace Medallion.CommandLine
{
    public sealed class SubCommandGrammar : CommandGrammar
    {
        public SubCommandGrammar(string name)
        {
            if (string.IsNullOrEmpty(name)) { throw new ArgumentException("may not be null or empty", nameof(name)); }
        }

        internal SubCommand ToSubCommand()
        {
            throw new NotImplementedException();
        }
    }
}
