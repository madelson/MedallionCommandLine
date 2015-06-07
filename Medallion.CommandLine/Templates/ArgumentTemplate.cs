using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medallion.CommandLine.Templates
{
    internal class ArgumentTemplate
    {
        public bool Required { get; set; }
        public Type Type { get; set; }
        public Func<string, object> Parser { get; set; }
        public Action<object> Validator { get; set; }
    }
}
