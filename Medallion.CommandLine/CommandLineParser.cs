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
            Throw.IfNullOrHasNulls(args, "args");

            var command = this.ParseCommand(this.template, new ListSegment<string>(args.ToArray()));
            return new CommandLineParseResult(command);
        }

        private Command ParseCommand(CommandTemplate template, ListSegment<string> args)
        {
            var errors = new List<CommandLineParseError>();

            // check for sub commands
            var subCommand = args.Any()
                ? template.SubCommands.Where(s => s.Name == args[0])
                    .Select(s => this.ParseCommand(s, args.Skip(1)))
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
                var options = new List<Option>();
                var arguments = new List<string>();
                this.GatherCommandOptionsAndArguments(template, args, errors, options, arguments);

                // validate options
                var missingRequiredOptions = template.Options.Where(o => o.Required)
                    .Except(options.Select(o => o.Template))
                    .ToList();
                missingRequiredOptions.ForEach(o => errors.Add(new CommandLineParseError()));

                var duplicateOptions = options.Where(o => !o.Template.AllowMultiple)
                    .GroupBy(o => o.Template)
                    .Where(g => g.Count() > 1)
                    .ToList();
                duplicateOptions.ForEach(g => errors.Add(new CommandLineParseError()));

                // validate arguments

            }
        }

        private void GatherCommandOptionsAndArguments(
            CommandTemplate template, 
            ListSegment<string> args, 
            List<CommandLineParseError> errors, 
            List<Option> options, 
            List<string> arguments)
        {
            if (args.Count == 0)
            {
                return;
            }

            var arg = args[0];
            if (arg.StartsWith("--"))
            {
                var optionName = arg.Substring(2);
                var match = template.Options.FirstOrDefault(o => o.Name == optionName);
                if (match != null)
                {
                    var option = this.ParseOption(match, args);
                    options.Add(option);
                    this.GatherCommandOptionsAndArguments(template, args.Skip(option.Tokens.Count), errors, options, arguments);
                }
                else
                {
                    errors.Add(new CommandLineParseError()); // unknown option
                }
            }
            else if (arg.StartsWith("-"))
            {
                var optionName = arg.Substring(1);
                var match = template.Options.FirstOrDefault(o => o.AllowShortName && o.Name.Substring(1) == optionName);
                if (match != null)
                {
                    var option = this.ParseOption(match, args);
                    options.Add(option);
                    this.GatherCommandOptionsAndArguments(template, args.Skip(option.Tokens.Count), errors, options, arguments);
                }
                else
                {
                    errors.Add(new CommandLineParseError()); // unknown option
                }
            }
            else
            {
                arguments.Add(arg);
                this.GatherCommandOptionsAndArguments(template, args.Skip(1), errors, options, arguments);
            }
        }

        private Option ParseOption(OptionTemplate template, ListSegment<string> args)
        {
            throw new NotImplementedException();
        }
    }
}
