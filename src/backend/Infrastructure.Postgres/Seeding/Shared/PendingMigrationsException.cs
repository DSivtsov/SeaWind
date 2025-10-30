namespace Infrastructure.Postgres.Seeding.Shared
{
    internal class PendingMigrationsException : Exception
    {
        public IReadOnlyList<string> PendingMigrations { get; }

        public PendingMigrationsException(IEnumerable<string> pending)
            : base($"Exist Active Migrations ({pending.Count()}): [{string.Join(", ", pending)}]")
        {
            PendingMigrations = pending.ToArray();
        }

        public PendingMigrationsException(IEnumerable<string> pending, Exception? innerException)
            : base($"Exist Active Migrations ({pending.Count()}): [{string.Join(", ", pending)}]", innerException)
        {
            PendingMigrations = pending.ToArray();
        }
    }
}