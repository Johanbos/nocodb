using System.ComponentModel.DataAnnotations;

namespace efc.Models;

public class FeatureFlagUser(string UserName, DateTime CreatedOnUtc)
{
    public string UserName { get; private set; } = UserName;
    public DateTime CreatedOnUtc { get; private set; } = CreatedOnUtc;
    public List<FeatureFlagUserAssignment> FeatureFlagAssignments { get; internal set; } = [];
}
