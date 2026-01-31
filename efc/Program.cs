using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var builder = Host.CreateDefaultBuilder(args);
        var host = builder.Build();
        var environment = host.Services.GetRequiredService<IHostEnvironment>();

        using var ffContext = new FeatureFlagsDbContext();

        if (environment.IsDevelopment())
        {
            await ffContext.Database.MigrateAsync();
        }
        else
        {
            /* In Staging & Production use in pipeline:
            ```
            dotnet tool install --global dotnet-ef
            ForEach ($dbcontext in (dotnet ef dbcontext list --no-build)) { dotnet ef database update --context $dbcontext }
            ```
            On your local machine use:
            ```
            $subdomain = "FeatureFlags"
            $migrationName = "Awesome"
            $dbcontext = $subdomain + "DbContext"
            dotnet ef migrations add $migrationName --context $dbcontext
            ```
            */
        }

        var feature = new FeatureFlag { Name = "OldFeature", IsEnabled = true };
        ffContext.FeatureFlags.Add(feature);
        await ffContext.SaveChangesAsync();

        var list = await ffContext.FeatureFlags
            .FromSql($"SELECT * FROM FeatureFlags where name like 'New%'")
            .ToListAsync();
        var count = await ffContext.FeatureFlags.CountAsync();
        Console.WriteLine($"Found: {list.Count}");
        Console.WriteLine($"Total FeatureFlags: {count}");
    }
}

public class FeatureFlagsDbContext : DbContext
{
    static readonly string connectionString = "Server=localhost;Database=ecommerce_db;User Id=root;Password=localhost;";

    public DbSet<FeatureFlag> FeatureFlags { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
    }
}

public class FeatureFlag
{
    [Key]
    public int Id { get; set; }
    public required string Name { get; set; }
    public bool IsEnabled { get; set; }
}