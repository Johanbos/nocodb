using efc.Subdomains.FeatureFlags;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace efc;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var builder = Host.CreateDefaultBuilder(args);
        var host = builder.Build();
        var environment = host.Services.GetRequiredService<IHostEnvironment>();

        var migrator = new FeatureFlagsDevelopmentMigrator(environment);
        await migrator.Migrate();
        
        using var ffContext = new FeatureFlagsDbContext();

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
