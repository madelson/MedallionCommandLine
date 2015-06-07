using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medallion.CommandLine
{
    internal class ListSegment<T> : IReadOnlyList<T>
    {
        private readonly IReadOnlyList<T> list;
        private readonly int start, count;

        public ListSegment(IReadOnlyList<T> list, int start = 0, int? count = null)
        {
            Throw.IfNull(list, "list");
            Throw.IfOutOfRange(start, min: 0, max: list.Count, paramName: "start");
            if (count.HasValue)
            {
                Throw.IfOutOfRange(count.Value, min: 0, max: list.Count - start, paramName: "count");
            }

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

        public ListSegment<T> Skip(int count)
        {
            Throw.IfOutOfRange(count, min: 0, max: this.Count, paramName: "count");

            return new ListSegment<T>(this.list, start: this.start + count);
        }

        public ListSegment<T> Take(int count)
        {
            Throw.IfOutOfRange(count, min: 0, max: this.count, paramName: "count");

            return new ListSegment<T>(this.list, start: this.start, count: count);
        }
    }
}
