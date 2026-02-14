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
        ffContext.FeatureFlagUsers.AddRange(user1, user2);
        await ffContext.SaveChangesAsync();

        var feature = new FeatureFlag { Name = "Feature X", IsEnabled = false, CreatedOnUtc = DateTime.UtcNow, UserAssignments = new List<FeatureFlagUserAssignment>
        {
            new() { FeatureFlagUserId = user1.Id, AssignedOnUtc = DateTime.UtcNow },
            new() { FeatureFlagUserId = user2.Id, AssignedOnUtc = DateTime.UtcNow }
        } };
        ffContext.FeatureFlags.Add(feature);
        await ffContext.SaveChangesAsync();
        await ffContext.Database.CommitTransactionAsync();
        var list = await ffContext.FeatureFlags.Include(f => f.UserAssignments).ToListAsync();
        Console.WriteLine("Feature Flags with User Assignments:");
        Console.WriteLine(JsonSerializer.Serialize(list, new JsonSerializerOptions { WriteIndented = true }));

        ffContext.FeatureFlags.RemoveRange(list);
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