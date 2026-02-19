using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FxRateHub.Domain.Entities;

namespace FxRateHub.Application.Common.Interfaces;

/// <summary>
/// Defines the contract for the application database context.
/// </summary>
public interface IApplicationDbContext : IDisposable, IAsyncDisposable
{
    /// <summary>
    /// Gets the DbSet for users.
    /// </summary>
    DbSet<User> Users { get; }

    /// <summary>
    /// Gets the DbSet for API keys.
    /// </summary>
    DbSet<ApiKey> ApiKeys { get; }

    /// <summary>
    /// Gets the DbSet for exchange rates.
    /// </summary>
    DbSet<ExchangeRate> ExchangeRates { get; }

    /// <summary>
    /// Gets the DbSet for exchange rate history.
    /// </summary>
    DbSet<ExchangeRateHistory> ExchangeRateHistory { get; }

    /// <summary>
    /// Gets the DbSet for API usage logs.
    /// </summary>
    DbSet<ApiUsageLog> ApiUsageLogs { get; }

    /// <summary>
    /// Gets the DbSet for application settings.
    /// </summary>
    DbSet<AppSetting> AppSettings { get; }

    /// <summary>
    /// Gets the DbSet for sync logs.
    /// </summary>
    DbSet<SyncLog> SyncLogs { get; }

    /// <summary>
    /// Saves all changes made in this context to the database.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous save operation. The task result contains the number of state entries written to the database.</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
