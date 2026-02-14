using System.ComponentModel.DataAnnotations;

namespace efc.Database;

public class FeatureFlagUser
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string UserName { get; set; } = string.Empty;
    public DateTime CreatedOnUtc { get; set; }
    public List<FeatureFlagUserAssignment> FeatureFlagAssignments { get; set; } = [];
}
