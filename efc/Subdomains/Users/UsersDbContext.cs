using Microsoft.EntityFrameworkCore;

namespace efc.Subdomains.Users;

/// <summary>
/// Database context for Users. Update with:
/// ```
/// $subdomain = "Users"
/// $migrationName = "Awesome"
/// $dbcontext = $subdomain + "DbContext"
/// dotnet ef migrations add $migrationName --context $dbcontext
/// ```
/// </summary>
public class UsersDbContext : DbContext
{
    static readonly string connectionString = "Server=localhost;Database=backend-v2;User Id=root;Password=localhost;";

    public DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
    }
}
