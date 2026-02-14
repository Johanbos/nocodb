
namespace efc.Models;

public class FeatureFlagUserAssignment(string userName, DateTime assignedOnUtc)
{
    public string UserName { get; private set; } = userName;
    public DateTime AssignedOnUtc { get; private set; } = assignedOnUtc;
}