using System;
using System.Collections.Generic;
using System.Text;

namespace Medallion.CommandLine
{
    public abstract class NamedParameter : Parameter
    {
        private protected NamedParameter(string name, Type valueType)
            : base(name, valueType)
        {
        }
    }

    public class NamedParameter<TValue> : NamedParameter
    {
        internal NamedParameter(string name)
            : base(name, typeof(TValue))
        {
        }
    }

    public class FlagParameter : NamedParameter<bool>
    {
        internal FlagParameter(string name) : base(name) { }
    }
}
