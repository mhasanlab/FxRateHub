using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FxRateHub.Domain.Entities;

namespace FxRateHub.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity configuration for the ApiUsageLog entity.
/// </summary>
public class ApiUsageLogConfiguration : IEntityTypeConfiguration<ApiUsageLog>
{
    /// <summary>
    /// Configures the ApiUsageLog entity mapping.
    /// </summary>
    /// <param name="builder">The entity type builder.</param>
    public void Configure(EntityTypeBuilder<ApiUsageLog> builder)
    {
        builder.ToTable("ApiUsageLogs");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Id)
            .HasColumnName("Id")
            .ValueGeneratedOnAdd();

        builder.Property(a => a.ApiKeyId)
            .IsRequired();

        builder.Property(a => a.Endpoint)
            .IsRequired()
            .HasMaxLength(200)
            .HasColumnType("varchar(200)");

        builder.Property(a => a.HttpMethod)
            .IsRequired()
            .HasMaxLength(10)
            .HasColumnType("varchar(10)");

        builder.Property(a => a.ResponseStatusCode)
            .IsRequired();

        builder.Property(a => a.RequestedAt)
            .IsRequired();

        builder.Property(a => a.IpAddress)
            .HasMaxLength(50)
            .HasColumnType("varchar(50)");

        builder.Property(a => a.CreatedAt)
            .IsRequired();

        // Configure relationship with ApiKey
        builder.HasOne(a => a.ApiKey)
            .WithMany(k => k.UsageLogs)
            .HasForeignKey(a => a.ApiKeyId)
            .OnDelete(DeleteBehavior.Cascade);

        // Add indexes for efficient querying
        builder.HasIndex(a => a.ApiKeyId);
        builder.HasIndex(a => a.RequestedAt);
        builder.HasIndex(a => new { a.ApiKeyId, a.RequestedAt });
    }
}
