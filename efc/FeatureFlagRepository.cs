using Microsoft.EntityFrameworkCore;
using Db = efc.Database;

namespace efc;

internal class FeatureFlagRepository(Db.FeatureFlagsDbContext dbContext)
{
    internal async Task Save(Models.FeatureFlag featureModel)
    {
        var featureRecord = dbContext.ChangeTracker.Entries<Db.FeatureFlag>().FirstOrDefault(e => e.Entity.Name == featureModel.Name)?.Entity;
        featureRecord ??= await dbContext.FeatureFlags.Include(f => f.UserAssignments).ThenInclude(ua => ua.FeatureFlagUser).FirstOrDefaultAsync(f => f.Name == featureModel.Name);
        featureRecord ??= new Db.FeatureFlag { Name = featureModel.Name };
        MapModelToRecord(featureModel, featureRecord);
        await dbContext.SaveChangesAsync();
    }

    private void MapModelToRecord(Models.FeatureFlag model, Db.FeatureFlag record)
    {
        record.Name = model.Name;
        record.IsEnabled = model.IsEnabled;
        record.CreatedOnUtc = model.CreatedOnUtc;
        record.EnabledOn = model.EnabledOn;
        record.UpdatedOnUtc = model.UpdatedOnUtc;

        // Delete removed user assignments
        record.UserAssignments.RemoveAll(db => !model.UserAssignments.Any(m => m.UserName == db.FeatureFlagUser.UserName));
        var userRecords = record.UserAssignments.Select(ua => ua.FeatureFlagUser).ToList();
        // Update existing and add new user assignments
        foreach (var modelAssignment in model.UserAssignments)
        {
            var recordUser = userRecords.FirstOrDefault(u => u.UserName == modelAssignment.UserName);
            recordUser ??= dbContext.FeatureFlagUsers.FirstOrDefault(u => u.UserName == modelAssignment.UserName);
            recordUser ??= new Db.FeatureFlagUser { UserName = modelAssignment.UserName };
            
            var recordAssignment = record.UserAssignments.FirstOrDefault(db => db.FeatureFlagUser.UserName == modelAssignment.UserName);
            recordAssignment ??= new Db.FeatureFlagUserAssignment{ FeatureFlag = record, FeatureFlagUser = recordUser };
            MapModelToRecord(modelAssignment, recordAssignment);
        }
    }

    private void MapModelToRecord(Models.FeatureFlagUserAssignment model, Db.FeatureFlagUserAssignment record)
    {
        record.AssignedOnUtc = model.AssignedOnUtc;
    }

    private void MapModelToRecord(Models.FeatureFlagUser model, Db.FeatureFlagUser record)
    {
        record.UserName = model.UserName;
        record.CreatedOnUtc = model.CreatedOnUtc;
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