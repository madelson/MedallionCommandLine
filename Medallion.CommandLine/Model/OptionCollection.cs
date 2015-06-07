using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medallion.CommandLine.Model
{
    class OptionCollection : IReadOnlyList<Option>
    {
        private readonly IReadOnlyList<Option> options;

        public Option this[int index]
        {
            get 
            {
                Throw.IfOutOfRange(index, min: 0, max: this.options.Count, paramName: "index");
                return this.options[index];
            }
        }

        public Option this[string name]
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public int Count
        {
            get { return this.options.Count; }
        }

        public IEnumerator<Option> GetEnumerator()
        {
            return this.options.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
