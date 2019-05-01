using System;
using System.Collections.Generic;
using System.Text;

namespace Medallion.CommandLine.ParameterBuilders
{
    public sealed class NamedParameterBuilder<TValue>
        : CommandParameterBuilder<TValue, NamedParameterBuilder<TValue>>, IHasParser<TValue, NamedParameterBuilder<TValue>>, IHasValidator<TValue, NamedParameterBuilder<TValue>>
    {
        internal NamedParameterBuilder(string name, char? shortName)
            : base(name, ParameterKind.Named, shortName, isVariadic: false)
        {
            this.DefaultValue(default);
        }

        public NamedParameterBuilder<TValue> Required()
        {
            this.SetRequired();
            return this;
        }

        public new NamedParameterBuilder<TValue> Parser(CommandArgumentParser<TValue> parser)
        {
            base.Parser = parser ?? throw new ArgumentNullException(nameof(parser));
            return this;
        }

        public new NamedParameterBuilder<TValue> Parser(Func<string, TValue> parser) => this.Parser(CommandArgumentParser.Create(parser));

        public NamedParameterBuilder<TValue> Validator(IValidator<TValue> validator)
        {
            this.AddValidator(validator);
            return this;
        }

        public NamedParameterBuilder<TValue> Validator(Action<TValue> validator) => this.Validator(Medallion.CommandLine.Validator.Create(validator));
    }
}
