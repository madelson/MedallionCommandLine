using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medallion.CommandLine.Builders
{
    class SubCommandParserBuilder : CommandParserBuilder<SubCommandParserBuilder>
    {
        private readonly string name;

        internal SubCommandParserBuilder(string name)
        {
            this.name = name;
        }
    }
}
