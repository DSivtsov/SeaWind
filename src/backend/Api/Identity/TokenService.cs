using Api.Exceptions;
using Application.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Api.Identity
{
    public interface ITokenService
    {
        Task<string> CreateAsync(AppUser user);
    }
    public class TokenService : ITokenService
    {
        // В рамках MVP JWT_TTL 2 hours
        private const int JWT_TTL = 2;
        private readonly IOptions<JwtOptions> option;
        private readonly SigningCredentials creds;
        private readonly UserManager<AppUser> _userManager;

        public TokenService(IOptions<JwtOptions> option, SigningCredentials creds, UserManager<AppUser> userManager)
        {
            this.option = option;
            this.creds = creds;
            _userManager = userManager;
        }

        public async Task<string> CreateAsync(AppUser user)
        {
            IList<string> list;
            try
            {
                list = await _userManager.GetRolesAsync(user);
                
                // у каждого пользователя только одна роль
                if (list.Count != 1)
                    throw new InvariantViolationException("Ошибка аутентификации.");
            }
            catch (Exception)
            {
                // В рамках MVP роли фиксированы и создаются через миграции (HasData),
                // поэтому GetRolesAsync здесь считается детерминированным
                throw new InvariantViolationException("Ошибка аутентификации.");
            }

            var jwt = option.Value;
            
            var claims = new[] {
                new Claim("sub", user.Id),
                new Claim("email", user.Email!),
                new Claim(ClaimTypes.Role, list[0])
            };

            var token = new JwtSecurityToken(jwt.Issuer, jwt.Audience, claims,
                expires: DateTime.UtcNow.AddHours(JWT_TTL), signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

}
