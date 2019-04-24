using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medallion.CommandLine
{
    public sealed class CommandArgumentCollection : IReadOnlyList<CommandArgument>, IReadOnlyDictionary<CommandParameter, CommandArgument>, IReadOnlyDictionary<string, CommandArgument>
    {
        private readonly IReadOnlyList<CommandArgument> _arguments;
        private readonly IReadOnlyDictionary<string, CommandArgument> _argumentsByName;

        internal CommandArgumentCollection(IEnumerable<CommandArgument> arguments, IEqualityComparer<string> nameComparer)
        {
            this._arguments = arguments.ToArray();
            this._argumentsByName = arguments.ToDictionary(a => a.Parameter.Name, a => a, nameComparer);
        }

        public CommandArgument this[int index] => this._arguments[index];
        public CommandArgument this[CommandParameter key] => this.TryGetValue(key, out var argument) ? argument : throw new KeyNotFoundException();
        public CommandArgument this[string key] => this._argumentsByName[key];

        public int Count => this._arguments.Count;

        IEnumerable<CommandParameter> IReadOnlyDictionary<CommandParameter, CommandArgument>.Keys => this._arguments.Select(a => a.Parameter);
        IEnumerable<CommandArgument> IReadOnlyDictionary<CommandParameter, CommandArgument>.Values => this._arguments;
        IEnumerable<string> IReadOnlyDictionary<string, CommandArgument>.Keys => this._arguments.Select(a => a.Parameter.Name);
        IEnumerable<CommandArgument> IReadOnlyDictionary<string, CommandArgument>.Values => this._arguments;

        public bool ContainsKey(CommandParameter parameter) => this.TryGetValue(parameter, out _);
        public bool ContainsKey(string parameterName) => this._argumentsByName.ContainsKey(parameterName);

        public IEnumerator<CommandArgument> GetEnumerator() => this._arguments.GetEnumerator();

        public bool TryGetValue(CommandParameter parameter, out CommandArgument argument)
        {
            if (this._argumentsByName.TryGetValue(parameter?.Name, out var nameMatch)
                && nameMatch.Element == parameter)
            {
                argument = nameMatch;
                return true;
            }

            argument = null;
            return false;
        }

        public bool TryGetValue(string parameterName, out CommandArgument argument) => this._argumentsByName.TryGetValue(parameterName, out argument);

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        IEnumerator<KeyValuePair<CommandParameter, CommandArgument>> IEnumerable<KeyValuePair<CommandParameter, CommandArgument>>.GetEnumerator() => 
            this._arguments.Select(a => new KeyValuePair<CommandParameter, CommandArgument>(a.Parameter, a)).GetEnumerator();

        IEnumerator<KeyValuePair<string, CommandArgument>> IEnumerable<KeyValuePair<string, CommandArgument>>.GetEnumerator() =>
            this._arguments.Select(a => new KeyValuePair<string, CommandArgument>(a.Parameter.Name, a)).GetEnumerator();
    }
}
