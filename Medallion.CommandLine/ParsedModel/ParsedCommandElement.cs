using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Medallion.CommandLine
{
    public abstract class ParsedCommandElement
    {
        private protected ParsedCommandElement(CommandElement element, ReadOnlyCollection<string> tokens)
        {
            this.Element = element;
            this.Tokens = tokens;
        }

        public CommandElement Element { get; }
        public ReadOnlyCollection<string> Tokens { get; }
    }
}
