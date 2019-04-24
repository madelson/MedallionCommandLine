using System;
using System.Collections.Generic;
using System.Text;

namespace Medallion.CommandLine
{
    public sealed class AlternativeCommandGrammar : CommandGrammar
    {
        public new void AddParameter(CommandParameter parameter) => base.AddParameter(parameter);
    }
}
