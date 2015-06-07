using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medallion.CommandLine.Builders
{
    public sealed class SubCommandParserBuilder : CommandParserBuilder<SubCommandParserBuilder>
    {
        internal SubCommandParserBuilder(string name)
            : base(name)
        {
        }
    }
}
