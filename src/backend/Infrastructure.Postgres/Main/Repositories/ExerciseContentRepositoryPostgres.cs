using Application.Abstractions.Repositories;
using Application.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Postgres.Main.Repositories;

public class ExerciseContentRepositoryPostgres : IExerciseContentRepository
{
    private readonly MainDbContext _mainDbContext;

    public ExerciseContentRepositoryPostgres(MainDbContext mainDbContext)
    {
        _mainDbContext = mainDbContext;
    }

    public async Task<ExerciseContent?> GetByExerciseContentByIdAsync(Guid exerciseId)
    {
        return await _mainDbContext.ExerciseContents
        .AsNoTracking()
        .FirstOrDefaultAsync(x => x.ExerciseId == exerciseId);
    }
}
