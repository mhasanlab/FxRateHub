using System;

namespace FxRateHub.Application.Common.Exceptions;

/// <summary>
/// Exception thrown when the daily rate limit has been exceeded.
/// </summary>
public class RateLimitExceededException : Exception
{
    /// <summary>
    /// Gets the daily limit for API calls.
    /// </summary>
    public int DailyLimit { get; }

    /// <summary>
    /// Gets the current usage count.
    /// </summary>
    public int CurrentUsage { get; }

    /// <summary>
    /// Creates a new RateLimitExceededException with the specified limits and usage.
    /// </summary>
    /// <param name="dailyLimit">The daily limit for API calls.</param>
    /// <param name="currentUsage">The current usage count.</param>
    public RateLimitExceededException(int dailyLimit, int currentUsage)
        : base($"Rate limit exceeded. Daily limit: {dailyLimit}, Current usage: {currentUsage}")
    {
        DailyLimit = dailyLimit;
        CurrentUsage = currentUsage;
    }

    /// <summary>
    /// Creates a new RateLimitExceededException with a custom message and limits.
    /// </summary>
    /// <param name="message">The custom error message.</param>
    /// <param name="dailyLimit">The daily limit for API calls.</param>
    /// <param name="currentUsage">The current usage count.</param>
    public RateLimitExceededException(string message, int dailyLimit, int currentUsage)
        : base(message)
    {
        DailyLimit = dailyLimit;
        CurrentUsage = currentUsage;
    }
}
