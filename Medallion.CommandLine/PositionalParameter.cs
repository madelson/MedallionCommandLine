using System;
using System.Collections.Generic;
using System.Text;

namespace Medallion.CommandLine
{
    public abstract class PositionalParameter : Parameter
    {
        private protected PositionalParameter(string name, Type valueType)
            : base(name, valueType)
        {
        }
    }

    public class PositionalParameter<TValue> : PositionalParameter
    {
        internal PositionalParameter(string name)
            : base(name, typeof(TValue))
        {
        }
    }
}
