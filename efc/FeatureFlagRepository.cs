using efc.Models;
using Microsoft.EntityFrameworkCore;
using Db = efc.Database;

namespace efc;

internal class FeatureFlagRepository(Db.FeatureFlagsDbContext dbContext)
{
    internal async Task Save(Models.FeatureFlag featureFlagModel)
    {
        var featureFlagRecord = dbContext.ChangeTracker.Entries<Db.FeatureFlag>()
            .FirstOrDefault(e => e.Entity.Name == featureFlagModel.Name)?.Entity;
        featureFlagRecord ??= await dbContext.FeatureFlags
            .Include(f => f.UserAssignments)
            .ThenInclude(ua => ua.FeatureFlagUser)
            .FirstOrDefaultAsync(f => f.Name == featureFlagModel.Name);
        featureFlagRecord ??= dbContext.FeatureFlags.Add(new Db.FeatureFlag()).Entity;

        featureFlagRecord.Name = featureFlagModel.Name;
        featureFlagRecord.IsEnabled = featureFlagModel.IsEnabled;
        featureFlagRecord.CreatedOnUtc = featureFlagModel.CreatedOnUtc;
        featureFlagRecord.EnabledOn = featureFlagModel.EnabledOn;
        featureFlagRecord.UpdatedOnUtc = featureFlagModel.UpdatedOnUtc;

        var userRecords = await dbContext.FeatureFlagUsers.ToListAsync();

        // Update existing and add new user assignments
        foreach (var modelUserAssignment in featureFlagModel.UserAssignments)
        {
            var recordUser = userRecords.FirstOrDefault(u => u.UserName == modelUserAssignment.UserName);
            recordUser ??= dbContext.FeatureFlagUsers.Add(new Db.FeatureFlagUser { UserName = modelUserAssignment.UserName, CreatedOnUtc = modelUserAssignment.AssignedOnUtc }).Entity;
            
            var recordUserAssignment = featureFlagRecord.UserAssignments.FirstOrDefault(db => db.FeatureFlagUser.UserName == modelUserAssignment.UserName);
            recordUserAssignment ??= dbContext.FeatureFlagAssignments.Add(new Db.FeatureFlagUserAssignment { FeatureFlag = featureFlagRecord, FeatureFlagUser = recordUser }).Entity;

            recordUserAssignment.AssignedOnUtc = modelUserAssignment.AssignedOnUtc;
        }

        await dbContext.SaveChangesAsync();
    }

    internal async Task<FeatureFlagUser?> LoadUser(string username)
    {
        var userRecord = await dbContext.FeatureFlagUsers
            .Include(u => u.FeatureFlagAssignments)
            .ThenInclude(ua => ua.FeatureFlag)
            .FirstOrDefaultAsync(u => u.UserName == username);

        if (userRecord == null)
        {
            return null;
        }
        
        var featureFlagAssignments = userRecord.FeatureFlagAssignments
            .Select(ua => new FeatureFlagUserAssignment(ua.FeatureFlagUser.UserName, ua.AssignedOnUtc)).ToList();
        return new FeatureFlagUser(userRecord.UserName, userRecord.CreatedOnUtc, featureFlagAssignments);
    }

    internal async Task<FeatureFlag?> Load(string code)
    {
        var featureRecord = await dbContext.FeatureFlags
            .Include(f => f.UserAssignments)
            .ThenInclude(ua => ua.FeatureFlagUser)
            .FirstOrDefaultAsync(f => f.Name == code);

        if (featureRecord == null)
        {
            return null;
        }

        return new FeatureFlag(
            featureRecord.Name,
            featureRecord.IsEnabled,
            featureRecord.CreatedOnUtc,
            featureRecord.EnabledOn,
            featureRecord.UpdatedOnUtc,
            featureRecord.UserAssignments.Select(ua => new FeatureFlagUserAssignment(ua.FeatureFlagUser.UserName, ua.AssignedOnUtc)).ToList()
        );
    }

    internal async Task DeleteAll()
    {
        var featureFlags = await dbContext.FeatureFlags.Include(f => f.UserAssignments).ToListAsync();
        dbContext.FeatureFlags.RemoveRange(featureFlags);
        var featureFlagsUsers = await dbContext.FeatureFlagUsers.ToListAsync();
        dbContext.FeatureFlagUsers.RemoveRange(featureFlagsUsers);
        await dbContext.SaveChangesAsync();
    }

    internal async Task VerifyDeleted()
    {
        var countFeatureFlags = await dbContext.FeatureFlags.CountAsync();
        var countUserAssignments = await dbContext.FeatureFlagAssignments.CountAsync();
        var countUsers = await dbContext.FeatureFlagUsers.CountAsync();
        
        if (countFeatureFlags == 0 && countUserAssignments == 0 && countUsers == 0)
        {
            Console.WriteLine("All feature flags, user assignments and users have been deleted successfully.");
        }
        else
        {
            var ex = new InvalidOperationException($"Deletion failed.");
            ex.Data["FeatureFlagsCount"] = countFeatureFlags;
            ex.Data["UserAssignmentsCount"] = countUserAssignments; 
            ex.Data["UsersCount"] = countUsers;
            throw ex;
        }
    }
}