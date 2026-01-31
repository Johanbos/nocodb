using System.ComponentModel.DataAnnotations;

namespace efc.Subdomains.Users;

public class User
{
    [Key]
    public int Id { get; set; }
    public required string Name { get; set; }
    public DateTime CreatedOnUtc { get; set; }
}