using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Postgres.Identity;

public class AppIdentityDbContext : IdentityDbContext<AppUser, IdentityRole, string>
{
    public const string Schema = "identity";
    public AppIdentityDbContext(DbContextOptions<AppIdentityDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.HasDefaultSchema(Schema);

        builder.Entity<IdentityRole>().HasData(
            new IdentityRole
            {
                Id = "role-freestudent",
                Name = "FreeStudent",
                NormalizedName = "FREESTUDENT"
            },
            new IdentityRole
            {
                Id = "role-student",
                Name = "Student",
                NormalizedName = "STUDENT"
            },
            new IdentityRole
            {
                Id = "role-mentor",
                Name = "Mentor",
                NormalizedName = "MENTOR"
            },
            new IdentityRole
            {
                Id = "role-admin",
                Name = "Admin",
                NormalizedName = "ADMIN"
            }
        );
    }
}
