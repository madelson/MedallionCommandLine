using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Medallion.CommandLine
{
    public abstract class CommandArgumentParser
    {
        private protected CommandArgumentParser() { }

        public abstract bool TryParse(IReadOnlyList<string> tokens, out object parsed, out string errorMessage);
        
        internal static CommandArgumentParser<TValue> Create<TValue>(Func<string, TValue> parser) => 
            new FuncParser<TValue>(parser ?? throw new ArgumentNullException(nameof(parser)));

        internal static CommandArgumentParser<ReadOnlyCollection<TValue>> FromElementParser<TValue>(CommandArgumentParser<TValue> elementParser) =>
            new CollectionParser<TValue>(elementParser ?? throw new ArgumentNullException(nameof(elementParser)));

        internal sealed class FuncParser<TValue> : SingleTokenParser<TValue>
        {
            private readonly Func<string, TValue> _parser;

            public FuncParser(Func<string, TValue> parser)
            {
                this._parser = parser;
            }

            protected override bool TryParse(string token, out TValue parsed, out string errorMessage)
            {
                try { parsed = this._parser(token); }
                catch (Exception ex)
                {
                    errorMessage = ex.Message;
                    parsed = default;
                    return false;
                }

                errorMessage = null;
                return true;
            }
        }

        private sealed class CollectionParser<TValue> : CommandArgumentParser<ReadOnlyCollection<TValue>>
        {
            private readonly CommandArgumentParser<TValue> _elementParser;

            public CollectionParser(CommandArgumentParser<TValue> elementParser)
            {
                this._elementParser = elementParser;
            }

            public override bool TryParse(IReadOnlyList<string> tokens, out ReadOnlyCollection<TValue> parsed, out string errorMessage)
            {
                if (tokens == null) { throw new ArgumentNullException(nameof(tokens)); }
                if (tokens.Contains(null)) { throw new ArgumentNullException(nameof(tokens) + ": must not contain null"); }

                var result = new TValue[tokens.Count];
                var singleToken = new string[1];
                for (var i = 0; i < result.Length; ++i)
                {
                    singleToken[0] = tokens[i];
                    if (!this._elementParser.TryParse(singleToken, out result[i], out errorMessage))
                    {
                        parsed = null;
                        return false;
                    }
                }

                parsed = new ReadOnlyCollection<TValue>(result);
                errorMessage = null;
                return true;
            }
        }
    }

    public abstract class CommandArgumentParser<TValue> : CommandArgumentParser
    {
        private static CommandArgumentParser<TValue> _cachedDefault;
        internal static CommandArgumentParser<TValue> Default => _cachedDefault ?? (_cachedDefault = DefaultCommandArgumentParserFactory.Create<TValue>());

        public sealed override bool TryParse(IReadOnlyList<string> tokens, out object parsed, out string errorMessage)
        {
            var result = this.TryParse(tokens, out TValue parsedValue, out errorMessage);
            parsed = parsedValue;
            return result;
        }

        public abstract bool TryParse(IReadOnlyList<string> tokens, out TValue parsed, out string errorMessage);
    }

    internal abstract class SingleTokenParser<TValue> : CommandArgumentParser<TValue>
    {
        public sealed override bool TryParse(IReadOnlyList<string> tokens, out TValue parsed, out string errorMessage)
        {
            if (tokens == null) { throw new ArgumentNullException(nameof(tokens)); }
            if (tokens.Count != 1) { throw new ArgumentOutOfRangeException(nameof(tokens), "must have length 1"); }
            var token = tokens[0];
            if (token == null) { throw new ArgumentNullException(nameof(tokens) + ": must not contain null"); }

            return this.TryParse(token, out parsed, out errorMessage);
        }

        protected abstract bool TryParse(string text, out TValue parsed, out string errorMessage);
    }
}
