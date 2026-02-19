using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FxRateHub.Domain.Entities;

namespace FxRateHub.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity configuration for the ExchangeRateHistory entity.
/// </summary>
public class ExchangeRateHistoryConfiguration : IEntityTypeConfiguration<ExchangeRateHistory>
{
    /// <summary>
    /// Configures the ExchangeRateHistory entity mapping.
    /// </summary>
    /// <param name="builder">The entity type builder.</param>
    public void Configure(EntityTypeBuilder<ExchangeRateHistory> builder)
    {
        builder.ToTable("ExchangeRateHistory");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasColumnName("Id")
            .ValueGeneratedOnAdd();

        builder.Property(e => e.BaseCurrency)
            .IsRequired()
            .HasMaxLength(3)
            .HasColumnType("nvarchar(3)");

        builder.Property(e => e.TargetCurrency)
            .IsRequired()
            .HasMaxLength(3)
            .HasColumnType("nvarchar(3)");

        builder.Property(e => e.Rate)
            .HasPrecision(18, 8);

        builder.Property(e => e.RecordedAt)
            .IsRequired();

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        // Add indexes for efficient querying
        builder.HasIndex(e => new { e.BaseCurrency, e.TargetCurrency, e.RecordedAt });
        builder.HasIndex(e => e.RecordedAt);
    }
}
