using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Medallion.CommandLine.ParameterBuilders
{
    public sealed class VariadicPositionalParameterBuilder<TValue>
        : CommandParameterBuilder<ReadOnlyCollection<TValue>, VariadicPositionalParameterBuilder<TValue>>, 
            IHasParser<TValue, VariadicPositionalParameterBuilder<TValue>>, 
            IVariadicHasValidator<TValue, VariadicPositionalParameterBuilder<TValue>>
    {
        internal VariadicPositionalParameterBuilder(string name)
            : base(name, ParameterKind.Positional, shortName: null, isVariadic: true)
        {
            this.DefaultValue(Helpers.EmptyReadOnlyCollection<TValue>());
        }

        public VariadicPositionalParameterBuilder<TValue> Required()
        {
            this.SetRequired();
            return this;
        }

        public new VariadicPositionalParameterBuilder<TValue> Parser(CommandParameterParser<TValue> parser)
        {
            base.Parser = CommandParameterParser.FromElementParser(parser);
            return this;
        }

        public new VariadicPositionalParameterBuilder<TValue> Parser(Func<string, TValue> parser) => this.Parser(CommandParameterParser.Create(parser));

        public VariadicPositionalParameterBuilder<TValue> Validator(CommandParameterValidator<ReadOnlyCollection<TValue>> validator)
        {
            this.AddValidator(validator);
            return this;
        }

        public VariadicPositionalParameterBuilder<TValue> Validator(CommandParameterValidator<TValue> validator) =>
            this.Validator(CommandParameterValidator.FromElementValidator(validator));

        public VariadicPositionalParameterBuilder<TValue> Validator(Action<TValue> validator) => this.Validator(CommandParameterValidator.Create(validator));

        public VariadicPositionalParameterBuilder<TValue> Validator(Action<ReadOnlyCollection<TValue>> validator) => this.Validator(CommandParameterValidator.Create(validator));
    }
}
