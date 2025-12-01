using Infrastructure.Postgres.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Api.Identity
{
    public interface ITokenService
    {
        string Create(AppUser user);
    }
    public class TokenService : ITokenService
    {
        private readonly IOptions<JwtOptions> option;
        private readonly SigningCredentials creds;

        public TokenService(IOptions<JwtOptions> option, SigningCredentials creds)
        {
            this.option = option;
            this.creds = creds;
        }

        public string Create(AppUser user)
        {
            var jwt = option.Value;
            
            var claims = new[] {
                new Claim("sub", user.Id),
                new Claim("email", user.Email!)
            };

            var token = new JwtSecurityToken(jwt.Issuer, jwt.Audience, claims,
                expires: DateTime.UtcNow.AddHours(1), signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

}
