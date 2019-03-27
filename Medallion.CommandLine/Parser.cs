using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Medallion.CommandLine
{
    public delegate bool TryParse<T>(string text, out T parsed);

#pragma warning disable SA1130
    public static class Parser
    {
        public static TryParse<T> For<T>() => DefaultParser<T>.Value;

        public static TryParse<T> Create<T>(Func<string, T> parse)
        {
            if (parse == null) { throw new ArgumentNullException(nameof(parse)); }

            return delegate(string text, out T parsed)
            {
                try
                {
                    parsed = parse(text);
                    return true;
                }
                catch
                {
                    parsed = default;
                    return false;
                }
            };
        }

        private static class DefaultParser<T>
        {
            private static TryParse<T> _value;

            public static TryParse<T> Value => _value ?? (_value = CreateDefault());

            private static TryParse<T> CreateDefault()
            {
                // special-case enums:
                if (typeof(T).GetTypeInfo().IsEnum)
                {
                    return CreateForEnum<T>();
                }

                // first priorty: TryParse static method
                var publicStaticMethods = typeof(T).GetTypeInfo().DeclaredMethods
                    .Where(m => m.IsPublic && m.IsStatic)
                    .ToArray();
                var tryParseMethod = publicStaticMethods.FirstOrDefault(
                    m => m.Name == "TryParse" 
                        && m.ReturnType == typeof(bool)
                        && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new[] { typeof(string), typeof(T).MakeByRefType() })
                );
                if (tryParseMethod != null)
                {
                    return delegate(string text, out T parsed)
                    {
                        var arguments = new object[] { text, null };
                        var result = (bool)tryParseMethod.InvokeWithOriginalException(obj: null, arguments: arguments);
                        parsed = (T)arguments[1];
                        return result;
                    };
                }

                // second priority: Parse static method
                var parseMethod = publicStaticMethods.FirstOrDefault(
                    m => m.Name == "Parse"
                        && m.ReturnType == typeof(T)
                        && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new[] { typeof(string) })
                );
                if (parseMethod != null)
                {
                    return Create(text => (T)parseMethod.InvokeWithOriginalException(obj: null, arguments: new object[] { text }));
                }

                // third priority: string constructor
                if (!typeof(T).GetTypeInfo().IsAbstract)
                {
                    var stringConstructor = typeof(T).GetTypeInfo().DeclaredConstructors
                        .Where(c => c.IsPublic && !c.IsStatic && c.GetParameters().Select(p => p.ParameterType).SequenceEqual(new[] { typeof(string) }))
                        .FirstOrDefault();
                    if (stringConstructor != null)
                    {
                        return Create(text => (T)stringConstructor.InvokeWithOriginalException(new object[] { text }));
                    }
                }

                // special-case nullabes
                var nullableUnderlyingType = Nullable.GetUnderlyingType(typeof(T));
                if (nullableUnderlyingType != null)
                {
                    return (TryParse<T>)typeof(Parser).GetTypeInfo().DeclaredMethods
                        .Single(m => m.Name == nameof(ForNullable))
                        .MakeGenericMethod(nullableUnderlyingType)
                        .InvokeWithOriginalException(obj: null, arguments: Array.Empty<object>());
                }

                throw new NotSupportedException(
                    $"Could not create a default parser for {typeof(T)}. The type must have a either a"
                    + $" 'public static bool TryParse(string, out {typeof(T)})' method, a"
                    + $" 'public static {typeof(T)} Parse(string)' method, or a 'public {typeof(T)}(string)' constructor"
                );
            }

            private static TryParse<TEnum> CreateForEnum<TEnum>()
            {
                // this is the default behavior for enums. We do not leverage native enum parsing because
                // we want to match (a) only defined values and (b) only text values. We ignore case by
                // default on the assumption that .NET enums will be PascalCase but CLIs probably don't want
                // that

                var valuesByName = Enum.GetValues(typeof(TEnum))
                    .Cast<TEnum>()
                    .ToDictionary(t => t.ToString(), t => t, StringComparer.OrdinalIgnoreCase);

                return delegate(string text, out TEnum parsed)
                {
                    if (text == null || !valuesByName.TryGetValue(text, out var value))
                    {
                        parsed = default;
                        return false;
                    }
                    parsed = value;
                    return true;
                };
            }
        }

        private static TryParse<T?> ForNullable<T>()
            where T : struct
        {
            var underlyingParser = For<T>();
            return delegate(string text, out T? parsed)
            {
                if (underlyingParser(text, out var underlyingParsed))
                {
                    parsed = underlyingParsed;
                    return true;
                }
                parsed = null;
                return false;
            };
        }
    }
#pragma warning restore SA1130
}
