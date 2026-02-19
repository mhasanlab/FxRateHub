using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FxRateHub.Domain.Common;

namespace FxRateHub.Domain.Entities;

/// <summary>
/// Represents an API key entity for authenticating and tracking API usage.
/// </summary>
public class ApiKey : BaseEntity<Guid>
{
    /// <summary>
    /// The ID of the user who owns this API key.
    /// </summary>
    [Required]
    public Guid UserId { get; set; }

    /// <summary>
    /// The SHA256 hash of the API key.
    /// </summary>
    [Required]
    [MaxLength(128)]
    [Column(TypeName = "varchar(128)")]
    public string KeyHash { get; set; } = string.Empty;

    /// <summary>
    /// The first 12 characters of the API key with ellipsis for display purposes.
    /// </summary>
    [Required]
    [MaxLength(16)]
    [Column(TypeName = "varchar(16)")]
    public string KeyPrefix { get; set; } = string.Empty;

    /// <summary>
    /// The number of requests made today.
    /// </summary>
    public int DailyRequestCount { get; set; }

    /// <summary>
    /// The total number of requests made with this API key.
    /// </summary>
    public long TotalRequestCount { get; set; }

    /// <summary>
    /// The date of the last request made.
    /// </summary>
    public DateTime? LastRequestDate { get; set; }

    /// <summary>
    /// Indicates whether this API key is active.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Navigation property to the associated user.
    /// </summary>
    public User User { get; set; } = null!;

    /// <summary>
    /// Collection of usage logs for this API key.
    /// </summary>
    public ICollection<ApiUsageLog> UsageLogs { get; set; } = new List<ApiUsageLog>();

    /// <summary>
    /// Creates a new API key with the specified parameters and default values.
    /// </summary>
    /// <param name="userId">The ID of the user who owns this API key.</param>
    /// <param name="keyHash">The SHA256 hash of the API key.</param>
    /// <param name="keyPrefix">The display prefix for the API key.</param>
    /// <returns>A new ApiKey instance configured with the provided values.</returns>
    public static ApiKey Create(Guid userId, string keyHash, string keyPrefix)
    {
        return new ApiKey
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            KeyHash = keyHash,
            KeyPrefix = keyPrefix,
            DailyRequestCount = 0,
            TotalRequestCount = 0,
            LastRequestDate = null,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Increments both daily and total request counts; resets daily count if it's a new day.
    /// </summary>
    public void IncrementUsage()
    {
        var today = DateTime.UtcNow.Date;

        if (LastRequestDate == null || LastRequestDate.Value.Date != today)
        {
            DailyRequestCount = 0;
        }

        DailyRequestCount++;
        TotalRequestCount++;
        LastRequestDate = DateTime.UtcNow;
    }

    /// <summary>
    /// Checks if the API key has exceeded the daily request limit.
    /// </summary>
    /// <param name="dailyLimit">The maximum number of requests allowed per day.</param>
    /// <returns>True if DailyRequestCount is greater than or equal to dailyLimit; otherwise, false.</returns>
    public bool HasExceededDailyLimit(int dailyLimit)
    {
        return DailyRequestCount >= dailyLimit;
    }

    /// <summary>
    /// Resets the daily request count to zero and updates the last request date to current UTC time.
    /// </summary>
    public void ResetDailyCount()
    {
        DailyRequestCount = 0;
        LastRequestDate = DateTime.UtcNow;
    }
}
