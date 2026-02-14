using System.ComponentModel.DataAnnotations;

namespace efc.Subdomains.FeatureFlags;

public class FeatureFlag
{
    [Key]
    public int Id { get; set; }
    public required string Name { get; set; }
    public bool IsEnabled { get; set; }
    public DateTime? EnabledOn { get; set; }
    public DateTime CreatedOnUtc { get; set; }
    public DateTime? UpdatedOnUtc { get; set; }
    public List<FeatureFlagUserAssignment> UserAssignments { get; set; } = new();
}
