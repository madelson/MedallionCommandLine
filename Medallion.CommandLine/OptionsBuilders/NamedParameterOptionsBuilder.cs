using System;
using System.Collections.Generic;
using System.Text;

namespace Medallion.CommandLine.OptionsBuilders
{
    public sealed class NamedParameterOptionsBuilder<TValue> : ParameterOptionsBuilder<NamedParameterOptionsBuilder<TValue>, TValue>
    {
        private NoDefault<char?> _shortName;

        internal NamedParameterOptionsBuilder() { }

        public NamedParameterOptionsBuilder<TValue> ShortName(char? shortName = null)
        {
            ValidateShortName(shortName);
            this._shortName = shortName;
            return this;
        }

        internal static void ValidateShortName(char? shortName)
        {
            if (shortName is char @char)
            {
                if (char.IsWhiteSpace(@char)) { throw new ArgumentException("may not be whitespace", nameof(shortName)); }
            }
        }
    }
}
