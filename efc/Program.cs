using System.Text.Json;
using efc.Models;
using Db = efc.Database;
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

        var migratorFF = new Db.FeatureFlagsDevelopmentMigrator(environment);
        await migratorFF.Migrate();

        await Save();
        var result = await Load();
        await Validate(result.Item1, result.Item2);
    }

    private static async Task<(FeatureFlag, FeatureFlagUser)> Load()
    {
        throw new NotImplementedException();
    }

    private static async Task Save()
    {
        using var ffContext = new Db.FeatureFlagsDbContext();
        var featureFlagRepository = new FeatureFlagRepository(ffContext);

        var user1 = new FeatureFlagUser("user1", DateTime.UtcNow);
        var user2 = new FeatureFlagUser("user2", DateTime.UtcNow);
        var feature = new FeatureFlag("New Feature", true, DateTime.UtcNow, null, null,
        [
            new(user1, DateTime.UtcNow ),
            new(user2, DateTime.UtcNow )
        ]);
        feature.UserAssignments.ForEach(assignment => assignment.FeatureFlag = feature);
        user1.FeatureFlagAssignments.AddRange(feature.UserAssignments.Where(assignment => assignment.FeatureFlagUser == user1));
        user2.FeatureFlagAssignments.AddRange(feature.UserAssignments.Where(assignment => assignment.FeatureFlagUser == user2));

        await featureFlagRepository.Save(feature);
    }

    private static async Task Validate(FeatureFlag feature, FeatureFlagUser user1)
    {
        using var ffContext = new Db.FeatureFlagsDbContext();
        var featureFlagRepository = new FeatureFlagRepository(ffContext);

        var jsonSerializerOptions = new JsonSerializerOptions()
        {
            WriteIndented = true,
            ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve
        };

        Console.WriteLine("Feature Flag with User Assignments:");
        Console.WriteLine(JsonSerializer.Serialize(feature, jsonSerializerOptions));

        Console.WriteLine("Feature Flag User 1 with User Assignments:");
        Console.WriteLine(JsonSerializer.Serialize(user1, jsonSerializerOptions));

        await featureFlagRepository.DeleteAll();
        await featureFlagRepository.VerifyDeleted();
    }
}