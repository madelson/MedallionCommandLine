using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Medallion.CommandLine
{
    public abstract class CommandParameterParser
    {
        private protected CommandParameterParser() { }

        internal static CommandParameterParser<TValue> Create<TValue>(Func<string, TValue> parserFunc) => throw new NotImplementedException();
        internal static CommandParameterParser<ReadOnlyCollection<TValue>> FromElementParser<TValue>(CommandParameterParser<TValue> elementParser) => throw new NotImplementedException();
    }

    public abstract class CommandParameterParser<TValue> : CommandParameterParser
    {
        internal static CommandParameterParser<TValue> Default { get; }
    }
}
