using System.Text.Json.Serialization;

namespace Api.Dtos;

public record UsersQueryDto(string? UserName, string? Role);
