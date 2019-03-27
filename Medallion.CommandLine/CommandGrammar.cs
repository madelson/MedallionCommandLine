using System;
using System.Collections.Generic;
using System.Text;

namespace Medallion.CommandLine
{
    /// <summary>
    /// Provides common methods for editing command line and sub command grammars
    /// </summary>
    public abstract class CommandGrammar
    {
        private readonly string _name;

        private protected CommandGrammar(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) { throw new ArgumentException("may not be null or whitespace", nameof(name)); }

            this._name = name;
        }
    }
}
