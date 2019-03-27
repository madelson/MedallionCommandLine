using System;
using System.Collections.Generic;
using System.Text;

namespace Medallion.CommandLine
{
    internal readonly struct NoDefault<T>
    {
        private readonly T _value;

        public NoDefault(T value)
        {
            this._value = value;
            this.HasValue = true;
        }

        public T Value => this.HasValue ? this._value : throw new InvalidOperationException("no value set");
        public bool HasValue { get; }

        public static implicit operator NoDefault<T>(T value) => new NoDefault<T>(value);
    }
}
