using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FxRateHub.Domain.Entities;

namespace FxRateHub.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity configuration for the ApiKey entity.
/// </summary>
public class ApiKeyConfiguration : IEntityTypeConfiguration<ApiKey>
{
    /// <summary>
    /// Configures the ApiKey entity mapping.
    /// </summary>
    /// <param name="builder">The entity type builder.</param>
    public void Configure(EntityTypeBuilder<ApiKey> builder)
    {
        builder.ToTable("ApiKeys");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Id)
            .HasColumnName("Id")
            .ValueGeneratedOnAdd();

        builder.Property(a => a.UserId)
            .IsRequired();

        builder.Property(a => a.KeyHash)
            .IsRequired()
            .HasMaxLength(128)
            .HasColumnType("varchar(128)");

        builder.Property(a => a.KeyPrefix)
            .IsRequired()
            .HasMaxLength(16)
            .HasColumnType("varchar(16)");

        builder.Property(a => a.DailyRequestCount)
            .HasDefaultValue(0);

        builder.Property(a => a.TotalRequestCount)
            .HasDefaultValue(0);

        builder.Property(a => a.IsActive)
            .HasDefaultValue(true);

        builder.Property(a => a.CreatedAt)
            .IsRequired();

        builder.Property(a => a.UpdatedAt);

        // Configure relationship with User
        builder.HasOne(a => a.User)
            .WithOne(u => u.ApiKey)
            .HasForeignKey<ApiKey>(a => a.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure relationship with ApiUsageLogs
        builder.HasMany(a => a.UsageLogs)
            .WithOne(l => l.ApiKey)
            .HasForeignKey(l => l.ApiKeyId)
            .OnDelete(DeleteBehavior.Cascade);

        // Add unique index on KeyHash
        builder.HasIndex(a => a.KeyHash)
            .IsUnique();

        // Add unique index on UserId (one key per user)
        builder.HasIndex(a => a.UserId)
            .IsUnique();
    }
}
