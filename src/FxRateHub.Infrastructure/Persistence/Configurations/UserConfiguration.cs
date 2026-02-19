using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FxRateHub.Domain.Entities;

namespace FxRateHub.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity configuration for the User entity.
/// </summary>
public class UserConfiguration : IEntityTypeConfiguration<User>
{
    /// <summary>
    /// Configures the User entity mapping.
    /// </summary>
    /// <param name="builder">The entity type builder.</param>
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id)
            .HasColumnName("Id")
            .ValueGeneratedOnAdd();

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(255)
            .HasColumnType("varchar(255)");

        builder.Property(u => u.PasswordHash)
            .HasMaxLength(500);

        builder.Property(u => u.FullName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(u => u.Role)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(u => u.AuthProvider)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(u => u.GoogleId)
            .HasMaxLength(255);

        builder.Property(u => u.IsBlocked)
            .HasDefaultValue(false);

        builder.Property(u => u.CreatedAt)
            .IsRequired();

        builder.Property(u => u.UpdatedAt);

        // Add unique index on Email
        builder.HasIndex(u => u.Email)
            .IsUnique();

        // Add index on GoogleId
        builder.HasIndex(u => u.GoogleId);

        // Configure one-to-one relationship with ApiKey (cascade delete)
        builder.HasOne(u => u.ApiKey)
            .WithOne(a => a.User)
            .HasForeignKey<ApiKey>(a => a.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
