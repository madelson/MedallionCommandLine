using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Medallion.CommandLine
{
    /// <summary>
    /// Allows for defining the grammatical structure used to parse a command line. This is
    /// the starting point for the library
    /// </summary>
    public sealed class CommandLineGrammar : CommandGrammar
    {
        /// <summary>
        /// Creates a command line grammar using either the provided <paramref name="name"/>
        /// or (by default) the name of the current process
        /// </summary>
        public CommandLineGrammar(string name = null)
            : base(name ?? GetDefaultName())
        {
        }

        private static string GetDefaultName()
        {
            using (var process = Process.GetCurrentProcess())
            {
                return process.ProcessName;
            }
        }
    }
}
