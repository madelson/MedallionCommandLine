using System;
using System.Collections.Generic;
using System.Text;

namespace Medallion.CommandLine.Collections
{
    public sealed class ArgumentCollection : ParsedCommandElementCollection<Parameter, Argument>
    {
        internal ArgumentCollection(IEnumerable<Argument> arguments, IEqualityComparer<string> nameComparer)
            : base(arguments, nameComparer)
        {
        }

        public Argument<TValue> Get<TValue>(Parameter<TValue> parameter) => (Argument<TValue>)this[parameter];

        public bool TryGetValue<TValue>(Parameter<TValue> parameter, out Argument<TValue> argument)
        {
            if (this.TryGetValue(parameter, out Argument nonGenericArgument))
            {
                argument = (Argument<TValue>)nonGenericArgument;
                return true;
            }

            argument = null;
            return false;
        }

        public TValue GetArgumentValue<TValue>(int index) => (TValue)this[index].Value;
        public TValue GetArgumentValue<TValue>(string name) => (TValue)this[name].Value;
    }
}
