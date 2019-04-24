using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Medallion.CommandLine
{
    public class ParsedCommandLine : ParsedCommand
    {
        public ParsedCommandLine(CommandLine command, CommandArgumentCollection arguments, SubCommand subCommand, ReadOnlyCollection<string> tokens) 
            : base(command, arguments, tokens)
        {
            this.SubCommand = subCommand;
        }

        public SubCommand SubCommand { get; }
        public new CommandLine Command => (CommandLine)base.Command;
    }
}
