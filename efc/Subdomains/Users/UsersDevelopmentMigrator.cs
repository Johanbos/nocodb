using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace efc.Subdomains.Users;

public class UsersDevelopmentMigrator(IHostEnvironment environment)
{
    private IHostEnvironment environment = environment;
    private UsersDbContext ffContext = new UsersDbContext();

    public async Task Migrate()
    {
        if (!environment.IsDevelopment())
            return;
        
        await ffContext.Database.MigrateAsync();
        await Seed();
        
        /* In Staging & Production use in pipeline:
        ```
        dotnet tool install --global dotnet-ef
        ForEach ($dbcontext in (dotnet ef dbcontext list --no-build)) { dotnet ef database update --context $dbcontext }
        ```
        */
    }

    private async Task Seed()
    {
        if (await ffContext.Users.CountAsync() == 0)
        {
            var user1 = new User { Name = "User1", CreatedOnUtc = DateTime.UtcNow };
            var user2 = new User { Name = "User2", CreatedOnUtc = DateTime.UtcNow };
            ffContext.Users.AddRange(user1, user2);
            await ffContext.SaveChangesAsync();
        }
    }
}