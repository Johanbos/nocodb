using Microsoft.EntityFrameworkCore;

namespace efc.Subdomains.FeatureFlags;

/// <summary>
/// Database context for Feature Flags. Update with:
/// ```
/// $subdomain = "FeatureFlags"
/// $migrationName = "Awesome"
/// $dbcontext = $subdomain + "DbContext"
/// dotnet ef migrations add $migrationName --context $dbcontext
/// ```
/// </summary>
public class FeatureFlagsDbContext : DbContext
{
    static readonly string connectionString = "Server=localhost;Database=backend-v2;User Id=root;Password=localhost;";

    public DbSet<FeatureFlag> FeatureFlags { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
    }
}
