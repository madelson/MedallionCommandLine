using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Medallion.CommandLine.ParameterBuilders;

namespace Medallion.CommandLine
{
    /// <summary>
    /// Provides common methods for editing command line and sub command grammars
    /// </summary>
    public abstract class CommandGrammar
    {
        private readonly List<CommandParameter> _parameters = new List<CommandParameter>();
        private readonly List<SubCommand> _subCommands = new List<SubCommand>();
        private readonly List<AlternativeCommandGrammar> _alternatives = new List<AlternativeCommandGrammar>();

        private protected CommandGrammar() { }

        public CommandParameter<TValue> AddPositionalParameter<TValue>(string name, Action<PositionalParameterBuilder<TValue>> options = null) =>
            this.AddParameter<TValue, PositionalParameterBuilder<TValue>>(new PositionalParameterBuilder<TValue>(name), options);

        public CommandParameter<string> AddPositionalParameter(string name, Action<PositionalParameterBuilder<string>> options = null) => 
            this.AddPositionalParameter<string>(name, options);

        public CommandParameter<ReadOnlyCollection<TValue>> AddVariadicPositionalParameter<TValue>(string name, Action<VariadicPositionalParameterBuilder<TValue>> options = null) =>
            this.AddParameter<ReadOnlyCollection<TValue>, VariadicPositionalParameterBuilder<TValue>>(new VariadicPositionalParameterBuilder<TValue>(name), options);

        public CommandParameter<ReadOnlyCollection<string>> AddVariadicPositionalParameter(string name, Action<VariadicPositionalParameterBuilder<string>> options = null) =>
            this.AddVariadicPositionalParameter<string>(name, options);

        public CommandParameter<TValue> AddNamedParameter<TValue>(string name, char? shortName = null, Action<NamedParameterBuilder<TValue>> options = null) =>
            this.AddParameter<TValue, NamedParameterBuilder<TValue>>(new NamedParameterBuilder<TValue>(name, shortName), options);

        public CommandParameter<string> AddNamedParameter(string name, char? shortName = null, Action<NamedParameterBuilder<string>> options = null) => 
            this.AddNamedParameter<string>(name, shortName, options);

        public CommandParameter<ReadOnlyCollection<TValue>> AddVariadicNamedParameter<TValue>(string name, char? shortName = null, Action<VariadicNamedParameterBuilder<TValue>> options = null) =>
            this.AddParameter<ReadOnlyCollection<TValue>, VariadicNamedParameterBuilder<TValue>>(new VariadicNamedParameterBuilder<TValue>(name, shortName), options);

        public CommandParameter<ReadOnlyCollection<string>> AddVariadicNamedParameter(string name, char? shortName = null, Action<VariadicNamedParameterBuilder<string>> options = null) =>
            this.AddVariadicNamedParameter(name, shortName, options);

        public CommandParameter<bool> AddSwitchParameter(string name, char? shortName = null, Action<SwitchParameterBuilder> options = null) =>
            this.AddParameter<bool, SwitchParameterBuilder>(new SwitchParameterBuilder(name, shortName), options);

        public SubCommand AddSubCommand(SubCommandGrammar subCommandGrammar)
        {
            if (subCommandGrammar == null) { throw new ArgumentNullException(nameof(subCommandGrammar)); }

            var subCommand = subCommandGrammar.ToSubCommand();
            this._subCommands.Add(subCommand);
            return subCommand;
        }
        
        private CommandParameter<TValue> AddParameter<TValue, TBuilder>(TBuilder builder, Action<TBuilder> options)
            where TBuilder : CommandParameterBuilder<TValue, TBuilder>
        {
            options?.Invoke(builder);
            var parameter = builder.ToParameter();
            this.AddParameter(parameter);
            return parameter;
        }

        private protected void AddParameter(CommandParameter parameter) => this._parameters.Add(parameter);
    }
}
