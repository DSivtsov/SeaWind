namespace Infrastructure.Postgres.Seeding.Shared
{
    internal class SeederDataException : Exception
    {
        public SeederDataException() { }

        public SeederDataException(string? message)
            : base(message) { }

        public SeederDataException(string? message, Exception? innerException)
            : base(message, innerException) { }
    }
}