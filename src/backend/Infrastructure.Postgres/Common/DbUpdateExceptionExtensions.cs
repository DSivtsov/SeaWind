using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Infrastructure.Postgres.Common;


public static class DbUpdateExceptionExtensions
{
    public static bool IsUniqueViolation(this DbUpdateException ex)
    {
        return ex.InnerException is PostgresException pg && pg.SqlState == "23505";
    }
}
