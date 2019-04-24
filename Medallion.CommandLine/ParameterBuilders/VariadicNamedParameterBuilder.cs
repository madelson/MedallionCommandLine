using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Medallion.CommandLine.ParameterBuilders
{
    public sealed class VariadicNamedParameterBuilder<TValue>
        : CommandParameterBuilder<ReadOnlyCollection<TValue>, VariadicNamedParameterBuilder<TValue>>,
            IHasParser<TValue, VariadicNamedParameterBuilder<TValue>>,
            IVariadicHasValidator<TValue, VariadicNamedParameterBuilder<TValue>>
    {
        internal VariadicNamedParameterBuilder(string name, char? shortName)
            : base(name, ParameterKind.Named, shortName, isVariadic: true)
        {
            this.DefaultValue(Helpers.EmptyReadOnlyCollection<TValue>());
        }

        public VariadicNamedParameterBuilder<TValue> Required()
        {
            this.SetRequired();
            return this;
        }

        public new VariadicNamedParameterBuilder<TValue> Parser(CommandParameterParser<TValue> parser)
        {
            base.Parser = CommandParameterParser.FromElementParser(parser);
            return this;
        }

        public new VariadicNamedParameterBuilder<TValue> Parser(Func<string, TValue> parser) => this.Parser(CommandParameterParser.Create(parser));

        public VariadicNamedParameterBuilder<TValue> Validator(CommandParameterValidator<ReadOnlyCollection<TValue>> validator)
        {
            this.AddValidator(validator);
            return this;
        }

        public VariadicNamedParameterBuilder<TValue> Validator(CommandParameterValidator<TValue> validator) =>
            this.Validator(CommandParameterValidator.FromElementValidator(validator));

        public VariadicNamedParameterBuilder<TValue> Validator(Action<TValue> validator) => this.Validator(CommandParameterValidator.Create(validator));

        public VariadicNamedParameterBuilder<TValue> Validator(Action<ReadOnlyCollection<TValue>> validator) => this.Validator(CommandParameterValidator.Create(validator));
    }
}
