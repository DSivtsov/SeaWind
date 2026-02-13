using Application.Models;
using Infrastructure.Postgres.Identity;
using Infrastructure.Postgres.Main.SeederDto;
using Infrastructure.Postgres.Seeding.Seeders;
using Infrastructure.Postgres.Seeding.Shared;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Postgres.Main.Seeders
{
    internal class AppUserSeeder : BaseSeeder<AppIdentityDbContext, AppUser, AppUserSeederDto>
    {
        public override SeedOrder Order => SeedOrder.Base;

        protected override void AddNewEntity(AppIdentityDbContext dbContext, AppUserSeederDto recDemo)
        {
            var email = recDemo.Email;
            var emailNorm = email.ToUpperInvariant();
            AppUser user = new()
            {
                Id = recDemo.Id,                                // контролируемый Id
                UserName = email,
                NormalizedUserName = emailNorm,
                Email = email,
                NormalizedEmail = emailNorm,
                PasswordHash = recDemo.PasswordHash,            // валидный hash
                SecurityStamp = Guid.NewGuid().ToString(),      // непустой
                ConcurrencyStamp = Guid.NewGuid().ToString(),   // непустой
            };

            dbContext.Users.Add(user);
        }

        protected override async Task RemoveEntities(AppIdentityDbContext dbContext, CancellationToken ct)
        {
            var dbSet = dbContext.Users;
            dbSet.RemoveRange(await dbSet.ToListAsync(ct));
        }

        protected override async Task<AppUser?> TryFindEntity(AppIdentityDbContext dbContext, AppUserSeederDto recDemo, CancellationToken ct)
        {
            return await dbContext.Users.FirstOrDefaultAsync(c => c.Id == recDemo.Id, ct);
        }

        protected override void UpdateEntity(AppUser entity, AppUserSeederDto recDemo)
        {
            var email = recDemo.Email;
            var emailNorm = email.ToUpperInvariant();

            entity.Email = email;
            entity.UserName = email;

            entity.NormalizedEmail = emailNorm;
            entity.NormalizedUserName = emailNorm;

            entity.PasswordHash = recDemo.PasswordHash;
        }
    }
}
