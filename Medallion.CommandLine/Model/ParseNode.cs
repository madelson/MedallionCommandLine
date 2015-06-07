using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medallion.CommandLine.Model
{
    public abstract class ParseNode
    {
        internal ParseNode(ListSegment<string> tokens, IEnumerable<CommandLineParseError> errors) 
        {
            this.InternalTokens = tokens;
            this.Errors = new CommandLineParseErrorCollection(errors);
        }

        internal ListSegment<string> InternalTokens { get; private set; }
        public IReadOnlyList<string> Tokens { get { return this.InternalTokens; } }
        public CommandLineParseErrorCollection Errors { get; private set; }
    }
}
