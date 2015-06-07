using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medallion.CommandLine.Model
{
    public sealed class CommandLineParseErrorCollection : ReadOnlyCollection<CommandLineParseError>
    {
        internal CommandLineParseErrorCollection(IEnumerable<CommandLineParseError> errors)
            : base(errors != null ? errors.ToArray() : null)
        {
        }
    }
}
