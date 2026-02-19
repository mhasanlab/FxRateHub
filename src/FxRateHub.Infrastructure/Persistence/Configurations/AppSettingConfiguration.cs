using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FxRateHub.Domain.Entities;

namespace FxRateHub.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity configuration for the AppSetting entity.
/// </summary>
public class AppSettingConfiguration : IEntityTypeConfiguration<AppSetting>
{
    /// <summary>
    /// Configures the AppSetting entity mapping.
    /// </summary>
    /// <param name="builder">The entity type builder.</param>
    public void Configure(EntityTypeBuilder<AppSetting> builder)
    {
        builder.ToTable("AppSettings");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Id)
            .HasColumnName("Id")
            .ValueGeneratedOnAdd();

        builder.Property(a => a.SettingKey)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnType("varchar(100)");

        builder.Property(a => a.SettingValue)
            .IsRequired()
            .HasMaxLength(500)
            .HasColumnType("varchar(500)");

        builder.Property(a => a.Description)
            .HasMaxLength(500)
            .HasColumnType("varchar(500)");

        builder.Property(a => a.UpdatedAt)
            .IsRequired();

        builder.Property(a => a.CreatedAt)
            .IsRequired();

        // Add unique index on SettingKey
        builder.HasIndex(a => a.SettingKey)
            .IsUnique();
    }
}
