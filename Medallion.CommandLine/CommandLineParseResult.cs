using Medallion.CommandLine.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medallion.CommandLine
{
    public sealed class CommandLineParseResult
    {
        internal CommandLineParseResult(Command command)
        {
            this.Command = command;
        }

        public Command Command { get; private set; }
    }
}
