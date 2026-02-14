using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace efc.Database;

public class FeatureFlagsDevelopmentMigrator(IHostEnvironment environment)
{
    private IHostEnvironment environment = environment;
    private FeatureFlagsDbContext ffContext = new FeatureFlagsDbContext();

    public async Task Migrate()
    {
        if (!environment.IsDevelopment())
            return;
        
        await ffContext.Database.MigrateAsync();
    }

}