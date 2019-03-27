using Medallion.CommandLine.Templates;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medallion.CommandLine.Builders
{
    public sealed class CommandLineParserBuilder : CommandParserBuilder<CommandLineParserBuilder>
    {
        private OptionStyles optionStyles = Medallion.CommandLine.OptionStyles.Default;

        internal CommandLineParserBuilder(string name = null)
            : base(name ?? GetDefaultCommandName())
        {
        }

        public CommandLineParserBuilder OptionStyles(OptionStyles optionStyles)
        {
            this.optionStyles = optionStyles;
            return this;
        }

        public CommandLineParser ToParser()
        {
            var commandTemplate = new CommandTemplate();
            this.Populate(commandTemplate);

            return new CommandLineParser(commandTemplate);
        }

        private static string GetDefaultCommandName()
        {
            // from yaclops project (https://github.com/dswisher/yaclops/blob/master/src/Yaclops/Help/HelpCommand.cs)
            var commandLineArgs = Environment.GetCommandLineArgs();
            if (commandLineArgs != null && commandLineArgs.Length > 0)
            {
                try
                {
                    var exeName = Path.GetFileNameWithoutExtension(commandLineArgs[0]);
                    if (!string.IsNullOrWhiteSpace(exeName))
                    {
                        return exeName;
                    }
                }
                catch
                {
                    // ignore errors related to parsing the path
                }
            }

            try
            {
                using (var process = Process.GetCurrentProcess())
                {
                    if (!string.IsNullOrWhiteSpace(process.ProcessName))
                    {
                        return process.ProcessName;
                    }
                }
            }
            catch
            {
                // ignore any errors from trying to fetch the process
            }

            return "[Unknown Command]";
        }
    }
}
