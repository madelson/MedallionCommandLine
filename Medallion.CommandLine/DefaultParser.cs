using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Medallion.CommandLine
{
    internal static class DefaultParser<T>
    {
        private static Func<string, object> @default;

        public static T Parse(string text)
        {
            Throw.IfNull(text, "text");

            return (T)(@default ?? (@default = DefaultParser.CreateDefaultParser(typeof(T))))(text);
        }
    }

    internal static class DefaultParser
    {
        public static Func<string, object> CreateDefaultParser(Type type)
        {
            if (type == typeof(string))
            {
                return s => s;
            }

            if (type.IsEnum)
            {
                return s => ParseEnum(type, s);
            }

            var parseMethod = type.GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Where(m => m.Name == "Parse")
                .Where(m => m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new[] { typeof(string) }))
                .FirstOrDefault();
            if (parseMethod != null)
            {
                return s => ParseParseableType(parseMethod, s);
            }
 
            throw new NotSupportedException(string.Format(
                "Default parsing is not supported for type {0}. Please specify a custom parser",
                type
            ));
        }

        private static object ParseEnum(Type type, string text)
        {
            object result;

            // disallow numeric enum parsing. We can't use int.TryParse because 
            // we need to support both long and ulong
            if (text.Length > 0 && char.IsLetter(text[0]))
            {
                try
                {
                    result = Enum.Parse(type, text);
                }
                catch
                {
                    result = null;
                }
            }
            else
            {
                result = null;
            }

            if (result == null || !Enum.IsDefined(type, result))
            {
                throw new FormatException(string.Format(
                    "Invalid value '{0}': expected one of [{1}]",
                    text,
                    string.Join(", ", Enum.GetValues(type))
                ));
            }

            return result;
        }

        private static object ParseParseableType(MethodInfo parseMethod, string text)
        {
            try
            {
                return parseMethod.Invoke(null, new object[] { text });
            }
            catch (Exception ex)
            {
                throw new FormatException(string.Format("Expected {0}, got '{1}'", parseMethod.DeclaringType.Name, text), ex);
            }
        }
    }
}
