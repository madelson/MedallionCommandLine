using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Text;

namespace Medallion.CommandLine
{
    internal static class DefaultCommandArgumentParserFactory
    {
        public static CommandArgumentParser<TValue> Create<TValue>()
        {
            var type = typeof(TValue);

            // special-case nullables
            var underlyingType = Nullable.GetUnderlyingType(type);
            if (underlyingType != null)
            {
                return (CommandArgumentParser<TValue>)Activator.CreateInstance(typeof(NullableParser<>).MakeGenericType(underlyingType));
            }

            // special-case enums
            if (type.IsEnum)
            {
                return (CommandArgumentParser<TValue>)Activator.CreateInstance(typeof(EnumParser<>).MakeGenericType(type));
            }

            // try to find a parse method or string constructor
            var parseMembers = type.GetMembers()
                .Select(ParseMemberInfo.ForMember)
                .Where(i => i.Kind != ParseMemberKind.Invalid)
                .OrderBy(i => i.Kind)
                .ThenBy(i => i.DefaultParameterCount)
                .ToArray();
            if (parseMembers.Length == 0)
            {
                throw new InvalidOperationException(
                    $"Unabled to construct a default parser for {type}. The type is not an enum, or nullable reference type, and it lacks a static (Try)Parse method or string constructor"
                );
            }

            var bestParseMember = parseMembers[0];
            switch (bestParseMember.Kind)
            {
                case ParseMemberKind.TryParse:
                    return new TryParseMethodParser<TValue>((MethodInfo)bestParseMember.Member);
                case ParseMemberKind.Parse:
                    {
                        var parseMethod = (MethodInfo)bestParseMember.Member;
                        var defaultValues = parseMethod.GetParameters().Skip(1).Select(p => p.DefaultValue)
                            .ToArray();
                        return CommandArgumentParser.Create<TValue>(text =>
                        {
                            var arguments = new object[1 + bestParseMember.DefaultParameterCount];
                            arguments[0] = text;
                            for (var i = 0; i < defaultValues.Length; ++i)
                            {
                                arguments[i + 1] = defaultValues[i];
                            }
                            try { return (TValue)parseMethod.Invoke(obj: null, arguments); }
                            catch (TargetInvocationException tie)
                            {
                                ExceptionDispatchInfo.Capture(tie.InnerException).Throw();
                                throw; // will never get here
                        }
                        });
                    }
                case ParseMemberKind.Constructor:
                    {
                        var constructor = (ConstructorInfo)bestParseMember.Member;
                        var defaultValues = constructor.GetParameters().Skip(1).Select(p => p.DefaultValue)
                            .ToArray();
                        return CommandArgumentParser.Create<TValue>(text =>
                        {
                            var arguments = new object[1 + bestParseMember.DefaultParameterCount];
                            arguments[0] = text;
                            for (var i = 0; i < defaultValues.Length; ++i)
                            {
                                arguments[i + 1] = defaultValues[i];
                            }
                            try { return (TValue)constructor.Invoke(arguments); }
                            catch (TargetInvocationException tie)
                            {
                                ExceptionDispatchInfo.Capture(tie.InnerException).Throw();
                                throw; // will never get here
                            }
                        });
                    }
                default:
                    throw new InvalidOperationException("should never get here");
            }
        }

        private sealed class NullableParser<TValue> : SingleTokenParser<TValue?>
            where TValue : struct
        {
            protected override bool TryParse(string token, out TValue? parsed, out string errorMessage)
            {
                if (CommandArgumentParser<TValue>.Default.TryParse(new[] { token }, out var parsedValue, out errorMessage))
                {
                    parsed = parsedValue;
                    return true;
                }

                parsed = default;
                return false;
            }
        }

        private sealed class EnumParser<TEnum> : SingleTokenParser<TEnum>
            where TEnum : struct
        {
            protected override bool TryParse(string text, out TEnum parsed, out string errorMessage)
            {
                // explicitly disallow numeric strings, since that's likely not the intent
                if ((text.Length > 0 && (char.IsDigit(text[0]) || text[0] == '-'))
                    || !Enum.TryParse(text, ignoreCase: true, out parsed))
                {
                    var values = Enum.GetValues(typeof(TEnum)).Cast<object>().ToArray();
                    
                    if (typeof(TEnum).GetCustomAttributes(typeof(FlagsAttribute), inherit: false).Any())
                    {
                        var defaultValue = Activator.CreateInstance(typeof(TEnum));
                        errorMessage = $"must be{(values.Contains(default) ? $" {defaultValue} or" : string.Empty)} one of [{string.Join(", ", values.Where(v => !Equals(v, defaultValue)))}]";
                    }
                    else
                    {
                        errorMessage = $"must be one of [{string.Join(", ", values)}]";
                    }

                    parsed = default;
                    return false;
                }

                errorMessage = null;
                return true;
            }
        }

        private sealed class TryParseMethodParser<TValue> : SingleTokenParser<TValue>
        {
            private readonly MethodInfo _tryParseMethod;

            public TryParseMethodParser(MethodInfo tryParseMethod)
            {
                this._tryParseMethod = tryParseMethod;
            }

            protected override bool TryParse(string text, out TValue parsed, out string errorMessage)
            {
                var arguments = new object[] { text, null };
                var result = (bool)this._tryParseMethod.Invoke(obj: null, arguments);
                parsed = (TValue)arguments[1];
                errorMessage = null;
                return result;
            }
        }

        private enum ParseMemberKind
        {
            TryParse,
            Parse,
            Constructor,
            Invalid,
        }

        private struct ParseMemberInfo
        {
            public MemberInfo Member;
            public ParseMemberKind Kind;
            public int DefaultParameterCount;

            public static ParseMemberInfo ForMember(MemberInfo member)
            {
                ParameterInfo[] parameters;
                if (member is MethodInfo method && method.IsPublic && method.IsStatic)
                {
                    if (method.Name == "TryParse")
                    {
                        if (method.ReturnType == typeof(bool)
                            && (parameters = method.GetParameters()).Length == 2
                            && parameters[0].ParameterType == typeof(string)
                            && parameters[1].ParameterType == method.DeclaringType.MakeByRefType()
                            && parameters[1].Attributes == ParameterAttributes.Out)
                        {
                            return new ParseMemberInfo { Member = member, Kind = ParseMemberKind.TryParse, DefaultParameterCount = 0 };
                        }
                    }
                    else if (method.Name == "Parse")
                    {
                        if (method.ReturnType == method.DeclaringType
                            && (parameters = method.GetParameters()).Length > 0
                            && parameters[0].ParameterType == typeof(string)
                            && parameters.Skip(1).All(p => p.HasDefaultValue))
                        {
                            return new ParseMemberInfo { Member = member, Kind = ParseMemberKind.Parse, DefaultParameterCount = parameters.Length - 1 };
                        }
                    }
                }
                else if (member is ConstructorInfo constructor && constructor.IsPublic && !constructor.IsStatic)
                {
                    if ((parameters = constructor.GetParameters()).Length > 0
                        && parameters[0].ParameterType == typeof(string)
                        && parameters.Skip(1).All(p => p.HasDefaultValue))
                    {
                        return new ParseMemberInfo { Member = member, Kind = ParseMemberKind.Constructor, DefaultParameterCount = parameters.Length - 1 };
                    }
                }

                return new ParseMemberInfo { Member = member, Kind = ParseMemberKind.Invalid };
            }
        }
    }
}
