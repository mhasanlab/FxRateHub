using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FxRateHub.Domain.Common;
using FxRateHub.Domain.Enums;

namespace FxRateHub.Domain.Entities;

/// <summary>
/// Represents a synchronization log entry for tracking data synchronization operations.
/// </summary>
public class SyncLog
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; private set; }

    [Required]
    [MaxLength(50)]
    [Column(TypeName = "varchar(50)")]
    public string SyncType { get; private set; } = "FxRate";

    [Required]
    public SyncStatus Status { get; private set; }

    [Required]
    public int CurrenciesUpdated { get; private set; }

    [MaxLength(2000)]
    [Column(TypeName = "varchar(2000)")]
    public string? ErrorMessage { get; private set; }

    [Required]
    public DateTime StartedAt { get; private set; }

    public DateTime? CompletedAt { get; private set; }

    [Required]
    public DateTime CreatedAt { get; private set; }

    private SyncLog() { }

    /// <summary>
    /// Creates a new sync log with Started status.
    /// </summary>
    /// <param name="syncType">The type of synchronization (default: "FxRate").</param>
    /// <returns>A new SyncLog instance with Started status.</returns>
    public static SyncLog StartNew(string syncType = "FxRate")
    {
        return new SyncLog
        {
            SyncType = syncType,
            Status = SyncStatus.Started,
            CurrenciesUpdated = 0,
            StartedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Marks the sync as completed successfully.
    /// </summary>
    /// <param name="currenciesUpdated">The number of currencies updated.</param>
    public void Complete(int currenciesUpdated)
    {
        Status = SyncStatus.Completed;
        CurrenciesUpdated = currenciesUpdated;
        CompletedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Marks the sync as failed with an error message.
    /// </summary>
    /// <param name="errorMessage">The error message describing the failure.</param>
    public void Fail(string errorMessage)
    {
        Status = SyncStatus.Failed;
        ErrorMessage = errorMessage;
        CompletedAt = DateTime.UtcNow;
    }
}
