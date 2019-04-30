using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

#pragma warning disable SA1649 // want to allow for multiple types in this file

namespace Medallion.CommandLine.ParameterBuilders
{
    internal interface IHasParser<TValue, TBuilder>
    {
        TBuilder Parser(CommandParameterParser<TValue> parser);
        TBuilder Parser(Func<string, TValue> parser);
    }

    internal interface IHasValidator<TValue, TBuilder>
    {
        TBuilder Validator(IValidator<TValue> validator);
        TBuilder Validator(Action<TValue> validator);
    }
    
    internal interface IVariadicHasValidator<TValue, TBuilder> : IHasValidator<TValue, TBuilder>
    {
        TBuilder Validator(IValidator<ReadOnlyCollection<TValue>> validator);
        TBuilder Validator(Action<ReadOnlyCollection<TValue>> validator);
    }
}
