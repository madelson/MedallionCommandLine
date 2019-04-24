using System;
using System.Collections.Generic;
using System.Text;

namespace Medallion.CommandLine
{
    public abstract class CommandParameter : CommandElement
    {
        private protected CommandParameter(
            string name, 
            ParameterKind kind, 
            Type valueType,
            char? shortName,
            bool isVariadic,
            NoDefault<object> defaultValue,
            CommandParameterParser parser,
            CommandParameterValidator validator)
            : base(name)
        {
            this.ValueType = valueType;
        }

        internal ParameterKind Kind { get; }
        public Type ValueType { get; }

        internal char? ShortName { get; }
        internal bool IsVariadic { get; }
        internal NoDefault<object> DefaultValue { get; }
        internal CommandParameterParser Parser { get; }
        internal CommandParameterValidator Validator { get; }
    }

    internal enum ParameterKind
    {
        Positional,
        Named,
        Switch,
    }

    public sealed class CommandParameter<TValue> : CommandParameter
    {
        internal CommandParameter(
            string name, 
            ParameterKind kind,
            char? shortName,
            bool isVariadic,
            NoDefault<TValue> defaultValue,
            CommandParameterParser<TValue> parser,
            CommandParameterValidator<TValue> validator)
            : base(name, kind, typeof(TValue), shortName, isVariadic, defaultValue.HasValue ? new NoDefault<object>(defaultValue.Value) : default, parser, validator)
        {
        }

        internal new NoDefault<TValue> DefaultValue => base.DefaultValue.HasValue ? new NoDefault<TValue>((TValue)base.DefaultValue.Value) : default;
        internal new CommandParameterParser<TValue> Parser => (CommandParameterParser<TValue>)base.Parser;
        internal new CommandParameterValidator<TValue> Validator => (CommandParameterValidator<TValue>)base.Validator;
    }
}
