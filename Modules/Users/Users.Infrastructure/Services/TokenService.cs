
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Users.Infrastructure.Config;
using Users.Domain.Entities;


namespace Users.Infrastructure.Services
{
    public class TokenService
    {
        private readonly JwtSettings _jwtSettings;

        public TokenService(IOptions<JwtSettings> jwtOptions)
        {
            _jwtSettings = jwtOptions.Value;
        }

        // ✅ Generate Access Token
        public string GenerateAccessToken(User user, IEnumerable<string> permissions)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
                new Claim(ClaimTypes.Name, user.Username ?? ""),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim("IsConfirmed", user.IsConfirmed.ToString())
            };

            // ✅ Add Roles from UserRoles
            if (user.UserRoles != null)
            {
                foreach (var userRole in user.UserRoles)
                {
                    if (userRole.Role != null && !string.IsNullOrWhiteSpace(userRole.Role.Code))
                    {
                        claims.Add(new Claim(ClaimTypes.Role, userRole.Role.Code)); 
                    }
                }
            }

            // ✅ Add Permissions (custom claims)
            foreach (var permission in permissions.Distinct())
            {
                claims.Add(new Claim("Permission", permission));
            }

            // ✅ Build JWT token
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // ✅ Generate Refresh Token
        public string GenerateRefreshToken()
        {
            return Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        }

        public object GenerateToken(string username, string[] strings)
        {
            throw new NotImplementedException();
        }
    }
}
