using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace efc.Subdomains.FeatureFlags;

public class FeatureFlagsDevelopmentMigrator
{
    private IHostEnvironment environment;
    private FeatureFlagsDbContext ffContext;

    public FeatureFlagsDevelopmentMigrator(IHostEnvironment environment)
    {
        this.environment = environment;
        ffContext = new FeatureFlagsDbContext();
    }

    public async Task Migrate()
    {
        if (!environment.IsDevelopment())
            return;
        
        await ffContext.Database.MigrateAsync();
        await Seed();
        
        /* In Staging & Production use in pipeline:
        ```
        dotnet tool install --global dotnet-ef
        ForEach ($dbcontext in (dotnet ef dbcontext list --no-build)) { dotnet ef database update --context $dbcontext }
        ```
        */
    }

    private async Task Seed()
    {
        if (await ffContext.FeatureFlags.CountAsync() == 0)
        {
            var feature1 = new FeatureFlag { Name = "NewFeature1", IsEnabled = true };
            var feature2 = new FeatureFlag { Name = "NewFeature2", IsEnabled = false };
            ffContext.FeatureFlags.AddRange(feature1, feature2);
            await ffContext.SaveChangesAsync();
        }
    }
}