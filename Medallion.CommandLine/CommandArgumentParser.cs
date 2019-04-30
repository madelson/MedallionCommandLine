//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using System.Text;

//namespace Medallion.CommandLine
//{
//    public abstract class CommandArgumentParser<TArgument>
//    {
//        public abstract bool TryParse(string text, out TArgument parsed, out string usageMessage);

//        internal static CommandArgumentParser<TArgument> Create(Func<string, TArgument> parser) => new FuncParser(parser);

//        private sealed class FuncParser : CommandArgumentParser<TArgument>
//        {
//            private readonly Func<string, TArgument> _parser;

//            public FuncParser(Func<string, TArgument> parser)
//            {
//                this._parser = parser;
//            }

//            public override bool TryParse(string text, out TArgument parsed, out string usageMessage)
//            {
//                try
//                {
//                    parsed = this._parser(text);
//                    usageMessage = null;
//                    return true;
//                }
//                catch (Exception ex)
//                {
//                    usageMessage = ex.Message;
//                    parsed = default;
//                    return false;
//                }
//            }
//        }
//    }

//    public delegate bool TryParseCommandArgument<T>(string text, out T parsed);

//#pragma warning disable SA1130 // work around stylecop issue
//    public abstract class Parser
//    {
//        public static TryParseCommandArgument<T> For<T>() => DefaultParser<T>.Value;

//        public static TryParseCommandArgument<T> Create<T>(Func<string, T> parse)
//        {
//            if (parse == null) { throw new ArgumentNullException(nameof(parse)); }

//            return delegate(string text, out T parsed)
//            {
//                try
//                {
//                    parsed = parse(text);
//                    return true;
//                }
//                catch
//                {
//                    parsed = default;
//                    return false;
//                }
//            };
//        }

//        private static class DefaultParser<T>
//        {
//            private static TryParseCommandArgument<T> _value;

//            public static TryParseCommandArgument<T> Value => _value ?? (_value = CreateDefault());

//            private static TryParseCommandArgument<T> CreateDefault()
//            {
//                // special-case enums:
//                if (typeof(T).GetTypeInfo().IsEnum)
//                {
//                    return CreateForEnum<T>();
//                }

//                // first priorty: TryParse static method
//                var publicStaticMethods = typeof(T).GetTypeInfo().DeclaredMethods
//                    .Where(m => m.IsPublic && m.IsStatic)
//                    .ToArray();
//                var tryParseMethod = publicStaticMethods.FirstOrDefault(
//                    m => m.Name == "TryParse" 
//                        && m.ReturnType == typeof(bool)
//                        && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new[] { typeof(string), typeof(T).MakeByRefType() })
//                );
//                if (tryParseMethod != null)
//                {
//                    return delegate(string text, out T parsed)
//                    {
//                        var arguments = new object[] { text, null };
//                        var result = (bool)tryParseMethod.InvokeWithOriginalException(obj: null, arguments: arguments);
//                        parsed = (T)arguments[1];
//                        return result;
//                    };
//                }

//                // second priority: Parse static method
//                var parseMethod = publicStaticMethods.FirstOrDefault(
//                    m => m.Name == "Parse"
//                        && m.ReturnType == typeof(T)
//                        && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new[] { typeof(string) })
//                );
//                if (parseMethod != null)
//                {
//                    return Create(text => (T)parseMethod.InvokeWithOriginalException(obj: null, arguments: new object[] { text }));
//                }

//                // todo should accept further optional parameters
//                // third priority: string constructor
//                if (!typeof(T).GetTypeInfo().IsAbstract)
//                {
//                    var stringConstructor = typeof(T).GetTypeInfo().DeclaredConstructors
//                        .Where(c => c.IsPublic && !c.IsStatic && c.GetParameters().Select(p => p.ParameterType).SequenceEqual(new[] { typeof(string) }))
//                        .FirstOrDefault();
//                    if (stringConstructor != null)
//                    {
//                        return Create(text => (T)stringConstructor.InvokeWithOriginalException(new object[] { text }));
//                    }
//                }

//                // special-case nullabes
//                var nullableUnderlyingType = Nullable.GetUnderlyingType(typeof(T));
//                if (nullableUnderlyingType != null)
//                {
//                    return (TryParseCommandArgument<T>)typeof(CommandArgumentParser).GetTypeInfo().DeclaredMethods
//                        .Single(m => m.Name == nameof(ForNullable))
//                        .MakeGenericMethod(nullableUnderlyingType)
//                        .InvokeWithOriginalException(obj: null, arguments: Array.Empty<object>());
//                }

//                throw new NotSupportedException(
//                    $"Could not create a default parser for {typeof(T)}. The type must have a either a"
//                    + $" 'public static bool TryParse(string, out {typeof(T)})' method, a"
//                    + $" 'public static {typeof(T)} Parse(string)' method, or a 'public {typeof(T)}(string)' constructor"
//                );
//            }

//            private static TryParseCommandArgument<TEnum> CreateForEnum<TEnum>()
//            {
//                // this is the default behavior for enums. We do not leverage native enum parsing because
//                // we want to match (a) only defined values and (b) only text values. We ignore case by
//                // default on the assumption that .NET enums will be PascalCase but CLIs probably don't want
//                // that

//                var valuesByName = Enum.GetValues(typeof(TEnum))
//                    .Cast<TEnum>()
//                    .ToDictionary(t => t.ToString(), t => t, StringComparer.OrdinalIgnoreCase);

//                // TODO should accept first letter when unique, like a command

//                return delegate(string text, out TEnum parsed)
//                {
//                    if (text == null || !valuesByName.TryGetValue(text, out var value))
//                    {
//                        parsed = default;
//                        return false;
//                    }
//                    parsed = value;
//                    return true;
//                };
//            }
//        }

//        private static TryParseCommandArgument<T?> ForNullable<T>()
//            where T : struct
//        {
//            var underlyingParser = For<T>();
//            return delegate(string text, out T? parsed)
//            {
//                if (underlyingParser(text, out var underlyingParsed))
//                {
//                    parsed = underlyingParsed;
//                    return true;
//                }
//                parsed = null;
//                return false;
//            };
//        }
//    }
//#pragma warning restore SA1130
//}
