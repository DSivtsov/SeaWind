using System.ComponentModel.DataAnnotations;

namespace Application.DtoAdmin;

public sealed record UpdateUserRoleRequest
{
    [Required]
    public string Role { get; init; } = null!;
}
