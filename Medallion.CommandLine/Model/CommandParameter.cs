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
            ObjectValidator validator,
            string shortDescription,
            string description,
            Uri helpUrl)
            : base(name)
        {
            this.Kind = kind;
            this.ValueType = valueType;
            this.ShortName = shortName;
            this.IsVariadic = isVariadic;
            this.DefaultValue = defaultValue;
            this.Parser = parser;
            this.Validator = validator;
        }

        internal ParameterKind Kind { get; }
        public Type ValueType { get; }

        internal char? ShortName { get; }
        internal bool IsVariadic { get; }
        internal NoDefault<object> DefaultValue { get; }
        internal CommandParameterParser Parser { get; }
        internal ObjectValidator Validator { get; }
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
            IValidator<TValue> validator,
            string shortDescription,
            string description, 
            Uri helpUrl)
            : base(
                  name, 
                  kind, 
                  typeof(TValue), 
                  shortName, 
                  isVariadic, 
                  defaultValue.HasValue ? new NoDefault<object>(defaultValue.Value) : default, 
                  parser, 
                  validator.ToObjectValidator(),
                  shortDescription: shortDescription,
                  description: description,
                  helpUrl)
        {
        }

        internal new NoDefault<TValue> DefaultValue => base.DefaultValue.HasValue ? new NoDefault<TValue>((TValue)base.DefaultValue.Value) : default;
        internal new CommandParameterParser<TValue> Parser => (CommandParameterParser<TValue>)base.Parser;
        internal new IValidator<TValue> Validator => (IValidator<TValue>)base.Validator;
    }
}
