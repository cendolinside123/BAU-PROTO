using BAU_PROTO.Persistence;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BAU_PROTO.Services.JwtService
{
    public class JwtService
    {
        private readonly string _key;
        private readonly IConfiguration _config;


        public JwtService(IConfiguration config)
        {
            _config = config;
            _key = Environment.GetEnvironmentVariable(ConstantConfig.Sec) ??
                throw new InvalidOperationException("Error server security config, go ask server owner");
        }

        public string GenerateToken(string username, string role, string refreshToken)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("refreshToken", refreshToken)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            return Guid.NewGuid().ToString();
        }

        public string RenewToken(string token, string refreshToken)
        {

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = GetUserInfoFromToken(token);
            
            var username = jwtToken.email;
            var claimNames = jwtToken.claimNames;
            var role = jwtToken.role;
            var refreshTokenClaim = jwtToken.refreshToken;
            var expiredDateFromToken = jwtToken.expiredDate;

            if (username == null || claimNames == null || role == null || refreshTokenClaim == null)
            {
                throw new JwtException(JwtErrorType.InvalidToken);
            }

            if (refreshTokenClaim != refreshToken)
            {
                throw new JwtException(JwtErrorType.RefreshTokenExpired);
            }

            if (expiredDateFromToken?.Date.AddDays(1) <= DateTime.UtcNow)
            {
                throw new JwtException(JwtErrorType.RefreshTokenExpired);
            }

            return this.GenerateToken(username, role, GenerateRefreshToken());
        }

        public (string? email, string? role, string? refreshToken, string? claimNames, DateTime? expiredDate) GetUserInfoFromToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            var email = jwtToken?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            var role = jwtToken?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            var refreshTokenClaim = jwtToken?.Claims.FirstOrDefault(c => c.Type == "refreshToken")?.Value;
            var claimNames = jwtToken?.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;
            var expiredDate = jwtToken?.ValidTo;

            return (email, role, refreshTokenClaim, claimNames, expiredDate);
        }
    }
}
