using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Medallion.CommandLine
{
    public class ParsedSubCommand : ParsedCommand
    {
        internal ParsedSubCommand(SubCommand command, CommandArgumentCollection arguments, ReadOnlyCollection<string> tokens) : base(command, arguments, tokens)
        {
        }

        public new SubCommand Command => (SubCommand)base.Command;
    }
}
