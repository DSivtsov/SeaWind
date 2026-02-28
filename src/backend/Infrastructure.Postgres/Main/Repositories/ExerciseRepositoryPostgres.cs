using Application.Abstractions.Repositories;
using Application.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Postgres.Main.Repositories;

public class ExerciseRepositoryPostgres : IExerciseRepository
{
    private readonly MainDbContext _mainDbContext;

    public ExerciseRepositoryPostgres(MainDbContext mainDbContext)
    {
        _mainDbContext = mainDbContext;
    }

    public async Task<Exercise?> GetExerciseByIdAsync(Guid exerciseId)
    {
        return await _mainDbContext.Exercises
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == exerciseId);
    }
}
