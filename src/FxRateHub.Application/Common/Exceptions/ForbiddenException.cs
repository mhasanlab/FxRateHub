using System;

namespace FxRateHub.Application.Common.Exceptions;

/// <summary>
/// Exception thrown when a user does not have permission to perform an action.
/// </summary>
public class ForbiddenException : Exception
{
    private const string DefaultMessage = "You do not have permission to perform this action.";

    /// <summary>
    /// Creates a new ForbiddenException with the default message.
    /// </summary>
    public ForbiddenException()
        : base(DefaultMessage)
    {
    }

    /// <summary>
    /// Creates a new ForbiddenException with a custom message.
    /// </summary>
    /// <param name="message">The custom error message.</param>
    public ForbiddenException(string message)
        : base(message)
    {
    }
}
