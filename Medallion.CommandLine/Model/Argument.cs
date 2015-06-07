using Medallion.CommandLine.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medallion.CommandLine.Model
{
    public sealed class Argument : ParseNode
    {
        internal Argument(PositionalArgumentTemplate template, ListSegment<string> tokens, IEnumerable<CommandLineParseError> errors)
            : base(tokens, errors)
        {
            this.Template = template;
        }

        internal PositionalArgumentTemplate Template { get; private set; }
    }
}
