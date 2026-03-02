using Application.Common;
using Application.Models;
using Infrastructure.Postgres.Main.SeederDto;
using Infrastructure.Postgres.Seeding.Seeders;
using Infrastructure.Postgres.Seeding.Shared;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Infrastructure.Postgres.Main.Seeders
{
    internal class ExerciseContentSeeder : BaseSeeder<MainDbContext, ExerciseContent, ExerciseContentSeederDto>
    {
        public override SeedOrder Order => SeedOrder.Links;

        protected override void AddNewEntity(MainDbContext dbContext, ExerciseContentSeederDto recDemo)
        {
            var blocks = recDemo.ContentBlocks ?? new();
            string contentBlocksJson = JsonSerializer.Serialize(blocks, AppJson.SerializerOpt);
            ExerciseContent entity = new(recDemo.ExerciseId, recDemo.Details, contentBlocksJson);
            dbContext.ExerciseContents.Add(entity);
        }

        protected override async Task RemoveEntities(MainDbContext dbContext, CancellationToken ct)
        {
            var dbSet = dbContext.ExerciseContents;
            dbSet.RemoveRange(await dbSet.ToListAsync(ct));
        }

        protected override async Task<ExerciseContent?> TryFindEntity(MainDbContext dbContext, ExerciseContentSeederDto recDemo,
            CancellationToken ct)
        {
            return await dbContext.ExerciseContents.FirstOrDefaultAsync(c => c.ExerciseId == recDemo.ExerciseId, ct);
        }

        protected override void UpdateEntity(ExerciseContent entity, ExerciseContentSeederDto recDemo)
        {
            var blocks = recDemo.ContentBlocks ?? new();
            string contentBlocksJson = JsonSerializer.Serialize(blocks, AppJson.SerializerOpt);
            entity.Details = recDemo.Details;
            entity.ContentBlocksJson = contentBlocksJson;
        }
    }
}
