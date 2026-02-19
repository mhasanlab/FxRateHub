namespace FxRateHub.Domain.Enums;

/// <summary>
/// Represents the status of a synchronization operation.
/// </summary>
public enum SyncStatus
{
    /// <summary>
    /// Synchronization has started.
    /// </summary>
    Started = 0,
    
    /// <summary>
    /// Synchronization has completed successfully.
    /// </summary>
    Completed = 1,
    
    /// <summary>
    /// Synchronization has failed.
    /// </summary>
    Failed = 2
}
