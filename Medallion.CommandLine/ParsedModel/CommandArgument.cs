using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Medallion.CommandLine
{
    public abstract class CommandArgument : ParsedCommandElement
    {
        private protected CommandArgument(CommandParameter parameter, object value, ReadOnlyCollection<string> tokens) : base(parameter, tokens)
        {
            this.Parameter = parameter;
            this.Value = value;
        }

        public CommandParameter Parameter { get; }
        public object Value { get; }
    }

    public class CommandArgument<TValue> : CommandArgument
    {
        internal CommandArgument(CommandParameter<TValue> parameter, TValue value, ReadOnlyCollection<string> tokens) : base(parameter, value, tokens)
        {
        }

        public new CommandParameter<TValue> Parameter => (CommandParameter<TValue>)base.Parameter;
        public new TValue Value => (TValue)base.Value;
    }
}
