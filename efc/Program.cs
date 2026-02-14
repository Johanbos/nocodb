using System.Text.Json;
using efc.Database;
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

        var migratorFF = new FeatureFlagsDevelopmentMigrator(environment);
        await migratorFF.Migrate();

        using var ffContext = new FeatureFlagsDbContext();

        await ffContext.Database.BeginTransactionAsync();

        var featureFlagRepository = new FeatureFlagRepository(ffContext);

        var user1 = new FeatureFlagUser { UserName = "user1", CreatedOnUtc = DateTime.UtcNow };
        var user2 = new FeatureFlagUser { UserName = "user2", CreatedOnUtc = DateTime.UtcNow };
        var feature = new FeatureFlag { Name = "Feature X", IsEnabled = false, CreatedOnUtc = DateTime.UtcNow, UserAssignments =
        [
            new() { FeatureFlagUser = user1, AssignedOnUtc = DateTime.UtcNow },
            new() { FeatureFlagUser = user2, AssignedOnUtc = DateTime.UtcNow }
        ]};
        ffContext.FeatureFlags.Add(feature);
        await ffContext.SaveChangesAsync();
        await ffContext.Database.CommitTransactionAsync();

        Console.WriteLine("Feature Flag with User Assignments:");
        Console.WriteLine(JsonSerializer.Serialize(feature, new JsonSerializerOptions { 
            WriteIndented = true, 
            ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve 
        }));

        Console.WriteLine("Feature Flag User 1 with User Assignments:");
        Console.WriteLine(JsonSerializer.Serialize(user1, new JsonSerializerOptions { 
            WriteIndented = true, 
            ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve 
        }));

        var featureFlags = await ffContext.FeatureFlags.Include(f => f.UserAssignments).ToListAsync();
        ffContext.FeatureFlags.RemoveRange(featureFlags);
        var featureFlagsUsers = await ffContext.FeatureFlagUsers.ToListAsync();
        ffContext.FeatureFlagUsers.RemoveRange(featureFlagsUsers);
        await ffContext.SaveChangesAsync();
        var countFeatureFlags = await ffContext.FeatureFlags.CountAsync();
        var countUserAssignments = await ffContext.FeatureFlagAssignments.CountAsync();
        var countUsers = await ffContext.FeatureFlagUsers.CountAsync();
        Console.WriteLine($"Total FeatureFlags: {countFeatureFlags}");
        Console.WriteLine($"Total User Assignments: {countUserAssignments}");
        Console.WriteLine($"Total Users: {countUsers}");
    }
}

internal class FeatureFlagRepository
{
    private FeatureFlagsDbContext ffContext;

    public FeatureFlagRepository(FeatureFlagsDbContext ffContext)
    {
        this.ffContext = ffContext;
    }
}