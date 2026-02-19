using System;

namespace FxRateHub.Application.Common.Exceptions;

/// <summary>
/// Exception thrown when a requested resource is not found.
/// </summary>
public class NotFoundException : Exception
{
    /// <summary>
    /// Creates a new NotFoundException with the specified message.
    /// </summary>
    /// <param name="message">The error message.</param>
    public NotFoundException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Creates a new NotFoundException for the specified entity and key.
    /// </summary>
    /// <param name="name">The name of the entity that was not found.</param>
    /// <param name="key">The key of the entity that was not found.</param>
    public NotFoundException(string name, object key)
        : base($"Entity \"{name}\" with key ({key}) was not found.")
    {
    }
}
