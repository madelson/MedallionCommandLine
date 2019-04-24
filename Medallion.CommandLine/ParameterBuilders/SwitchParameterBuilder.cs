using System;
using System.Collections.Generic;
using System.Text;

namespace Medallion.CommandLine.ParameterBuilders
{
    public sealed class SwitchParameterBuilder : CommandParameterBuilder<bool, SwitchParameterBuilder>
    {
        internal SwitchParameterBuilder(string name, char? shortName)
            : base(name, ParameterKind.Switch, shortName, isVariadic: false)
        {
            this.DefaultValue(false);
        }
    }
}
