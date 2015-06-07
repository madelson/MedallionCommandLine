using Medallion.CommandLine.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medallion.CommandLine.Model
{
    public sealed class Command : ParseNode
    {
        private readonly CommandTemplate template;

        internal Command(
            CommandTemplate template, 
            Command subCommand,
            IEnumerable<Option> options, 
            IEnumerable<Argument> arguments, 
            ListSegment<string> tokens, 
            IEnumerable<CommandLineParseError> errors) 
            : base(tokens, errors)
        {
            this.template = template;
            this.SubCommand = subCommand;
            this.Options = new OptionCollection(options);
            this.Arguments = new ArgumentCollection(arguments);
        }

        public Command SubCommand { get; private set; }

        public OptionCollection Options { get; private set; }

        public ArgumentCollection Arguments { get; private set; }
    }
}
