using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Medallion.CommandLine.Collections;

namespace Medallion.CommandLine
{
    public abstract class ParsedCommand : ParsedCommandElement
    {
        private protected ParsedCommand(Command command, ArgumentCollection arguments, ReadOnlyCollection<string> tokens) 
            : base(command, tokens)
        {
            this.Arguments = arguments;
        }

        public ArgumentCollection Arguments { get; }
        public Command Command => (Command)this.Element;
    }
}
