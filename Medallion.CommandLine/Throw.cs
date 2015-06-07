using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medallion.CommandLine
{
    internal static class Throw
    {
        public static void IfNull<T>(T value, string paramName)
        {
            if (value == null)
            {
                throw new ArgumentNullException(paramName);
            }
        }

        public static void IfNullOrWhitespace(string value, string paramName)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentNullException(paramName, "may not be null or whitespace: ");
            }
        }

        public static void IfOutOfRange<T>(T value, T? min = null, T? max = null, string paramName = null)
            where T : struct
        {
            if ((min.HasValue && Comparer<T>.Default.Compare(value, min.Value) < 0)
                || (max.HasValue && Comparer<T>.Default.Compare(value, max.Value) > 0))
            {
                throw new ArgumentOutOfRangeException(paramName);
            }
        }
    }
}
