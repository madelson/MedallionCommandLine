using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medallion.CommandLine.Model
{
    public sealed class OptionCollection : ReadOnlyCollection<Option>
    {
        internal OptionCollection(IEnumerable<Option> options)
            : base(options != null ? options.ToArray() : null)
        {
        }
    }
}
