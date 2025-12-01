namespace Infrastructure.Postgres.Time.SeederDto;

public record CarSeederDto(Guid Id, string Model, Guid Owner, string RegNumber);

