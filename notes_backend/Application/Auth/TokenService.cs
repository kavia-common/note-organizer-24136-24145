using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using NotesBackend.Domain.Entities;

namespace NotesBackend.Application.Auth
{
    /// <summary>
    /// JWT token generation service.
    /// </summary>
    public class TokenService : ITokenService
    {
        private readonly JwtSettings _settings;

        public TokenService(JwtSettings settings)
        {
            _settings = settings;
        }

        public string CreateToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Email, user.Email),
                new Claim("displayName", user.DisplayName),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            var token = new System.IdentityModel.Tokens.Jwt.JwtSecurityToken(
                issuer: _settings.Issuer,
                audience: _settings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_settings.ExpiryMinutes),
                signingCredentials: creds);

            return new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
