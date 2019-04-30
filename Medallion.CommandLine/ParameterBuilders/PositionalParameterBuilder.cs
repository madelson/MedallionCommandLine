using System;
using System.Collections.Generic;
using System.Text;

namespace Medallion.CommandLine.ParameterBuilders
{
    public sealed class PositionalParameterBuilder<TValue> 
        : CommandParameterBuilder<TValue, PositionalParameterBuilder<TValue>>, IHasParser<TValue, PositionalParameterBuilder<TValue>>, IHasValidator<TValue, PositionalParameterBuilder<TValue>>
    {
        internal PositionalParameterBuilder(string name)
            : base(name, ParameterKind.Positional, shortName: null, isVariadic: false)
        {
            this.SetRequired();
        }

        public new PositionalParameterBuilder<TValue> Parser(CommandParameterParser<TValue> parser)
        {
            base.Parser = parser ?? throw new ArgumentNullException(nameof(parser));
            return this;
        }

        public new PositionalParameterBuilder<TValue> Parser(Func<string, TValue> parser) => this.Parser(CommandParameterParser.Create(parser));

        public PositionalParameterBuilder<TValue> Validator(IValidator<TValue> validator)
        {
            this.AddValidator(validator);
            return this;
        }

        public PositionalParameterBuilder<TValue> Validator(Action<TValue> validator) => this.Validator(Medallion.CommandLine.Validator.Create(validator));
    }
}
