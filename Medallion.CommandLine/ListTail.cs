using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medallion.CommandLine
{
    internal class ListTail<T> : IReadOnlyList<T>
    {
        private readonly IReadOnlyList<T> list;
        private readonly int start;

        public ListTail(IReadOnlyList<T> list, int start = 0)
        {
            this.list = list;
            this.start = start;
        }

        public T this[int index]
        {
            get { return this.list[index + start]; }
        }

        public int Count
        {
            get { return this.list.Count - start; }
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (var i = start; i < this.list.Count; ++i)
            {
                yield return this.list[i];
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public ListTail<T> Advance(int count = 1)
        {
            Throw.IfOutOfRange(count, min: 1, max: this.Count, paramName: "count");

            return new ListTail<T>(this.list, this.start + count);
        }
    }
}
