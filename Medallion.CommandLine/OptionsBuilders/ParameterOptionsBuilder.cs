using System;
using System.Collections.Generic;
using System.Text;

namespace Medallion.CommandLine.OptionsBuilders
{
    public abstract class ParameterOptionsBuilder<TBuilder, TValue>
        where TBuilder : ParameterOptionsBuilder<TBuilder, TValue>
    {
        private protected ParameterOptionsBuilder() { }
    }
}
