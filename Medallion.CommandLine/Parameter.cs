using System;
using System.Collections.Generic;
using System.Text;

namespace Medallion.CommandLine
{
    public abstract class Parameter : CommandElement
    {
        private protected Parameter(string name, Type valueType) 
            : base(name)
        {
            this.ValueType = valueType;
        }

        public Type ValueType { get; }
    }

    public class Parameter<TValue> : Parameter
    {
        internal Parameter(string name)
            : base(name, typeof(TValue))
        {
        }
    }
}
