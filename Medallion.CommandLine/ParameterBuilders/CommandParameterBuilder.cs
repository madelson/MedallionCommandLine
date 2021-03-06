﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Medallion.CommandLine.ParameterBuilders
{
    public abstract class CommandParameterBuilder<TValue, TBuilder>
        where TBuilder : CommandParameterBuilder<TValue, TBuilder>
    {
        private readonly string _name;
        private readonly ParameterKind _kind;
        private readonly char? _shortName;
        private readonly bool _isVariadic;

        private readonly List<IValidator<TValue>> _validators = new List<IValidator<TValue>>();
        private NoDefault<TValue> _defaultValue;
        private string _shortDescription;
        private string _description;
        private Uri _helpUrl;

        private protected CommandParameterBuilder(string name, ParameterKind kind, char? shortName, bool isVariadic)
        {
            this._name = name ?? throw new ArgumentNullException(nameof(name));
            this._kind = kind;
            this._shortName = shortName;
            this._isVariadic = isVariadic;
        }
        
        private protected CommandArgumentParser<TValue> Parser { get; set; }

        public TBuilder DefaultValue(TValue defaultValue)
        {
            this._defaultValue = defaultValue;
            return (TBuilder)this;
        }

        public TBuilder ShortDescription(string shortDescription)
        {
            this._shortDescription = shortDescription ?? throw new ArgumentNullException(nameof(shortDescription));
            return (TBuilder)this;
        }

        public TBuilder Description(string description)
        {
            this._description = description ?? throw new ArgumentNullException(nameof(description));
            return (TBuilder)this;
        }

        public TBuilder HelpUrl(Uri helpUrl)
        {
            if (helpUrl == null) { throw new ArgumentNullException(nameof(helpUrl)); }
            if (!helpUrl.IsAbsoluteUri) { throw new ArgumentException(nameof(helpUrl), "must be absolute"); }

            this._helpUrl = helpUrl;
            return (TBuilder)this;
        }

        private protected void SetRequired() => this._defaultValue = default;
        private protected void AddValidator(IValidator<TValue> validator) => 
            this._validators.Add(validator ?? throw new ArgumentNullException(nameof(validator)));

        internal CommandParameter<TValue> ToParameter() =>
            new CommandParameter<TValue>(
                this._name,
                this._kind,
                this._shortName,
                this._isVariadic,
                this._defaultValue,
                this.Parser ?? CommandArgumentParser<TValue>.Default,
                Validator.Combine(this._validators),
                shortDescription: this._shortDescription,
                description: this._description,
                this._helpUrl
            );
    }
}
