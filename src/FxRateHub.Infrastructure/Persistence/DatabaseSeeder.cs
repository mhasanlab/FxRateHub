using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using FxRateHub.Application.Common.Interfaces;
using FxRateHub.Domain.Entities;
using FxRateHub.Domain.Enums;

namespace FxRateHub.Infrastructure.Persistence;

/// <summary>
/// Provides database seeding functionality for initial data population.
/// </summary>
public static class DatabaseSeeder
{
    private const string AdminEmail = "admin@fxratehub.com";
    private const string AdminPassword = "Admin@123!";
    private const string AdminFullName = "System Administrator";

    /// <summary>
    /// Seeds the database with initial data if records don't already exist.
    /// </summary>
    /// <param name="context">The application database context.</param>
    /// <param name="passwordHasher">The password hashing service.</param>
    /// <param name="logger">The logger for database seeding operations.</param>
    /// <returns>A task representing the asynchronous seeding operation.</returns>
    public static async Task SeedAsync(
        ApplicationDbContext context,
        IPasswordHasher passwordHasher,
        ILogger logger)
    {
        var seededCount = 0;

        try
        {
            // Seed admin user
            var adminSeeded = await SeedAdminUserAsync(context, passwordHasher, logger);
            if (adminSeeded)
            {
                seededCount++;
            }

            // Seed app settings
            var settingsSeeded = await SeedAppSettingsAsync(context, logger);
            seededCount += settingsSeeded;

            if (seededCount > 0)
            {
                await context.SaveChangesAsync();
                logger.LogInformation("Seeded {Count} item(s) to the database", seededCount);
            }
            else
            {
                logger.LogInformation("Database seeding skipped - all records already exist");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the database");
            throw;
        }
    }

    private static async Task<bool> SeedAdminUserAsync(
        ApplicationDbContext context,
        IPasswordHasher passwordHasher,
        ILogger logger)
    {
        // Check if admin user already exists
        var existingAdmin = await context.Users
            .AnyAsync(u => u.Email == AdminEmail.ToLowerInvariant());

        if (existingAdmin)
        {
            logger.LogInformation("Admin user with email {Email} already exists", AdminEmail);
            return false;
        }

        // Hash the admin password
        var hashedPassword = passwordHasher.Hash(AdminPassword);

        // Create admin user
        var adminUser = User.CreateAdmin(
            email: AdminEmail,
            passwordHash: hashedPassword,
            fullName: AdminFullName);

        await context.Users.AddAsync(adminUser);
        logger.LogInformation("Admin user with email {Email} will be seeded", AdminEmail);

        return true;
    }

    private static async Task<int> SeedAppSettingsAsync(
        ApplicationDbContext context,
        ILogger logger)
    {
        var seededCount = 0;
        var settingsToSeed = new (string Key, string Value, string Description)[]
        {
            ("SyncIntervalMinutes", "60", "Interval in minutes for FX rate sync"),
            ("DailyRequestLimit", "24", "Maximum API requests per day per key"),
            ("MaintenanceMode", "false", "Enable/Disable maintenance mode")
        };

        foreach (var (key, value, description) in settingsToSeed)
        {
            // Check if setting already exists
            var existingSetting = await context.AppSettings
                .AnyAsync(s => s.SettingKey == key);

            if (existingSetting)
            {
                logger.LogInformation("App setting with key {Key} already exists", key);
                continue;
            }

            // Create and add the setting
            var setting = AppSetting.Create(key, value, description);
            await context.AppSettings.AddAsync(setting);
            seededCount++;
            logger.LogInformation("App setting {Key} will be seeded", key);
        }

        return seededCount;
    }
}
