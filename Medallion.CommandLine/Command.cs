using System;
using System.Collections.Generic;
using System.Text;

namespace Medallion.CommandLine
{
    public abstract class Command : CommandElement
    {
        private protected Command(string name) : base(name)
        {
        }
    }
}
