using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FxRateHub.Application.Common.Interfaces;
using FxRateHub.Domain.Entities;

namespace FxRateHub.Infrastructure.Persistence;

/// <summary>
/// Entity Framework Core database context for the FxRateHub application.
/// </summary>
public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    /// <summary>
    /// Gets the DbSet for users.
    /// </summary>
    public DbSet<User> Users { get; private set; } = null!;

    /// <summary>
    /// Gets the DbSet for API keys.
    /// </summary>
    public DbSet<ApiKey> ApiKeys { get; private set; } = null!;

    /// <summary>
    /// Gets the DbSet for exchange rates.
    /// </summary>
    public DbSet<ExchangeRate> ExchangeRates { get; private set; } = null!;

    /// <summary>
    /// Gets the DbSet for exchange rate history.
    /// </summary>
    public DbSet<ExchangeRateHistory> ExchangeRateHistory { get; private set; } = null!;

    /// <summary>
    /// Gets the DbSet for API usage logs.
    /// </summary>
    public DbSet<ApiUsageLog> ApiUsageLogs { get; private set; } = null!;

    /// <summary>
    /// Gets the DbSet for application settings.
    /// </summary>
    public DbSet<AppSetting> AppSettings { get; private set; } = null!;

    /// <summary>
    /// Gets the DbSet for sync logs.
    /// </summary>
    public DbSet<SyncLog> SyncLogs { get; private set; } = null!;

    /// <summary>
    /// Creates a new instance of ApplicationDbContext with the specified options.
    /// </summary>
    /// <param name="options">The options to configure the database context.</param>
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Configures the database model using Fluent API.
    /// </summary>
    /// <param name="modelBuilder">The model builder to configure the entity mappings.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all entity configurations from this assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }

    /// <summary>
    /// Saves all changes made in this context to the database asynchronously.
    /// Automatically sets CreatedAt for new entities and UpdatedAt for modified entities.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous save operation. The task result contains the number of state entries written to the database.</returns>
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries();
        var utcNow = DateTime.UtcNow;

        foreach (var entry in entries)
        {
            if (entry.Entity is Domain.Common.BaseEntity baseEntity)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        baseEntity.CreatedAt = utcNow;
                        baseEntity.UpdatedAt = utcNow;
                        break;
                    case EntityState.Modified:
                        baseEntity.UpdatedAt = utcNow;
                        break;
                }
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}
