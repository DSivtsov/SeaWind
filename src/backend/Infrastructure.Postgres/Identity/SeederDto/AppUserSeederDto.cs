namespace Infrastructure.Postgres.Main.SeederDto;

/// <summary>
/// AppUserSeederDto транспортный объект (data transfer)
/// </summary>
public record AppUserSeederDto(string Id, string Email, string PasswordHash);
