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
        private readonly bool ignoreCase;
        private IReadOnlyDictionary<string, Option> optionsByName;

        internal OptionCollection(IEnumerable<Option> options, bool ignoreCase)
            : base(options != null ? options.ToArray() : null)
        {
            this.ignoreCase = ignoreCase;
        }

        public Option this[string name]
        {
            get
            {
                Option value;
                if (!this.TryGetValue(name, out value))
                {
                    throw new KeyNotFoundException(string.Format("no option could be found with name matching '{0}'", name));
                }
                return value;
            }
        }

        public bool TryGetValue(string name, out Option option)
        {
            Throw.IfNull(name, "name");

            this.EnsureOptionsByNameInitialized();

            return this.optionsByName.TryGetValue(name, out option);
        }

        public bool ContainsKey(string name)
        {
            Option ignored;
            return this.TryGetValue(name, out ignored);
        }

        private void EnsureOptionsByNameInitialized()
        {
            if (this.optionsByName == null)
            {
                var dictionary = new Dictionary<string, Option>(this.ignoreCase ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal);
                foreach (var option in this)
                {
                    dictionary.Add(option.Template.Name, option);
                    if (option.Template.AllowShortName)
                    {
                        dictionary.Add(option.Template.Name.Substring(0, 1), option);
                    }
                }
                this.optionsByName = dictionary;
            }
        }
    }
}
