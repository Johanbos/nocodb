using System.ComponentModel.DataAnnotations;

namespace efc.Subdomains.FeatureFlags;

public class FeatureFlagUserAssignment
{
    [Key]
    public int Id { get; set; }
    public int FeatureFlagId { get; set; }
    public int FeatureFlagUserId { get; set; }
    public DateTime AssignedOnUtc { get; set; }
}