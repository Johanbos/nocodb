using System.ComponentModel.DataAnnotations;

namespace efc.Database;

public class FeatureFlagUserAssignment
{
    [Key]
    public int Id { get; set; }
    public int FeatureFlagId { get; set; }
    public int FeatureFlagUserId { get; set; }
    public required FeatureFlagUser FeatureFlagUser { get; set; }
    public required FeatureFlag FeatureFlag { get; set; }
    public DateTime AssignedOnUtc { get; set; }
}