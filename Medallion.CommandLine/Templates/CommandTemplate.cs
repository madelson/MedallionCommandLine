using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medallion.CommandLine.Templates
{
    internal class CommandTemplate : TemplateBase
    {
        public bool RequiresSubCommand { get; set; }

        private readonly List<CommandTemplate> subCommands = new List<CommandTemplate>();
        public List<CommandTemplate> SubCommands { get { return this.subCommands; } }

        private readonly List<OptionTemplate> options = new List<OptionTemplate>();
        public ICollection<OptionTemplate> Options { get { return this.options; } }

        private readonly List<ArgumentTemplate> arguments = new List<ArgumentTemplate>();
        public List<ArgumentTemplate> Arguments { get { return this.arguments; } }
    }
}
