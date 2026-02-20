using System;
using System.Collections.Generic;
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

    /// <summary>
    /// Seeds exchange rates from the CurrencyFreaks API if no rates exist in the database.
    /// </summary>
    /// <param name="context">The application database context.</param>
    /// <param name="fxRateProvider">The FX rate provider service.</param>
    /// <param name="logger">The logger for seeding operations.</param>
    /// <returns>A task representing the asynchronous seeding operation.</returns>
    public static async Task SeedExchangeRatesAsync(
        ApplicationDbContext context,
        IFxRateProvider fxRateProvider,
        ILogger logger)
    {
        try
        {
            // Check if exchange rates already exist
            var existingRatesCount = await context.ExchangeRates.CountAsync();
            
            if (existingRatesCount > 0)
            {
                logger.LogInformation("Exchange rates already exist in database ({Count} rates). Skipping initial sync.", existingRatesCount);
                return;
            }

            logger.LogInformation("No exchange rates found in database. Performing initial sync from CurrencyFreaks API...");

            // Fetch rates from CurrencyFreaks API
            var rates = await fxRateProvider.GetLatestRatesAsync("USD");

            if (rates == null || !rates.Any())
            {
                logger.LogWarning("No rates received from CurrencyFreaks API during initial sync");
                return;
            }

            // Save rates to database
            var exchangeRates = new List<ExchangeRate>();
            foreach (var rate in rates)
            {
                var exchangeRate = ExchangeRate.Create("USD", rate.Key, rate.Value);
                exchangeRates.Add(exchangeRate);
            }

            await context.ExchangeRates.AddRangeAsync(exchangeRates);
            await context.SaveChangesAsync();

            // Create a sync log entry
            var syncLog = SyncLog.StartNew();
            syncLog.Complete(rates.Count);
            await context.SyncLogs.AddAsync(syncLog);
            await context.SaveChangesAsync();

            logger.LogInformation("Initial sync completed. {Count} exchange rates saved to database.", rates.Count);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred during initial exchange rate sync from CurrencyFreaks API");
            // Don't throw - allow the application to start even if initial sync fails
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
