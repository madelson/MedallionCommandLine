using System;
using System.Collections.Generic;
using System.Text;

namespace Medallion.CommandLine
{
    public abstract class CommandElement
    {
        private protected CommandElement(string name)
        {
            this.Name = name;
        }

        public string Name { get; }
    }
}
