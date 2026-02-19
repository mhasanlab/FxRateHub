using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace FxRateHub.Infrastructure.Persistence;

/// <summary>
/// Design-time factory for creating ApplicationDbContext instances.
/// This is used by EF Core migrations and the dotnet ef CLI tools.
/// </summary>
public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    /// <summary>
    /// Creates a new instance of ApplicationDbContext for use with migrations.
    /// </summary>
    /// <param name="args">Arguments passed from the command line.</param>
    /// <returns>A new ApplicationDbContext instance configured for migrations.</returns>
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        
        // Use the connection string from environment variable or default
        var connectionString = args.Length > 0 
            ? args[0] 
            : "Server=sql.bsite.net\\MSSQL2016;Database=fxrately_db;User Id=fxrately_db;Password=123456;TrustServerCertificate=True;";

        optionsBuilder.UseSqlServer(connectionString);

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}
