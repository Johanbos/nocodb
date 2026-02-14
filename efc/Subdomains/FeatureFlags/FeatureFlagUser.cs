using System.ComponentModel.DataAnnotations;

namespace efc.Subdomains.FeatureFlags;

public class FeatureFlagUser
{
    [Key]
    public int Id { get; set; }

    [Required]
    public required string UserName { get; set; }
    public DateTime CreatedOnUtc { get; set; }
    public List<FeatureFlagUserAssignment> FeatureFlagAssignments { get; set; } = new();
}
