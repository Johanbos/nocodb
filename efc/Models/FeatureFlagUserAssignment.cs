
namespace efc.Models;

public class FeatureFlagUserAssignment(FeatureFlagUser featureFlagUser, DateTime assignedOnUtc)
{
    public FeatureFlagUser FeatureFlagUser { get; private set; } = featureFlagUser;
    public DateTime AssignedOnUtc { get; private set; } = assignedOnUtc;
    public FeatureFlag? FeatureFlag { get; internal set; }
}