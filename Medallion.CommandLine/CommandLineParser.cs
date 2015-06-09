using Medallion.CommandLine.Builders;
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

        public static CommandLineParserBuilder Create(string commandName = null)
        {
            return new CommandLineParserBuilder(commandName);
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
            var options = new List<Option>();
            var arguments = new List<Argument>();

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
                var argumentTokens = new List<ListSegment<string>>();
                this.GatherCommandOptionsAndArguments(template, args, errors, options, argumentTokens);

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

                // parse arguments
                this.ParseArguments(template, argumentTokens, arguments, errors);
            }

            return new Command(
                template,
                subCommand,
                options,
                arguments,
                args,
                errors
            );
        }

        private void GatherCommandOptionsAndArguments(
            CommandTemplate template, 
            ListSegment<string> args, 
            List<CommandLineParseError> errors, 
            List<Option> options, 
            List<ListSegment<string>> arguments)
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
                if (optionName.Length > 1)
                {
                    // multi-flag option (e. g. -xds)
                    throw new NotImplementedException();
                }

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
                arguments.Add(args.Take(1));
                this.GatherCommandOptionsAndArguments(template, args.Skip(1), errors, options, arguments);
            }
        }

        private void ParseArguments(CommandTemplate template, List<ListSegment<string>> argumentTokens, List<Argument> arguments, List<CommandLineParseError> errors)
        {
            if (!template.Arguments.Any())
            {
                if (argumentTokens.Any())
                {
                    errors.Add(new CommandLineParseError()); // unexpected argument
                }
                return;
            }

            if (template.Arguments[0].Required)
            {
                // parse forward
                for (var i = 0; i < template.Arguments.Count; ++i)
                {
                    var argumentTemplate = template.Arguments[i];

                    if (i >= argumentTokens.Count)
                    { 
                        if (argumentTemplate.Required)
                        {
                            errors.Add(new CommandLineParseError()); // missing required arg
                        }
                        break; // further errors likely aren't meaningful
                    }

                    if (argumentTemplate.IsParams)
                    {
                        throw new NotImplementedException(); // need adjacency logic
                    }
                    else
                    {
                        arguments.Add(this.ParseArgument(argumentTemplate, argumentTokens[i]));
                    }
                }
            }
            else
            {
                // parse backward (TODO merge with forward)
                for (var i = 0; i < template.Arguments.Count; ++i)
                {
                    var argumentTemplate = template.Arguments[template.Arguments.Count - i - 1];

                    if (i >= argumentTokens.Count)
                    { 
                        if (argumentTemplate.Required)
                        {
                            errors.Add(new CommandLineParseError()); // missing required arg
                        }
                        break; // further errors likely aren't meaningful
                    }

                    if (argumentTemplate.IsParams)
                    {
                        throw new NotImplementedException(); // need adjacency logic
                    }
                    else
                    {
                        arguments.Add(this.ParseArgument(argumentTemplate, argumentTokens[template.Arguments.Count - i - 1]));
                    }
                }
            }
        }

        private Argument ParseArgument(ArgumentTemplate argument, ListSegment<string> args)
        {
            throw new NotImplementedException();
        }

        private Option ParseOption(OptionTemplate template, ListSegment<string> args)
        {
            var errors = new List<CommandLineParseError>();

            if (template.IsFlag)
            {
                return new Option(template, value: true, tokens: args.Take(1), errors: errors);
            }

            ListSegment<string> tokens;
            object value;
            if (args.Count < 2)
            {
                errors.Add(new CommandLineParseError()); // missing arg
                tokens = args.Take(1);
                value = null;
            }
            else
            {
                tokens = args.Take(2);

                try
                {
                    value = template.Parser(tokens[1]);
                }
                catch (Exception ex)
                {
                    errors.Add(new CommandLineParseError()); // ex
                    value = null;
                }

                if (!errors.Any())
                {
                    try
                    {
                        template.Validator(value);
                    }
                    catch (Exception ex)
                    {
                        errors.Add(new CommandLineParseError()); // ex
                    }
                }
            }

            return new Option(template, value, tokens, errors);
        }
    }
}
