using Medallion.CommandLine.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medallion.CommandLine.Builders
{
    public sealed class OptionParserBuilder : ParserBuilderBase<OptionParserBuilder>
    {
        private readonly string name;

        internal OptionParserBuilder(string name)
        {
            this.name = name;
        }

        internal void Populate(OptionTemplate template)
        {
            throw new NotImplementedException();
        }
    }
}
