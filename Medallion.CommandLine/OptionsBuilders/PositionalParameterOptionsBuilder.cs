using System;
using System.Collections.Generic;
using System.Text;

namespace Medallion.CommandLine.OptionsBuilders
{
    public class PositionalParameterOptionsBuilder<TValue> : ParameterOptionsBuilder<PositionalParameterOptionsBuilder<TValue>, TValue>
    {
        internal PositionalParameterOptionsBuilder() { }
    }
}
