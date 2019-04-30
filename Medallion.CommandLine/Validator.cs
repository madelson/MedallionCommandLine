//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Text;
//using System.Text.RegularExpressions;

//namespace Medallion.CommandLine
//{
//    public delegate string Validator<T>(T value);

//    // todo should be an abstract class
//    public static class Validator
//    {
//        public static Validator<T> Min<T>(T min, IComparer<T> comparer = null, bool exclusive = false, string message = null)
//        {
//            return value =>
//            {
//                return ((comparer ?? Comparer<T>.Default).Compare(value, min) >= (exclusive ? 1 : 0))
//                    ? null
//                    : message ?? $"expected {(exclusive ? ">" : ">=")} {min}";
//            };
//        }

//        public static Validator<T> Max<T>(T max, IComparer<T> comparer = null, bool exclusive = false, string message = null)
//        {
//            return value =>
//            {
//                return ((comparer ?? Comparer<T>.Default).Compare(value, max) <= (exclusive ? -1 : 0))
//                    ? null
//                    : message ?? $"expected {(exclusive ? "<" : "<=")} {max}";
//            };
//        }

//        public static Validator<FileSystemInfo> Exists { get; } = value => value.Exists ? null : $"{(value is FileInfo ? "file" : "directory")} does not exist";

//        public static Validator<string> Matches(Regex regex, string message = null) =>
//            value => regex.IsMatch(value) ? null : message ?? $"does not match /{regex}/";

//        public static Validator<string> IsNotEmpty { get; } = value => value == string.Empty ? "may not be the empty string" : null;

//        public static Validator<T> And<T>(this Validator<T> first, Validator<T> second)
//        {
//            if (first == null) { throw new ArgumentNullException(nameof(first)); }
//            if (second == null) { throw new ArgumentNullException(nameof(second)); }

//            return value => first(value) ?? second(value);
//        }
//    }
//}
