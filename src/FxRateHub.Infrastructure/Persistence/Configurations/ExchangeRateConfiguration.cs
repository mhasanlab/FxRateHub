using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FxRateHub.Domain.Entities;

namespace FxRateHub.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity configuration for the ExchangeRate entity.
/// </summary>
public class ExchangeRateConfiguration : IEntityTypeConfiguration<ExchangeRate>
{
    /// <summary>
    /// Configures the ExchangeRate entity mapping.
    /// </summary>
    /// <param name="builder">The entity type builder.</param>
    public void Configure(EntityTypeBuilder<ExchangeRate> builder)
    {
        builder.ToTable("ExchangeRates");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasColumnName("Id")
            .ValueGeneratedOnAdd();

        builder.Property(e => e.BaseCurrency)
            .IsRequired()
            .HasMaxLength(3)
            .HasColumnType("nvarchar(3)")
            .HasDefaultValue("USD");

        builder.Property(e => e.TargetCurrency)
            .IsRequired()
            .HasMaxLength(3)
            .HasColumnType("nvarchar(3)");

        builder.Property(e => e.Rate)
            .HasColumnType("decimal(18,8)");

        builder.Property(e => e.UpdatedAt)
            .IsRequired();

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        // Add unique constraint on BaseCurrency + TargetCurrency combination
        builder.HasIndex(e => new { e.BaseCurrency, e.TargetCurrency })
            .IsUnique();
    }
}
