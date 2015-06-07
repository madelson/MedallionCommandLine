using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medallion.CommandLine.Model
{
    public sealed class ArgumentCollection : ReadOnlyCollection<Argument>
    {
        internal ArgumentCollection(IEnumerable<Argument> arguments)
            : base(arguments != null ? arguments.ToArray() : null)
        {
        }
    }
}
