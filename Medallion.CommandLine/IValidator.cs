using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Medallion.CommandLine
{
    public interface IValidator<in TValue>
    {
        string Description { get; }

        string[] GetValidationErrors(TValue value);
    }

    public static class Validator
    {
        public static IValidator<TValue> Create<TValue>(Func<TValue, string[]> getValidationErrors, string description = null)
        {
            return new FuncValidator<TValue>(
                getValidationErrors ?? throw new ArgumentNullException(nameof(getValidationErrors)),
                description
            );
        }

        public static IValidator<TValue> Create<TValue>(Func<TValue, string> getValidationErrorOrDefault, string description = null)
        {
            if (getValidationErrorOrDefault == null) { throw new ArgumentNullException(nameof(getValidationErrorOrDefault)); }

            return new FuncValidator<TValue>(
                v => getValidationErrorOrDefault(v) is string error ? new[] { error } : Array.Empty<string>(),
                description
            );
        }

        public static IValidator<TValue> Create<TValue>(Action<TValue> throwValidationError, string description = null)
        {
            if (throwValidationError == null) { throw new ArgumentNullException(nameof(throwValidationError)); }

            return new FuncValidator<TValue>(
                v =>
                {
                    try { throwValidationError(v); }
                    catch (Exception ex)
                    {
                        if (ex is AggregateException aggregate)
                        {
                            return aggregate.Flatten().InnerExceptions
                                .Select(e => e.Message)
                                .ToArray();
                        }

                        return new[] { ex.Message };
                    }

                    return Array.Empty<string>();
                },
                description
            );
        }

        public static IValidator<TValue> Min<TValue>(TValue min, IComparer<TValue> comparer = null, bool exclusive = false, string message = null)
        {
            var condition = $"{(exclusive ? ">" : ">=")} {ToString(min)}";
            return Create<TValue>(
                v => ((comparer ?? Comparer<TValue>.Default).Compare(v, min) >= (exclusive ? 1 : 0))
                    ? null
                    : message ?? $"must be {condition}",
                $"be {condition}"
            );
        }

        public static IValidator<TValue> Max<TValue>(TValue max, IComparer<TValue> comparer = null, bool exclusive = false, string message = null)
        {
            var condition = $"{(exclusive ? "<" : "<=")} {ToString(max)}";
            return Create<TValue>(
                v => ((comparer ?? Comparer<TValue>.Default).Compare(v, max) <= (exclusive ? -1 : 0))
                    ? null
                    : message ?? $"must be {condition}",
                $"be {condition}"
            );
        }

        private static IValidator<FileSystemInfo> _cachedExists;

        public static IValidator<FileSystemInfo> Exists => _cachedExists
            ?? (
                _cachedExists =
                Create<FileSystemInfo>(
                    value => value.Exists ? null : $"{(value is FileInfo ? "file" : "directory")} does not exist",
                    "exist"
                )
           );

        public static IValidator<string> Matches(Regex regex, string errorMessage = null, string description = null)
        {
            if (regex == null) { throw new ArgumentNullException(nameof(regex)); }

            return Create<string>(
                v => regex.IsMatch(v) ? null : errorMessage ?? $"does not match /{regex}/",
                description: description ?? $"match /{regex}/"
            );
        }

        private static IValidator<string> _cachedIsNotEmpty;

        public static IValidator<string> IsNotEmpty => _cachedIsNotEmpty
            ?? (
                _cachedIsNotEmpty =
                Create<string>(
                    value => value == string.Empty ? "must not be the empty string" : null,
                    "not be the empty string"
                )
            );

        public static IValidator<TValue> And<TValue>(this IValidator<TValue> first, IValidator<TValue> second, bool shortCircuit = false)
        {
            if (first == null) { throw new ArgumentNullException(nameof(first)); }
            if (second == null) { throw new ArgumentNullException(nameof(second)); }

            return new FuncValidator<TValue>(
                v =>
                {
                    var firstErrors = first.GetValidationErrorsChecked(v);
                    return shortCircuit && firstErrors.Length > 0
                        ? firstErrors
                        : firstErrors.Concat(second.GetValidationErrorsChecked(v)).ToArray();
                },
                first.Description != null && second.Description != null
                    ? $"{first.Description} and {second.Description}"
                    : first.Description ?? second.Description
            );
        }

        internal static ObjectValidator ToObjectValidator<TValue>(this IValidator<TValue> validator)
        {
            return validator is ObjectValidator objectValidator ? objectValidator : new ObjectValidator<TValue>(validator);
        }

        internal static string[] GetValidationErrorsChecked<TValue>(this IValidator<TValue> validator, TValue value)
        {
            return validator.GetValidationErrors(value)
                ?? throw new InvalidOperationException($"Validator{(validator.Description != null ? $" '{validator.Description}'" : string.Empty)} returned null; expected {typeof(string[])}");
        }

        internal static IValidator<TValue> Combine<TValue>(IReadOnlyList<IValidator<TValue>> validators)
        {
            return new FuncValidator<TValue>(
                value => validators.SelectMany(v => v.GetValidationErrorsChecked(value)).ToArray(),
                validators.Select(v => v.Description)
                    .Aggregate(default(StringBuilder), (builder, description) => description == null ? builder : builder == null ? new StringBuilder(description) : builder.Append(" and ").Append(description))
                    ?.ToString()
            );
        }

        internal static IValidator<ReadOnlyCollection<TValue>> FromElementValidator<TValue>(IValidator<TValue> elementValidator)
        {
            if (elementValidator == null) { throw new ArgumentNullException(nameof(elementValidator)); }

            return new FuncValidator<ReadOnlyCollection<TValue>>(
                collection =>
                {
                    for (var i = 0; i < collection.Count; ++i)
                    {
                        var errorMessages = elementValidator.GetValidationErrorsChecked(collection[i]);
                        if (errorMessages.Length > 0)
                        {
                            return errorMessages;
                        }
                    }
                    return Array.Empty<string>();
                },
                elementValidator.Description != null ? $"all {elementValidator.Description}" : null
            );
        }

        private static string ToString(object value)
        {
            switch (value?.ToString())
            {
                case null: return "null";
                case "": return "''";
                case string s: return s;
            }
        }

        private class FuncValidator<TValue> : IValidator<TValue>
        {
            private readonly Func<TValue, string[]> _getValidationErrors;

            public FuncValidator(Func<TValue, string[]> getValidationErrors, string description)
            {
                this._getValidationErrors = getValidationErrors;
                this.Description = description;
            }

            public string Description { get; }

            public string[] GetValidationErrors(TValue value) => this._getValidationErrors(value);
        }
    }

    internal abstract class ObjectValidator
    {
        public abstract string Description { get; }
        public abstract string[] GetValidationErrorsChecked(object value);
    }

    internal class ObjectValidator<TValue> : ObjectValidator, IValidator<TValue>
    {
        private readonly IValidator<TValue> _validator;

        public ObjectValidator(IValidator<TValue> validator)
        {
            this._validator = validator ?? throw new ArgumentNullException(nameof(validator));
        }

        public override string Description => this._validator.Description;

        public string[] GetValidationErrors(TValue value) => this._validator.GetValidationErrors(value);
        public override string[] GetValidationErrorsChecked(object value) => Validator.GetValidationErrorsChecked(this, (TValue)value);
    }
}
