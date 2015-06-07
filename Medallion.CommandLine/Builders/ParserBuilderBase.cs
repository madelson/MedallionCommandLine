using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medallion.CommandLine.Builders
{
    public abstract class ParserBuilderBase<TBuilder>
        where TBuilder : ParserBuilderBase<TBuilder>
    {
        internal ParserBuilderBase() { }

        internal TBuilder Initialize(Action<TBuilder> builder)
        {
            builder((TBuilder)this);
            return (TBuilder)this;
        }
    }
}
