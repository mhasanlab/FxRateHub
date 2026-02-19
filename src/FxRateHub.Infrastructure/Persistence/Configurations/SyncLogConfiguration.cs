using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FxRateHub.Domain.Entities;
using FxRateHub.Domain.Enums;

namespace FxRateHub.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity configuration for the SyncLog entity.
/// </summary>
public class SyncLogConfiguration : IEntityTypeConfiguration<SyncLog>
{
    /// <summary>
    /// Configures the SyncLog entity mapping.
    /// </summary>
    /// <param name="builder">The entity type builder.</param>
    public void Configure(EntityTypeBuilder<SyncLog> builder)
    {
        builder.ToTable("SyncLogs");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Id)
            .HasColumnName("Id")
            .ValueGeneratedOnAdd();

        builder.Property(s => s.SyncType)
            .IsRequired()
            .HasMaxLength(50)
            .HasColumnType("varchar(50)")
            .HasDefaultValue("FxRate");

        builder.Property(s => s.Status)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(s => s.CurrenciesUpdated)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(s => s.ErrorMessage)
            .HasMaxLength(2000)
            .HasColumnType("varchar(2000)");

        builder.Property(s => s.StartedAt)
            .IsRequired();

        builder.Property(s => s.CompletedAt);

        builder.Property(s => s.CreatedAt)
            .IsRequired();

        // Add indexes for efficient querying
        builder.HasIndex(s => s.SyncType);
        builder.HasIndex(s => s.Status);
        builder.HasIndex(s => s.StartedAt);
        builder.HasIndex(s => new { s.SyncType, s.StartedAt });
    }
}
