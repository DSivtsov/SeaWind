using Infrastructure.Postgres.Identity;
using Infrastructure.Postgres.Main.SeederDto;
using Infrastructure.Postgres.Seeding.Seeders;
using Infrastructure.Postgres.Seeding.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Postgres.Main.Seeders
{
    internal class AspNetUserRolesSeeder : BaseSeeder<AppIdentityDbContext, IdentityUserRole<string>, AspNetUserRolesSeederDto>
    {
        public override SeedOrder Order => SeedOrder.Core;

        protected override void AddNewEntity(AppIdentityDbContext dbContext, AspNetUserRolesSeederDto recDemo)
        {
            IdentityUserRole<string> userRole = new()
            {
                UserId = recDemo.UserId,                                
                RoleId = recDemo.RoleId,
            };

            dbContext.UserRoles.Add(userRole);
        }

        protected override async Task RemoveEntities(AppIdentityDbContext dbContext, CancellationToken ct)
        {
            var dbSet = dbContext.UserRoles;
            dbSet.RemoveRange(await dbSet.ToListAsync(ct));
        }

        protected override async Task<IdentityUserRole<string>?> TryFindEntity(AppIdentityDbContext dbContext, AspNetUserRolesSeederDto recDemo, CancellationToken ct)
        {
            return await dbContext.UserRoles.FirstOrDefaultAsync(c => c.UserId == recDemo.UserId, ct);
        }

        protected override void UpdateEntity(IdentityUserRole<string> entity, AspNetUserRolesSeederDto recDemo)
        {
            entity.RoleId = recDemo.RoleId;
        }
    }
}
