using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medallion.CommandLine
{
    [Flags]
    public enum OptionStyles : long
    {
        DoubleDashOptionPrefix = 1 << 0,
        SingleDashOptionPrefix = 1 << 1,
        SlashOptionPrefix = 1 << 2,
        SingleDashShortAliasPrefix = 1 << 3,

        PositionalOptionArguments = 1 << 4,
        EqualsSeparatedOptionArguments = 1 << 5,
        ColonSeparatedOptionArguments = 1 << 6,

        DoubleDashOptionScanningTerminator = 1 << 7,
        AllowFlagCombination = 1 << 8,
        IgnoreCase = 1 << 9,

        Cmd = SlashOptionPrefix | ColonSeparatedOptionArguments | IgnoreCase,
        Powershell = SingleDashOptionPrefix | PositionalOptionArguments | IgnoreCase,
        Unix = DoubleDashOptionPrefix | SingleDashShortAliasPrefix | PositionalOptionArguments | DoubleDashOptionScanningTerminator | AllowFlagCombination,
        Default = Unix,
    }
}
