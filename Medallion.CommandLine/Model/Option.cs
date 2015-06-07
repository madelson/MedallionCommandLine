using Medallion.CommandLine.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medallion.CommandLine.Model
{
    class Option : ParseNode
    {
        internal Option(OptionTemplate template, ListSegment<string> tokens, IEnumerable<CommandLineParseError> errors) 
            : base(tokens, errors)
        {
            this.Template = template;
        }

        internal OptionTemplate Template { get; private set; }
    }
}
