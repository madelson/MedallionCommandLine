using Medallion.CommandLine.Model;
using Medallion.CommandLine.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medallion.CommandLine.Builders
{
    public abstract class CommandParserBuilder<TBuilder> : ParserBuilderBase<TBuilder>
        where TBuilder : CommandParserBuilder<TBuilder>
    {
        private readonly string name;

        internal CommandParserBuilder(string name)
        {
            this.name = name;
        }

        private readonly List<Func<SubCommandParserBuilder>> subCommands = new List<Func<SubCommandParserBuilder>>();

        public TBuilder SubCommand(string name, Action<SubCommandParserBuilder> subCommandBuilder)
        {
            Throw.IfNullOrWhitespace(name, "name");
            Throw.IfNull(subCommandBuilder, "subCommandBuilder");

            this.subCommands.Add(() => new SubCommandParserBuilder(name).Initialize(subCommandBuilder));

            return (TBuilder)this;
        }

        private readonly List<Func<OptionParserBuilder>> options = new List<Func<OptionParserBuilder>>();

        public TBuilder Option(string name, Action<OptionParserBuilder> optionBuilder)
        {
            Throw.IfNullOrWhitespace(name, "name");
            Throw.IfNull(optionBuilder, "optionBuilder");

            this.options.Add(() => new OptionParserBuilder(name).Initialize(optionBuilder));

            return (TBuilder)this;
        }

        internal void Populate(CommandTemplate template)
        {
            this.subCommands.ForEach(builderFactory =>
            {
                var commandTemplate = new CommandTemplate();
                builderFactory().Populate(commandTemplate);
                template.SubCommands.Add(commandTemplate);
            });

            this.options.ForEach(builderFactory =>
            {
                var optionTemplate = new OptionTemplate();
                builderFactory().Populate(optionTemplate);
                template.Options.Add(optionTemplate);
            });
        }
    }
}
