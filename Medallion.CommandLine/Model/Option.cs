using Medallion.CommandLine.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medallion.CommandLine.Model
{
    public sealed class Option : ParseNode
    {
        internal Option(OptionTemplate template, object value, ListSegment<string> tokens, IEnumerable<CommandLineParseError> errors) 
            : base(tokens, errors)
        {
            this.Template = template;
            this.Value = value;
        }

        internal OptionTemplate Template { get; private set; }
        public object Value { get; private set; }
    }
}
