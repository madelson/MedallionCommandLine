using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Medallion.CommandLine
{
    public abstract class ParsedCommand : ParsedCommandElement
    {
        private protected ParsedCommand(Command command, CommandArgumentCollection arguments, ReadOnlyCollection<string> tokens) 
            : base(command, tokens)
        {
            this.Arguments = arguments;
        }

        public CommandArgumentCollection Arguments { get; }
        public Command Command => (Command)this.Element;
    }
}
