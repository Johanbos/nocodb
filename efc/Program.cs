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

        await Save("New Feature", "user1");
        var result = await Load("New Feature", "user1");
        await Validate(result.Item1, result.Item2);
    }

    private static async Task<(FeatureFlag?, FeatureFlagUser?)> Load(string code, string username)
    {
        using var ffContext = new Db.FeatureFlagsDbContext();
        var featureFlagRepository = new FeatureFlagRepository(ffContext);
        var featureflag = await featureFlagRepository.Load(code);
        var user1 = await featureFlagRepository.LoadUser(username);
        return (featureflag, user1);
    }

    private static async Task Save(string code, string username)
    {
        using var ffContext = new Db.FeatureFlagsDbContext();
        var featureFlagRepository = new FeatureFlagRepository(ffContext);

        var feature = new FeatureFlag(code, true, DateTime.UtcNow, null, null,
        [
            new(username, DateTime.UtcNow)
        ]);

        await featureFlagRepository.Save(feature);
    }

    private static async Task Validate(FeatureFlag? feature, FeatureFlagUser? user1)
    {
        if (feature == null) throw new InvalidOperationException("Feature flag not found.");
        if (user1 == null) throw new InvalidOperationException("User not found.");

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