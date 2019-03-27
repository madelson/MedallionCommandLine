using Medallion.CommandLine.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medallion.CommandLine.Builders
{
    public sealed class OptionParserBuilder : ParserBuilderBase<OptionParserBuilder>
    {
        private readonly string name;
        private Type type;
        private Func<string, object> parser;
        private Action<object> validator;
        private bool isFlag;
        private bool required;
        
        internal OptionParserBuilder(string name)
        {
            this.name = name;
        }

        public OptionParserBuilder<TValue> Parser<TValue>(Func<string, TValue> parser)
        {
            Throw.IfNull(parser, "parser");

            this.type = typeof(TValue);
            this.parser = s => (object)parser;
            return new OptionParserBuilder<TValue>(this);
        }

        public OptionParserBuilder<TValue> Type<TValue>()
        {
            return this.Parser<TValue>(DefaultParser<TValue>.Parse);
        }

        public OptionParserBuilder<bool> IsFlag()
        {
            this.isFlag = true;
            return this.Type<bool>();
        }

        public OptionParserBuilder Required(bool required = true)
        {
            this.required = required;
            return this;
        }

        internal void Validator(Action<object> validator)
        {
            this.validator = validator;
        }

        internal void Populate(OptionTemplate template)
        {
            template.Name = this.name;
            template.IsFlag = this.isFlag;
            template.Parser = this.parser;
            template.Type = this.type;
            template.Validator = this.validator;
            template.Required = this.required;
        }
    }

    public sealed class OptionParserBuilder<TValue>
    {
        private readonly OptionParserBuilder builder;

        internal OptionParserBuilder(OptionParserBuilder builder) 
        {
            this.builder = builder;
        }

        public OptionParserBuilder<TValue> Validator(Action<TValue> validator)
        {
            Throw.IfNull(validator, "validator");

            this.builder.Validator(o => validator((TValue)o));
            return this;
        }
    }
}
