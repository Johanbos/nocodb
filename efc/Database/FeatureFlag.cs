using System.ComponentModel.DataAnnotations;

namespace efc.Database;

public class FeatureFlag
{
    [Key]
    public int Id { get; set; }
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    public bool IsEnabled { get; set; }
    public DateTime? EnabledOn { get; set; }
    public DateTime CreatedOnUtc { get; set; }
    public DateTime? UpdatedOnUtc { get; set; }
    public List<FeatureFlagUserAssignment> UserAssignments { get; set; } = [];
}
