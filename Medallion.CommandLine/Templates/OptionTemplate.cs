using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medallion.CommandLine.Templates
{
    internal class OptionTemplate : ArgumentTemplate
    {
        public bool AllowShortName { get; set; }
        public bool AllowMultiple { get; set; }
        public bool IsFlag { get; set; }
    }
}
