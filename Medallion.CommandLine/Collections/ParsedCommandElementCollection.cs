using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medallion.CommandLine.Collections
{
    // todo not sure we need this base... in which case we can implement IROD?
    public abstract class ParsedCommandElementCollection<TElement, TParsedElement> : IReadOnlyList<TParsedElement>, IReadOnlyDictionary<TElement, TParsedElement>
        where TElement : CommandElement
        where TParsedElement : ParsedCommandElement
    {
        private readonly IReadOnlyList<TParsedElement> _values;
        private readonly IReadOnlyDictionary<string, TParsedElement> _valuesByName;

        private protected ParsedCommandElementCollection(
            IEnumerable<TParsedElement> values,
            IEqualityComparer<string> nameComparer)
        {
            this._values = values.ToArray();
            this._valuesByName = values.ToDictionary(e => e.Element.Name, e => e, nameComparer);
        }

        public TParsedElement this[int index] => this._values[index];
        public TParsedElement this[string name] => this._valuesByName[name];
        public TParsedElement this[TElement element] => this.TryGetValue(element, out var value) ? value : throw new KeyNotFoundException();

        public int Count => this._values.Count;
        
        IEnumerable<TElement> IReadOnlyDictionary<TElement, TParsedElement>.Keys => this._valuesByName.Values.Select(v => (TElement)v.Element);
        
        IEnumerable<TParsedElement> IReadOnlyDictionary<TElement, TParsedElement>.Values => this._valuesByName.Values;

        public bool ContainsName(string name) => this._valuesByName.ContainsKey(name);

        public bool ContainsKey(TElement element) => this.TryGetValue(element, out _);

        public IEnumerator<TParsedElement> GetEnumerator() => this._values.GetEnumerator();

        public bool TryGetValue(string name, out TParsedElement value) => this._valuesByName.TryGetValue(name, out value);

        public bool TryGetValue(TElement element, out TParsedElement value)
        {
            if (!this.TryGetValue(element?.Name, out var valueByName)
                || valueByName.Element != element)
            {
                value = null;
                return false;
            }

            value = valueByName;
            return true;
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
        
        IEnumerator<KeyValuePair<TElement, TParsedElement>> IEnumerable<KeyValuePair<TElement, TParsedElement>>.GetEnumerator() =>
            this._valuesByName.Values.Select(e => new KeyValuePair<TElement, TParsedElement>((TElement)e.Element, e))
                .GetEnumerator();
    }
}
