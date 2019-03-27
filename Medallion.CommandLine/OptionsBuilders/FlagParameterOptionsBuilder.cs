using System;
using System.Collections.Generic;
using System.Text;

namespace Medallion.CommandLine.OptionsBuilders
{
    // todo do I actually want this?
    public sealed class FlagParameterOptionsBuilder
    {
        private NoDefault<char?> _shortName;

        internal FlagParameterOptionsBuilder() { }

        public FlagParameterOptionsBuilder ShortName(char? shortName = null)
        {
            NamedParameterOptionsBuilder<bool>.ValidateShortName(shortName);
            this._shortName = shortName;
            return this;
        }
    }
}
