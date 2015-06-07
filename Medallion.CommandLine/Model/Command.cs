using Medallion.CommandLine.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medallion.CommandLine.Model
{
    abstract class Command
    {
        private readonly CommandTemplate template;

        internal Command(CommandTemplate template) 
        {
            this.template = template;
        }

        public Command SubCommand { get; private set; }

        public OptionCollection Options { get; private set; }

        public ArgumentCollection Arguments { get; private set; }
    }
}
