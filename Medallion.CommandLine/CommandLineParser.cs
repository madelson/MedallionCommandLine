using Medallion.CommandLine.Model;
using Medallion.CommandLine.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medallion.CommandLine
{
    public sealed class CommandLineParser
    {
        private readonly CommandTemplate template;

        internal CommandLineParser(CommandTemplate template)
        {
            this.template = template;
        }

        public CommandLineParseResult Parse(IReadOnlyList<string> args)
        {
            Throw.IfNull(args, "args");

            var command = this.ParseCommand(this.template, new ListTail<string>(args));
            return new CommandLineParseResult(command);
        }

        private Command ParseCommand(CommandTemplate template, ListTail<string> args)
        {
            var errors = new List<CommandLineParseError>();

            // check for sub commands
            var subCommand = args.Any()
                ? template.SubCommands.Where(s => s.Name == args[0])
                    .Select(s => this.ParseCommand(s, args.Advance()))
                    .Take(1)
                    .FirstOrDefault()
                : null;
            
            if (subCommand == null)
            {
                if (template.RequiresSubCommand)
                {
                    errors.Add(new CommandLineParseError());
                }
            }
            else
            {
                // scan for options & arguments

            }
        }
    }
}
