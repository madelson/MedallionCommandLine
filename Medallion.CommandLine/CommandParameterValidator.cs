using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Medallion.CommandLine
{
    public abstract class CommandParameterValidator
    {
        private protected CommandParameterValidator() { }

        internal static CommandParameterValidator<TValue> Combine<TValue>(IReadOnlyList<CommandParameterValidator<TValue>> validators)
        {
            throw new NotImplementedException();
        }

        internal static CommandParameterValidator<TValue> Create<TValue>(Action<TValue> validator) => throw new NotImplementedException();

        internal static CommandParameterValidator<ReadOnlyCollection<TValue>> FromElementValidator<TValue>(CommandParameterValidator<TValue> elementValidator) => throw new NotImplementedException();
    }

    public abstract class CommandParameterValidator<TValue> : CommandParameterValidator
    {        
    }
}
