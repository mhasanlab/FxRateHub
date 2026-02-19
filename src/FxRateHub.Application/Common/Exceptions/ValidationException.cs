using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation.Results;

namespace FxRateHub.Application.Common.Exceptions;

/// <summary>
/// Exception thrown when validation fails.
/// </summary>
public class ValidationException : Exception
{
    /// <summary>
    /// Gets the dictionary of validation errors, keyed by property name.
    /// </summary>
    public IDictionary<string, string[]> Errors { get; } = new Dictionary<string, string[]>();

    /// <summary>
    /// Creates a new ValidationException from a collection of ValidationFailures.
    /// </summary>
    /// <param name="failures">The collection of validation failures.</param>
    public ValidationException(IEnumerable<ValidationFailure> failures)
        : base("One or more validation errors occurred.")
    {
        Errors = failures
            .GroupBy(failure => failure.PropertyName)
            .ToDictionary(
                group => group.Key,
                group => group.Select(failure => failure.ErrorMessage).ToArray());
    }
}
