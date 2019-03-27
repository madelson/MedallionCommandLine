using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Medallion.CommandLine
{
    public abstract class Argument : ParsedCommandElement
    {
        private protected Argument(Parameter parameter, object value, ReadOnlyCollection<string> tokens) : base(parameter, tokens)
        {
            this.Parameter = parameter;
            this.Value = value;
        }

        public Parameter Parameter { get; }
        public object Value { get; }
    }

    public class Argument<TValue> : Argument
    {
        internal Argument(Parameter<TValue> parameter, TValue value, ReadOnlyCollection<string> tokens) : base(parameter, value, tokens)
        {
        }

        public new Parameter<TValue> Parameter => (Parameter<TValue>)base.Parameter;
        public new TValue Value => (TValue)base.Value;
    }
}
